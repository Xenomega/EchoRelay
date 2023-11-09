using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Common;
using EchoRelay.Core.Server.Messages.Matching;
using EchoRelay.Core.Server.Services.ServerDB;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using static EchoRelay.Core.Server.Messages.ServerDB.ERGameServerStartSession;

namespace EchoRelay.Core.Server.Services.Matching
{
    public class MatchingService : Service
    {
        public MatchingService(Server server) : base(server, "MATCHING")
        {
        }

        /// <summary>
        /// Handles a packet being received by a peer.
        /// This is called after all events have been fired for <see cref="OnPacketReceived"/>.
        /// </summary>
        /// <param name="sender">The peer which sent the packet.</param>
        /// <param name="packet">The packet sent by the peer.</param>
        protected override async Task HandlePacket(Peer sender, Packet packet)
        {
            // Loop for each message received in the packet
            foreach (Message message in packet)
            {
                switch (message)
                {
                    case LobbyCreateSessionRequestv9 createSessionRequestv9:
                        await ProcessCreateSessionRequestv9(sender, createSessionRequestv9);
                        break;
                    case LobbyFindSessionRequestv11 findSessionRequestv11:
                        await ProcessFindSessionRequestv11(sender, findSessionRequestv11);
                        break;
                    case LobbyJoinSessionRequestv7 joinSessionRequestv7:
                        await ProcessJoinSessionRequestv7(sender, joinSessionRequestv7);
                        break;
                    case LobbyPendingSessionCancel pendingSessionCancel:
                        await ProcessPendingSessionCancel(sender, pendingSessionCancel);
                        break;
                    case LobbyMatchmakerStatusRequest matchmakerStatusRequest:
                        break;
                    case LobbyPingResponse pingResponse:
                        await ProcessPingResponse(sender, pingResponse);
                        break;
                    case LobbyPlayerSessionsRequestv5 playerSessionsRequestv5:
                        await ProcessPlayerSessionsRequestv5(sender, playerSessionsRequestv5);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a <see cref="LobbyCreateSessionRequestv9"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessCreateSessionRequestv9(Peer sender, LobbyCreateSessionRequestv9 request)
        {
            // Set the matching data for our user to provide context to matching operations moving forward.
            sender.SetSessionData(MatchingSession.FromCreateSessionCriteria(request.UserId, request.ChannelUUID, request.GameTypeSymbol, request.LevelSymbol, request.LobbyType, (TeamIndex)request.TeamIndex, request.SessionSettings));

            // Process the underlying request.
            await ProcessMatchingSession(sender, request.Session, request.UserId);
        }

        /// <summary>
        /// Processes a <see cref="LobbyFindSessionRequestv11"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessFindSessionRequestv11(Peer sender, LobbyFindSessionRequestv11 request)
        {
            // Set the matching data for our user to provide context to matching operations moving forward.
            sender.SetSessionData(MatchingSession.FromFindSessionCriteria(request.UserId, request.ChannelUUID, request.GameTypeSymbol, (TeamIndex)request.TeamIndex, request.SessionSettings));

            // Process the underlying request.
            await ProcessMatchingSession(sender, request.Session, request.UserId);
        }

        /// <summary>
        /// Processes a <see cref="LobbyJoinSessionRequestv7"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessJoinSessionRequestv7(Peer sender, LobbyJoinSessionRequestv7 request)
        {
            // Set the matching data for our user to provide context to matching operations moving forward.
            sender.SetSessionData(MatchingSession.FromJoinSpecificSessionCriteria(request.UserId, request.LobbyUUID, (TeamIndex)request.TeamIndex, request.SessionSettings));

            // Process the underlying request.
            await ProcessMatchingSession(sender, request.Session, request.UserId);
        }

        /// <summary>
        /// Processes the underlying data derived from <see cref="LobbyFindSessionRequestv11"/>, 
        /// <see cref="LobbyFindSessionRequestv11"/>, or <see cref="LobbyJoinSessionRequestv7"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        private async Task ProcessMatchingSession(Peer sender, Guid session, XPlatformId userId)
        {
            // Verify the session details provided
            if (!Server.LoginService.CheckUserSessionValid(session, userId))
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.BadRequest, "Unauthorized");
                return;
            }

            // Verify the account behind the request.
            AccountResource? account = Storage.Accounts.Get(userId);
            if (account == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.BadRequest, "Failed to obtain profile");
                return;
            }

            // Check if the user is banned, if so, disallow them from matching.
            if (account.Banned)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.BannedFromLobbyGroup, $"Banned until: {account.BannedUntil!.Value:MM/dd/yyyy @ hh:mm:ss tt} (UTC)");
                return;
            }

            // Set the authenticated user information.
            sender.UpdateUserAuthentication(userId, account.Profile.Server.DisplayName);

            // Obtain the user's matching session
            MatchingSession? matchingSession = sender.GetSessionData<MatchingSession>();
            if (matchingSession == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.InternalError, "Cannot process session request, no matching session exists");
                return;
            }

            // Validate the user is not requesting to be a moderator when they are not one.
            if (matchingSession.TeamIndex == TeamIndex.Moderator && !account.IsModerator)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.NotALobbyGroupMod, "User is not a moderator");
                return;
            }

            // Send the status to the user.
            // TODO: This should be a response to LobbyMatchmakerStatusRequest and should be relocated.
            //  That request is sent along with this request we are currently processing, so it is technically fine to respond here (for the client), but it's just ugly in terms of code correctness.
            await sender.Send(new LobbyMatchmakerStatus(0));

            // If we were provided a lobby/session identifier (via LobbyJoinSessionRequestv7), search for the game server directly.
            // This uses a special lookup method using the lobby id, that is faster than filtering all game servers.
            if (matchingSession.LobbyId != null)
            {
                RegisteredGameServer? requestedGameServer = Server.ServerDBService.Registry.GetGameServer(matchingSession.LobbyId.Value);
                if (requestedGameServer == null)
                {
                    await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.ServerDoesNotExist, "Could not find requested lobby id");
                    return;
                }

                // Obtain our session from the game server.
                await requestedGameServer.ProcessLobbySessionRequest(sender);
                return;
            }

            // This is a create lobby, or find lobby request. We will try to find an existing server that matches the request.
            // Filter game servers, produce ping request endpoint data.
            // We limit the amount to 100, to avoid the response hitting the max packet size.
            var gameServers = Server.ServerDBService.Registry.FilterGameServers(
                findMax: 100,
                sessionId: matchingSession.LobbyId,
                gameTypeSymbol: matchingSession.GameTypeSymbol,
                levelSymbol: matchingSession.LevelSymbol,
                channel: matchingSession.Channel,
                locked: false,
                lobbyTypes: matchingSession.SearchLobbyTypes,
                requestedTeam: matchingSession.TeamIndex,
                unfilledServerOnly: true
            );

            // If we only have one game server, immediately connect the peer. Otherwise, perform a ping request to determine the lowest ping server.
            if (gameServers.Count() == 1)
            {
                // Process the new session request, to get the peer the information it needs to connect to the lobby.
                await gameServers.First().ProcessLobbySessionRequest(sender);
            }
            else
            {
                // Construct our endpoint data for the ping request, from the game servers we got in our previous query.
                var pingEndpoints = new LobbyPingRequestv3.EndpointData[gameServers.Count()];
                int current = 0;
                foreach (var gameServer in gameServers)
                {
                    pingEndpoints[current++] = new LobbyPingRequestv3.EndpointData(
                        gameServer.InternalAddress,
                        gameServer.ExternalAddress,
                        gameServer.Port
                        );
                }

                // Send a ping request to the peer.
                await sender.Send(new LobbyPingRequestv3(0, 4, 100, pingEndpoints));
            }
            await sender.Send(new TcpConnectionUnrequireEvent());
        }

        /// <summary>
        /// Processes a <see cref="LobbyPendingSessionCancel"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessPendingSessionCancel(Peer sender, LobbyPendingSessionCancel request)
        {
            // Clear the matching session data for this peer.
            sender.ClearSessionData();
            await sender.Send(new TcpConnectionUnrequireEvent());
        }

        /// <summary>
        /// Processes a <see cref="LobbyPingResponse"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessPingResponse(Peer sender, LobbyPingResponse request)
        {
            // Obtain the user's matching session
            MatchingSession? matchingSession = sender.GetSessionData<MatchingSession>();
            if (matchingSession == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.InternalError, "Ping response given, but no matching session exists");
                return;
            }

            // Try to select a game server
            RegisteredGameServer? selectedGameServer = null;

            // If we have no results, there are likely no game servers available to serve the request.
            // See if the user specified that they wish to force users in this scenario to join any available server.
            if (request.Results.Length == 0)
            {
                if (Server.Settings.ForceIntoAnySessionIfCreationFails)
                {
                    // Resolve the most populated available game server with open space and select it.
                    selectedGameServer = Server.ServerDBService.Registry.FilterGameServers(locked: false, requestedTeam: matchingSession.TeamIndex, unfilledServerOnly: true, lobbyTypes: new LobbyType[] {LobbyType.Unassigned, LobbyType.Public})
                        .MaxBy(x => (float)x.SessionPlayerCount / x.SessionPlayerLimits.TotalPlayerLimit);
                } 
                else
                {
                    await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.ServerFindFailed, "Could not receive a ping response from any game servers");
                    return;
                }
            }
            else
            {
                // Convert the ping results to a lookup
                Dictionary<(uint InternalAddress, uint ExternalAddress), uint> pingResultLookup = request.Results.ToDictionary(x => (x.InternalAddress.ToUInt32(), x.ExternalAddress.ToUInt32()), x => x.PingMilliseconds);

                // Resolve game servers matching this address with any other provided lookup criteria.
                var gameServers = Server.ServerDBService.Registry.FilterGameServers(
                    addresses: pingResultLookup.Keys.ToHashSet(),
                    sessionId: matchingSession.LobbyId,
                    gameTypeSymbol: matchingSession.GameTypeSymbol,
                    levelSymbol: matchingSession.LevelSymbol,
                    channel: matchingSession.Channel,
                    locked: false,
                    lobbyTypes: matchingSession.SearchLobbyTypes,
                    requestedTeam: matchingSession.TeamIndex,
                    unfilledServerOnly: true
                );
                
                // All servers should either have no session started, or match the criteria we filtered for.
                // Depending on our matching strategy, we will first sort by population or ping, followed by the latter.
                // The most optimal game server will be selected.
                if (Server.Settings.FavorPopulationOverPing)
                {
                    // Select the game server which is most full.
                    selectedGameServer = gameServers.MaxBy(x => (float)x.SessionPlayerCount / x.SessionPlayerLimits.TotalPlayerLimit);
                } 
                else
                {
                    // Sort the game servers with preference of filters: session started, lowest ping, highest player count.
                    var sortedGameServers = gameServers.Select(gameServer => {
                        uint? pingMilliseconds = pingResultLookup.TryGetValue((gameServer.InternalAddress.ToUInt32(), gameServer.ExternalAddress.ToUInt32()), out uint p) ? p : uint.MaxValue;
                        return (gameServer, pingMilliseconds);
                    }).OrderBy(x => x.gameServer.SessionStarted ? 0 : 1).ThenBy(x => x.pingMilliseconds).ThenBy(x => (float)x.gameServer.SessionPlayerCount / x.gameServer.SessionPlayerLimits.TotalPlayerLimit);

                    // Select the first game server.
                    selectedGameServer = sortedGameServers.FirstOrDefault().gameServer;
                }
            }

            // Verify that a game server candidate was found.
            if (selectedGameServer == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.ServerFindFailed, "Could not obtain registered game server to serve request.");
                return;
            }

            // Process the new session request, to get the peer the information it needs to connect to the lobby.
            await selectedGameServer.ProcessLobbySessionRequest(sender);
        }

        /// <summary>
        /// Processes a <see cref="LobbyPlayerSessionsRequestv5"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessPlayerSessionsRequestv5(Peer sender, LobbyPlayerSessionsRequestv5 request)
        {
            // Verify the session details provided
            if (!Server.LoginService.CheckUserSessionValid(request.Session, request.UserId))
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.BadRequest, "Unauthorized");
                return;
            }

            // Obtain the user's matching session
            MatchingSession? matchingSession = sender.GetSessionData<MatchingSession>();
            if (matchingSession == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.InternalError, "Player sessions requested, but no matching session exists");
                return;
            }

            if (matchingSession.MatchedGameServer == null)
            {
                await SendLobbySessionFailure(sender, LobbySessionFailureErrorCode.InternalError, "Player sessions requested, but no matched game server exists");
                return;
            }

            // Coordinate the player session request with the game server.
            await matchingSession.MatchedGameServer.ProcessPlayerSessionRequest(sender, request.UserId, matchingSession.Channel ?? new Guid());
            sender.ClearSessionData();
        }

        /// <summary>
        /// Sends all versions of the lobby session failure message to a peer, indicating that
        /// a matching operation failed. This method does nothing if the peer has no active matching session.
        /// </summary>
        /// <param name="peer">The peer to send the message to.</param>
        /// <param name="errorCode">The error code to send for the failure.</param>
        /// <param name="errorMessage">The error message to send.</param>
        /// <returns></returns>
        internal async Task SendLobbySessionFailure(Peer peer, LobbySessionFailureErrorCode errorCode, string errorMessage)
        {
            // Obtain the peer's matching session data.
            MatchingSession? matchingSession = peer.GetSessionData<MatchingSession>();
            if (matchingSession == null)
                return;

            // Clear the matching session data.
            peer.ClearSessionData();

            // Define the arguments for our failure messages.
            long gameTypeSymbol = matchingSession.GameTypeSymbol ?? -1;
            Guid channel = matchingSession.Channel ?? matchingSession.LobbyId ?? new Guid();

            // Send the failure messages.
            await peer.Send(new LobbySessionFailurev1(errorCode));
            await peer.Send(new LobbySessionFailurev2(channel, errorCode));
            await peer.Send(new LobbySessionFailurev3(gameTypeSymbol, channel, errorCode, 0));
            await peer.Send(new LobbySessionFailurev4(gameTypeSymbol, channel, errorCode, 0, errorMessage));
        }
    }
}
