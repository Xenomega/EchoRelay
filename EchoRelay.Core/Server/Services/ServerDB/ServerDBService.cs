using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Common;
using EchoRelay.Core.Server.Messages.ServerDB;
using System.Collections.Specialized;
using System.Web;

namespace EchoRelay.Core.Server.Services.ServerDB
{
    public class ServerDBService : Service
    {
        public GameServerRegistry Registry { get; }
        public ServerDBService(Server server) : base(server, "SERVERDB")
        {
            Registry = new GameServerRegistry();
            OnPeerDisconnected += ServerDBService_OnPeerDisconnected;
        }

        private void ServerDBService_OnPeerDisconnected(Service service, Peer peer)
        {
            ClearPeerRegistration(peer);
        }

        private void ClearPeerRegistration(Peer peer)
        {
            // If this peer registered a game server, remove it from the registry.
            RegisteredGameServer? registeredGameServer = peer.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer != null)
            {
                Registry.RemoveGameServer(registeredGameServer);
            }

            // Clear any session data.
            peer.ClearSessionData();
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
                    case ERGameServerRegistrationRequest registrationRequest:
                        await ProcessLobbyRegistrationRequest(sender, registrationRequest);
                        break;
                    case ERGameServerSessionStarted sessionStarted:
                        await ProcessSessionStarted(sender, sessionStarted);
                        break;
                    case ERGameServerEndSession endSession:
                        await ProcessEndSession(sender, endSession);
                        break;
                    case ERGameServerPlayerSessionsLocked sessionLocked:
                        await ProcessPlayerSessionsLocked(sender, sessionLocked);
                        break;
                    case ERGameServerPlayerSessionsUnlocked sessionUnlocked:
                        await ProcessPlayerSessionsUnlocked(sender, sessionUnlocked);
                        break;
                    case ERGameServerAcceptPlayers acceptPlayers:
                        await ProcessAcceptPlayers(sender, acceptPlayers);
                        break;
                    case ERGameServerRemovePlayer removePlayer:
                        await ProcessRemovePlayer(sender, removePlayer);
                        break;

                }
            }
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerRegistrationRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessLobbyRegistrationRequest(Peer sender, ERGameServerRegistrationRequest request)
        {
            // Clear a previous registration if it exists.
            ClearPeerRegistration(sender);

            // Obtain our service query parameters, so we can check the API key.
            NameValueCollection queryStrings = HttpUtility.ParseQueryString(sender.RequestUri.Query);
            string? apiKey = queryStrings.Get("api_key");


            // Validate the API key if we enforce one.
            if (Server.Settings.ServerDBApiKey != null && apiKey != Server.Settings.ServerDBApiKey)
            {
                await sender.Send(new LobbyRegistrationFailure(LobbyRegistrationFailure.FailureCode.DatabaseError));
                return;
            }

            // Register the game server and update our session data with it.
            RegisteredGameServer registeredGameServer = Registry.RegisterGameServer(sender, request);
            sender.SetSessionData(registeredGameServer);

            // Send our registration success message.
            await sender.Send(new LobbyRegistrationSuccess(request.ServerId, sender.Address));
            await sender.Send(new TcpConnectionUnrequireEvent());
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerSessionStarted"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessSessionStarted(Peer sender, ERGameServerSessionStarted request)
        {
            // This is here if we need it, but we assume the session started when a session start message is sent.
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerEndSession"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessEndSession(Peer sender, ERGameServerEndSession request)
        {
            // Obtain the registered game server
            RegisteredGameServer? registeredGameServer = sender.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer == null)
                return;

            // Update the session started status.
            await registeredGameServer.EndSession();
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerPlayerSessionsLocked"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessPlayerSessionsLocked(Peer sender, ERGameServerPlayerSessionsLocked request)
        {
            // Obtain the registered game server
            RegisteredGameServer? registeredGameServer = sender.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer == null)
                return;

            // Update the locked status.
            registeredGameServer.SetLockedStatus(true);
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerPlayerSessionsUnlocked"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessPlayerSessionsUnlocked(Peer sender, ERGameServerPlayerSessionsUnlocked request)
        {
            // Obtain the registered game server
            RegisteredGameServer? registeredGameServer = sender.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer == null)
                return;

            // Update the locked status.
            registeredGameServer.SetLockedStatus(false);
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerAcceptPlayers"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessAcceptPlayers(Peer sender, ERGameServerAcceptPlayers request)
        {
            // Obtain the registered game server
            RegisteredGameServer? registeredGameServer = sender.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer == null)
                return;

            // Add the provided player sessions to the associated game server.
            await registeredGameServer.AddPlayers(request.PlayerSessions);
        }

        /// <summary>
        /// Processes a <see cref="ERGameServerRemovePlayer"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessRemovePlayer(Peer sender, ERGameServerRemovePlayer request)
        {
            // Obtain the registered game server
            RegisteredGameServer? registeredGameServer = sender.GetSessionData<RegisteredGameServer>();
            if (registeredGameServer == null)
                return;

            // Remove the provided player session from the associated game server.
            await registeredGameServer.RemovePlayer(request.PlayerSession);
        }
    }
}
