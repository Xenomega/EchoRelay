using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Common;
using EchoRelay.Core.Server.Messages.ServerDB;
using System.Collections.Specialized;
using System.Web;
using static EchoRelay.Core.Server.Services.ServerDB.ServerDBService;

namespace EchoRelay.Core.Server.Services.ServerDB
{
    public class ServerDBService : Service
    {
        #region Properties
        /// <summary>
        /// The registry maintaining all registered game servers.
        /// </summary>
        public GameServerRegistry Registry { get; }
        #endregion

        #region Events
        /// <summary>
        /// Event of a peer failing game server registration.
        /// </summary>
        /// <param name="peer">The peer which failed registration.</param>
        /// <param name="registrationRequest">The registration request which was denied.</param>
        public delegate void GameServerRegistrationFailure(Peer peer, ERGameServerRegistrationRequest registrationRequest, string failureMessage);
        /// <summary>
        /// Event of a game server failing registration.
        /// </summary>
        public event GameServerRegistrationFailure? OnGameServerRegistrationFailure;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a ServerDB service attached to the given server.
        /// </summary>
        /// <param name="server">The server to initialize the serverdb service with.</param>
        public ServerDBService(Server server) : base(server, "SERVERDB")
        {
            Registry = new GameServerRegistry();
            OnPeerDisconnected += ServerDBService_OnPeerDisconnected;
        }
        #endregion

        #region Functions
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
                OnGameServerRegistrationFailure?.Invoke(sender, request, "Bad API key");
                await sender.Send(new LobbyRegistrationFailure(LobbyRegistrationFailure.FailureCode.DatabaseError));
                return;
            }

            // Create the game server object, then validate it.
            RegisteredGameServer registeredGameServer = new RegisteredGameServer(Registry, sender, request);
            if (Server.Settings.ServerDBValidateServerEndpoint && !(await GameServerPingClient.CheckAvailable(registeredGameServer, Server.Settings.ServerDBValidateServerEndpointTimeout)))
            {
                OnGameServerRegistrationFailure?.Invoke(sender, request, "Raw ping request/acknowledgement failed. The game server could not be connected to. It may not have exposed its ports properly.");
                await sender.Send(new LobbyRegistrationFailure(LobbyRegistrationFailure.FailureCode.ConnectionFailed));
                return;
            }

            // Add the game server to the registry
            Registry.AddGameServer(registeredGameServer);

            // Update our session data with the registered game server
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
        #endregion
    }
}
