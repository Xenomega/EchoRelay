using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages.Matching;
using EchoRelay.Core.Server.Messages.ServerDB;
using EchoRelay.Core.Server.Services.Matching;
using EchoRelay.Core.Utils;
using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace EchoRelay.Core.Server.Services.ServerDB
{
    /// <summary>
    /// A provider which tracks a game server which has registered with a <see cref="ServerDBService"/> service.
    /// It fulfills matching requests between the <see cref="Matching.MatchingService"/> service and the 
    /// <see cref="ServerDBService"/> service.
    /// </summary>
    public class RegisteredGameServer
    {
        #region Fields/Properties
        /// <summary>
        /// The parent <see cref="GameServerRegistry"/> which the <see cref="RegisteredGameServer"/> is registered to.
        /// </summary>
        private GameServerRegistry Registry { get; }

        /// <summary>
        /// The registration request provided by the game server initially.
        /// </summary>
        private ERGameServerRegistrationRequest _registrationRequest;

        /// <summary>
        /// The actual peer connection used to communicate with the game server.
        /// </summary>
        public Peer Peer { get; }

        /// <summary>
        /// The server which the <see cref="Peer"/> is associated to.
        /// </summary>
        public Server Server
        {
            get { return Peer.Server; }
        }

        /// <summary>
        /// The identifier of the game server.
        /// </summary>
        public ulong ServerId
        {
            get { return _registrationRequest.ServerId; }
        }
        /// <summary>
        /// The internal/private IP address of the game server.
        /// </summary>
        public IPAddress InternalAddress
        {
            get { return _registrationRequest.InternalAddress; }
        }
        /// <summary>
        /// The public/external IP address of the game server.
        /// </summary>
        public IPAddress ExternalAddress
        {
            get
            {
                // If the external address registered as a private address, NAT configurations may have redirected request, etc.
                // So we match it with the public IP address we attempt to fetch on server start, and assume anything on the same
                // internal network is accessible through this IP.
                if (Peer.Address.IsPrivate())
                    if(Peer.Server.PublicIPAddress != null)
                        return Peer.Server.PublicIPAddress;

                // The peer address is external, so we return it immediately.
                return Peer.Address;
            }
        }
        /// <summary>
        /// The UDP port that the game server is broadcasting on.
        /// </summary>
        public ushort Port
        {
            get { return _registrationRequest.Port; }
        }
        /// <summary>
        /// A symbol indicating the region of the server.
        /// </summary>
        public long RegionSymbol
        {
            get { return _registrationRequest.RegionSymbol; }
        }
        /// <summary>
        /// The version of the server, prevents mismatches in matching.
        /// </summary>
        public long VersionLock
        {
            get { return _registrationRequest.VersionLock; }
        }

        /// <summary>
        /// The current session identifier (null if a session has not been started).
        /// </summary>
        public Guid? SessionId { get; private set; }
        /// <summary>
        /// The type of lobby (visibility) to clients matching. 
        /// e.g. Public, Private.
        /// </summary>
        public ERGameServerStartSession.LobbyType SessionLobbyType { get; set; }
        /// <summary>
        /// A symbol representing the gametype of the current session.
        /// </summary>
        public long? SessionGameTypeSymbol { get; private set; }
        /// <summary>
        /// A symbol representing the level of the current session.
        /// </summary>
        public long? SessionLevelSymbol { get; private set; }
        /// <summary>
        /// The channel used for filtering this game server from others serving other client channels.
        /// e.g. PLAYGROUND, ECHO COMBAT PLAYERS, etc.
        /// </summary>
        public Guid? SessionChannel { get; private set; }
        /// <summary>
        /// The total amount of players allowed in the session.
        /// Note: One slot may be reserved for the server itself, as the default is "16",
        /// but the game usually only allows 15 players (3 teams x 5 players each).
        /// </summary>
        public GameTypePlayerLimits.PlayerLimits SessionPlayerLimits { get; set; }
        /// <summary>
        /// Indicates whether the server is currently in a locked state.
        /// </summary>
        public bool SessionLocked { get; private set; }
        /// <summary>
        /// Indicates whether a session has been started.
        /// </summary>
        public bool SessionStarted
        {
            get
            {
                return SessionId != null;
            }
        }

        /// <summary>
        /// The current amount of players in the server.
        /// </summary>
        public byte SessionPlayerCount
        {
            get { return (byte)_playerSessions.Count; }
        }

        /// <summary>
        /// Represents the active player sessions in the server.
        /// </summary>
        private Dictionary<Guid, (Peer peer, TeamIndex requestedTeam)> _playerSessions;
      
        /// <summary>
        /// A lock used for asynchronous/awaitable concurrent access to this object.
        /// </summary>
        public AsyncLock _accessLock;
        #endregion

        #region Events
        /// <summary>
        /// Event for players being added to the current <see cref="RegisteredGameServer"/>'s game session.
        /// </summary>
        /// <param name="gameServer">The <see cref="RegisteredGameServer"/> the players were added to.</param>
        /// <param name="players">The player sessions and connections for every player added.</param>
        public delegate void PlayersAddedEventHandler(RegisteredGameServer gameServer, (Guid playerSession, Peer? peer)[] players);
        /// <summary>
        /// Event for players being added to the current <see cref="RegisteredGameServer"/>'s game session.
        /// </summary>
        public event PlayersAddedEventHandler? OnPlayersAdded;

        /// <summary>
        /// Event for a player being removed from the current <see cref="RegisteredGameServer"/>'s game session.
        /// </summary>
        /// <param name="gameServer">The <see cref="RegisteredGameServer"/> the players were removed from.</param>
        /// <param name="players">The player session and connection for the removed player.</param>
        public delegate void PlayerRemovedEventHandler(RegisteredGameServer gameServer, Guid playerSession, Peer? peer);
        /// <summary>
        /// Event for a player being removed from the current <see cref="RegisteredGameServer"/>'s game session.
        /// </summary>
        public event PlayerRemovedEventHandler? OnPlayerRemoved;

        /// <summary>
        /// Event for the <see cref="RegisteredGameServer"/>'s session starting, ending, locking, or unlocking.
        /// This does not fire when players are added or removed.
        /// </summary>
        /// <param name="gameServer">The <see cref="RegisteredGameServer"/> the session state changed for.</param>
        public delegate void SessionStateChanged(RegisteredGameServer gameServer);
        /// <summary>
        /// Event for the <see cref="RegisteredGameServer"/>'s session starting, ending, locking, or unlocking.
        /// This does not fire when players are added or removed.
        /// </summary>
        public event SessionStateChanged? OnSessionStateChanged;
        #endregion

        #region Constructor
        public RegisteredGameServer(GameServerRegistry registry, Peer peer, ERGameServerRegistrationRequest registrationRequest)
        {
            Registry = registry;
            Peer = peer;
            _registrationRequest = registrationRequest;
            SessionLobbyType = ERGameServerStartSession.LobbyType.Unassigned;
            SessionPlayerLimits = GameTypePlayerLimits.DefaultLimits;
            _playerSessions = new Dictionary<Guid, (Peer, TeamIndex)>();
            _accessLock = new AsyncLock();
        }
        #endregion

        #region Functions
        public bool CheckTeamAvailability(TeamIndex requestedTeam)
        {
            // If the session wasn't started, there is availability to join any team.
            if (!SessionStarted)
                return true;

            // Otherwise check availability.
            return SessionPlayerLimits.CheckTeamAvailability(_playerSessions.Values.Select(x => x.requestedTeam).ToArray(), requestedTeam);
        }
        public async Task StartSession(XPlatformId requester, ERGameServerStartSession.LobbyType lobbyType, Guid channel, long? gameTypeSymbol, long? levelSymbol, ERGameServerStartSession.SessionSettings? settings)
        {
            // Lock throughout this method.
            await _accessLock.ExecuteLocked(async () =>
            {
                // Start the session.
                await StartSessionInternal(requester, lobbyType, channel, gameTypeSymbol, levelSymbol, settings);
            });

            // Invoke the session state change event.
            OnSessionStateChanged?.Invoke(this);
        }
        private async Task StartSessionInternal(XPlatformId requester, ERGameServerStartSession.LobbyType lobbyType, Guid channel, long? gameTypeSymbol, long? levelSymbol, ERGameServerStartSession.SessionSettings? settings)
        {
            // Remove the previous session id from the parent registry's lookup.
            if (SessionId != null)
                Registry.RegisteredGameServersBySessionId.Remove(SessionId.Value, out _);

            // Set up our session variables
            SessionId = SecureGuidGenerator.Generate();
            SessionLobbyType = lobbyType;
            SessionChannel = channel;
            SessionGameTypeSymbol = gameTypeSymbol;
            SessionLevelSymbol = levelSymbol;
            SessionPlayerLimits = GameTypePlayerLimits.DefaultLimits;

            _playerSessions.Clear();
            SessionLocked = false;

            // Merge session settings information and send a "start session" message to the game server.
            var mergedSessionSettings = new ERGameServerStartSession.SessionSettings(
                appId: settings?.AppId ?? "1369078409873402",
                gametype: settings?.GameType ?? SessionGameTypeSymbol,
                level: settings?.Level ?? SessionLevelSymbol,
                additionalData: settings?.AdditionalData
            );

            // Set our player limits according to the gametype.
            if (mergedSessionSettings.GameType != null)
                SessionPlayerLimits = GameTypePlayerLimits.GetPlayerLimits(Peer.Server, mergedSessionSettings.GameType.Value);

            // Provide entrant descriptors.
            List<ERGameServerStartSession.EntrantDescriptor> entrantDescriptors = new List<ERGameServerStartSession.EntrantDescriptor>();

            // TODO: This code doesn't currently consistently work, setting 4 AI players will add them, but public arena is still a problem.
            // This could be because the websocket is supposed to send the game additional information, or it's initializing the lobby for this incorrectly.
            // It seems to fail when processing the entrants, and determining factors like players per team, human team, etc.
            if (false)
            {
                // Note: In a local offline game, your local player id is added here.
                // It's unclear if the server's platform id should be added here, or the first entrant requesting the session be created, or none (as a server with no initial entrant).
                // For now, we continue to not add the player's id.
                //entrantDescriptors.Add(new ERGameServerStartSession.EntrantDescriptor(SecureGuidGenerator.Generate(), requester, 0x0144BB8000));

                // If this is an AI match, where one of the gametypes end in "_ai" (e.g. echo_arena_public_ai, echo_arena_private_ai), then we add bot entrants.
                if (mergedSessionSettings.GameType != null)
                {
                    string? gameTypeName = Server.SymbolCache.GetName(mergedSessionSettings.GameType.Value);
                    bool isAIMatch = gameTypeName?.EndsWith("_ai", StringComparison.OrdinalIgnoreCase) ?? false;
                    if (isAIMatch)
                    {
                        // AI matches will have 3 enemies on opposing team.
                        // This is observed in a local offline match. Additional players seem to automatically be added later as a result(?)
                        for (int i = 0; i < 3; i++)
                            entrantDescriptors.Add(new ERGameServerStartSession.EntrantDescriptor()); // default constructor attempts to create a random new AI/bot entrant.
                    }
                }
            }

            // Send the start session message to the game server.
            await Peer.Send(new ERGameServerStartSession(SessionId.Value, SessionChannel.Value, (byte)SessionPlayerLimits.TotalPlayerLimit, SessionLobbyType, mergedSessionSettings, entrantDescriptors.ToArray()));

            // Add the new session id to the parent registry's lookup.
            Registry.RegisteredGameServersBySessionId[SessionId.Value] = this;
        }
        public async Task ProcessLobbySessionRequest(Peer matchingPeer)
        {
            // Obtain the peer's matching session data.
            MatchingSession? matchingSession = matchingPeer.GetSessionData<MatchingSession>();
            if (matchingSession == null)
                return;

            // Set the matched game server to this one.
            matchingSession.MatchedGameServer = this;

            // Lock throughout this method.
            bool newSessionStarted = false;
            await _accessLock.ExecuteLocked(async () =>
            {
                // If the server hasn't started a session, direct it to do so now.
                if (!SessionStarted)
                {
                    // Start a new session on the game server.
                    await StartSessionInternal(
                        matchingSession.UserId,
                        matchingSession.NewSessionLobbyType, 
                        matchingSession.Channel ?? new Guid(),
                        matchingSession.GameTypeSymbol ?? matchingSession.SessionSettings?.GameType,
                        matchingSession.LevelSymbol ?? matchingSession.SessionSettings?.Level, 
                        matchingSession.SessionSettings
                        );

                    newSessionStarted = true;
                }

                // Verify player limits.
                if (!CheckTeamAvailability(matchingSession.TeamIndex))
                {
                    await matchingPeer.Server.MatchingService.SendLobbySessionFailure(matchingPeer, LobbySessionFailureErrorCode.ServerIsFull, "Team is full");
                    return;
                }

                // Set the session id from our game server in our matching session
                matchingSession.MatchedSessionId = SessionId;

                // Create our packet encoding settings
                PacketEncoderSettings serverEncoderSettings = new PacketEncoderSettings(
                        encryptionEnabled: true,
                        macEnabled: true,
                        macDigestSize: 32,
                        macPBKDF2IterationCounter: 0,
                        macKeySize: 32,
                        encryptionKeySize: 32,
                        randomKeySize: 32
                    );
                PacketEncoderSettings clientEncoderSettings = new PacketEncoderSettings(
                        encryptionEnabled: true,
                        macEnabled: true,
                        macDigestSize: 64,
                        macPBKDF2IterationCounter: 0,
                        macKeySize: 32,
                        encryptionKeySize: 32,
                        randomKeySize: 32
                    );

                // Create our success messages with our game server and client packet encryption parameters.
                // These are typical parameters you'd see configured in normal gameplay.
                LobbySessionSuccessv5 sessionSuccessv5 = new LobbySessionSuccessv5(
                    gameTypeSymbol: SessionGameTypeSymbol ?? -1,
                    matchingSession: matchingSession.MatchedSessionId!.Value,
                    channelUUID: SessionChannel ?? new Guid(),
                    endpoint: new LobbyPingRequestv3.EndpointData(InternalAddress, ExternalAddress, Port),
                    teamIndex: (short)matchingSession.TeamIndex,
                    unk1: 0,
                    serverEncoderFlags: (ulong)serverEncoderSettings,
                    clientEncoderFlags: (ulong)clientEncoderSettings,
                    serverSequenceId: BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)),
                    serverMacKey: RandomNumberGenerator.GetBytes(serverEncoderSettings.MacKeySize),
                    serverEncKey: RandomNumberGenerator.GetBytes(serverEncoderSettings.EncryptionKeySize),
                    serverRandomKey: RandomNumberGenerator.GetBytes(serverEncoderSettings.RandomKeySize),
                    clientSequenceId: BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)),
                    clientMacKey: RandomNumberGenerator.GetBytes(clientEncoderSettings.MacKeySize),
                    clientEncKey: RandomNumberGenerator.GetBytes(clientEncoderSettings.EncryptionKeySize),
                    clientRandomKey: RandomNumberGenerator.GetBytes(clientEncoderSettings.RandomKeySize)
                );
                LobbySessionSuccessv4 sessionSuccessv4 = new LobbySessionSuccessv4(
                    sessionSuccessv5.GameTypeSymbol,
                    sessionSuccessv5.MatchingSession,
                    sessionSuccessv5.Endpoint,
                    sessionSuccessv5.TeamIndex,
                    sessionSuccessv5.Unk1,
                    sessionSuccessv5.ServerEncoderFlags,
                    sessionSuccessv5.ClientEncoderFlags,
                    sessionSuccessv5.ServerSequenceId,
                    sessionSuccessv5.ServerMacKey,
                    sessionSuccessv5.ServerEncKey,
                    sessionSuccessv5.ServerRandomKey,
                    sessionSuccessv5.ClientSequenceId,
                    sessionSuccessv5.ClientMacKey,
                    sessionSuccessv5.ClientEncKey,
                    sessionSuccessv5.ClientRandomKey
                );

                // Send the success messages to the server (so it knows to expect a new connection with these packet encoder settings).
                await Peer.Send(sessionSuccessv4);
                await Peer.Send(sessionSuccessv5);

                // Send the success messages to the peer (so it can connect).
                await matchingPeer.Send(sessionSuccessv4);
                await matchingPeer.Send(sessionSuccessv5);
            });

            // If a new session was started, fire the relevant event handlers.
            if(newSessionStarted)
                OnSessionStateChanged?.Invoke(this);
        }
 
        public async Task ProcessPlayerSessionRequest(Peer matchingPeer, XPlatformId userId, Guid channel)
        {
            // Lock throughout this method.
            await _accessLock.ExecuteLocked(async () =>
            {
                // Obtain the peer's matching session data and ensure they have a session id set.
                MatchingSession? matchingSession = matchingPeer.GetSessionData<MatchingSession>();
                if (matchingSession == null || matchingSession.MatchedSessionId == null)
                    return;

                // Use a cryptographically secure RNG to generate the player sessions.
                Guid[] playerSessions = new Guid[] { SecureGuidGenerator.Generate() };

                // Try to send the player sessions to the server and add them to our pending lookup if it succeeds without exception.
                try
                {
                    // Check the player count and decide whether to accept or deny the player session.
                    if (playerSessions.Length + SessionPlayerCount > SessionPlayerLimits.TotalPlayerLimit)
                    {
                        // Inform the game server we rejected player sessions.
                        await Peer.Send(new ERGameServerPlayersRejected(ERGameServerPlayersRejected.PlayerSessionError.LobbyFull, playerSessions));

                        // TODO: Send player sessions failure to the client.
                    }
                    else
                    {
                        // Send the player sessions to the player.
                        await matchingPeer.Send(new LobbyPlayerSessionsSuccessUnk1(matchingSession.MatchedSessionId.Value, playerSessions));
                        await matchingPeer.Send(new LobbyPlayerSessionsSuccessv2(0xFF, matchingSession.UserId, playerSessions[0]));
                        await matchingPeer.Send(new LobbyPlayerSessionsSuccessv3(0xFF, matchingSession.UserId, playerSessions[0], (short)matchingSession.TeamIndex, 0, 0));

                        // Add the pending player session associated to this peer.
                        _playerSessions[playerSessions[0]] = (matchingPeer, matchingSession.TeamIndex);
                    }

                }
                catch { }
            });
        }

        /// <summary>
        /// Sets the locked status on the game server, controlling whether new players can join or not.
        /// </summary>
        /// <param name="locked">The locked status to set for the lobby/session.</param>
        public void SetLockedStatus(bool locked)
        {
            // Determine if the locked status will change
            bool changed = SessionLocked != locked;

            // Set the locked status
            SessionLocked = locked;

            // Fire the relevant event handler.
            if (changed)
                OnSessionStateChanged?.Invoke(this);
        }

        public async Task<Peer?> GetPeer(Guid playerSession)
        {
            // Lock throughout this method and grab the peer for this session.
            Peer? peer = null;
            await _accessLock.ExecuteLocked(() =>
            {
                if (_playerSessions.TryGetValue(playerSession, out var playerInfo))
                    peer = playerInfo.peer;
                return Task.CompletedTask;
            });
            return peer;
        }

        public async Task<(Guid PlayerSession, Peer? Peer)[]> GetPlayers()
        {
            // Lock throughout this method and grab the player sessions.
            var playersInfo = Array.Empty<(Guid playerSession, Peer? peer)>();
            await _accessLock.ExecuteLocked(() =>
            {
                playersInfo = _playerSessions.AsEnumerable().Select(x => (x.Key, (Peer?)x.Value.peer)).ToArray();
                return Task.CompletedTask;
            });
            return playersInfo;
        }

        public async Task AddPlayers(Guid[] playerSessions)
        {
            // Lock throughout this method.
            (Guid playerSession, Peer? peer)[] addedPlayersInfo = Array.Empty<(Guid, Peer?)>();
            await _accessLock.ExecuteLocked(async () =>
            {
                // If there is no active session, we will reject.
                if (!SessionStarted)
                    return;

                // Signal for the game server to accept the players.
                await Peer.Send(new ERGameServerPlayersAccepted(playerSessions));

                // Obtain every added player session and the associated peer.
                addedPlayersInfo = new (Guid playerSession, Peer? peer)[playerSessions.Length];

                // Build the event data for all players added.
                for (int i = 0; i < addedPlayersInfo.Length; i++)
                {
                    var playerSession = playerSessions[i];
                    if (_playerSessions.TryGetValue(playerSession, out var playerInfo))
                        addedPlayersInfo[i] = (playerSessions[i], playerInfo.peer);
                }
            });

            // Fire the event for players being added
            if(addedPlayersInfo.Length > 0)
                OnPlayersAdded?.Invoke(this, addedPlayersInfo);
        }

        public async Task KickPlayer(Guid playerSession)
        {
            // Inform the game server we rejected player sessions.
            await Peer.Send(new ERGameServerPlayersRejected(ERGameServerPlayersRejected.PlayerSessionError.KickedFromServer, new Guid[] { playerSession }));
        }

        public async Task RemovePlayer(Guid playerSession)
        {
            // Define the peer associated with this player session, which we will attempt to obtain to fire the removed event later.
            Peer? peer = null;

            // Lock throughout this method.
            await _accessLock.ExecuteLocked(() =>
            {
                // Try to get the existing peer for this player session.
                if(_playerSessions.TryGetValue(playerSession, out var playerInfo))
                    peer = playerInfo.peer;

                // Remove this player session from our lookup if it exists.
                _playerSessions.Remove(playerSession);

                // If we hit 0 players, expect end of session, set server as not ready to match.
                if (SessionStarted && SessionPlayerCount == 0)
                    SessionLocked = true;

                return Task.CompletedTask;
            });

            // Fire the event for a player being removed
            OnPlayerRemoved?.Invoke(this, playerSession, peer);
        }

        public async Task EndSession()
        {
            // Lock throughout this method.
            await _accessLock.ExecuteLocked(() => {
                // Reset all variables
                SessionId = null;
                SessionLobbyType = ERGameServerStartSession.LobbyType.Unassigned;
                SessionChannel = null;
                SessionGameTypeSymbol = null;
                SessionLevelSymbol = null;
                SessionLocked = false;
                SessionPlayerLimits = GameTypePlayerLimits.DefaultLimits;
                _playerSessions.Clear();

                return Task.CompletedTask;
            });

            // Fire the event for the session ending.
            OnSessionStateChanged?.Invoke(this);
        }
        #endregion
    }
}
