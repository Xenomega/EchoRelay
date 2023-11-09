#include <cstdio>
#include "pch.h"
#include "echovr.h"
#include "echovrunexported.h"
#include "messages.h"
#include "gameserver.h"

/// <summary>
/// A wrapper for WriteLog, simplifying logging operations.
/// </summary>
/// <returns>None</returns>
VOID Log(EchoVR::LogLevel level, const CHAR* format, ...) {
	va_list args;
	va_start(args, format);
	EchoVR::WriteLog(level, 0, format, args);
	va_end(args);
}

/// <summary>
/// Subscribes to internal local events for a given message type. These are typically sent internally by the game
/// to its self, or derived from connected peer's messages (UDP broadcast port forwards events). The provided
/// function is used as a callback when a message of the type is received from any peer or onesself.
/// </summary>
/// <param name="self">The game server library which is listening for the message.</param>
/// <param name="msgId">The 64-bit symbol used to describe a message type/identifier to listen for.</param>
/// <param name="isMsgReliable">Indicates whether we are listening for events for messages sent over the reliable or mailbox game server message inbox types.</param>
/// <param name="func">The function to use as callback when a broadcaster message of the given type is received.</param>
/// <returns>An identifier/handle for the callback registration, to be later used in unregistering.</returns>
UINT16 ListenForBroadcasterMessage(GameServerLib* self, EchoVR::SymbolId msgId, BOOL isMsgReliable, VOID* func)
{
	// Subscribe to the provided message id.
	EchoVR::DelegateProxy listenerProxy;
	memset(&listenerProxy, 0, sizeof(listenerProxy));
	listenerProxy.method[0] = 0xFFFFFFFFFFFFFFFF;
	listenerProxy.instance = (VOID*)self;
	listenerProxy.proxyFunc = func;

	return EchoVR::BroadcasterListen(self->lobby->broadcaster, msgId, isMsgReliable, (VOID*)&listenerProxy, true);
}

/// <summary>
/// Subscribes the TCP broadcasters (websocket) to a given message type. The provided
/// function is used as a callback when a message of the type is received from any service.
/// </summary>
/// <param name="self">The game server library which is listening for the message.</param>
/// <param name="msgId">The 64-bit symbol used to describe a message type/identifier to listen for.</param>
/// <param name="func">The function to use as callback when a TCP/webosocket message of the given type is received.</param>
/// <returns>None</returns>
UINT16 ListenForTcpBroadcasterMessage(GameServerLib* self, EchoVR::SymbolId msgId, VOID* func)
{
	// Subscribe to the provided message id.
	// Normally a proxy function is provided, which calls the underlying method provided, but we just use the proxy function callback to receive everything to keep it simple.
	EchoVR::DelegateProxy listenerProxy;
	memset(&listenerProxy, 0, sizeof(listenerProxy));
	listenerProxy.method[0] = 0xFFFFFFFFFFFFFFFF;
	listenerProxy.instance = (VOID*)self;
	listenerProxy.proxyFunc = func;

	return EchoVR::TcpBroadcasterListen(self->lobby->tcpBroadcaster, msgId, 0, 0, 0, (VOID*)&listenerProxy, true);
}

/// <summary>
/// Sends a message using the TCP broadcaster to the ServerDB websocket service.
/// </summary>
/// <param name="self">The game server library which is sending the message to the service.</param>
/// <param name="msgId">The 64-bit symbol used to describe the message type/identifier being sent.</param>
/// <param name="msg">A pointer to the message data to be sent.</param>
/// <param name="msgSize">The size of the msg to be sent, in bytes.</param>
/// <returns>None</returns>
VOID SendServerdbTcpMessage(GameServerLib* self, EchoVR::SymbolId msgId, VOID* msg, UINT64 msgSize)
{
	// Wrap the send call provided by the TCP broadcaster.
	self->tcpBroadcasterData->SendToPeer(self->serverDbPeer, msgId, NULL, 0, msg, msgSize);
}

/// <summary>
/// Event handler for receiving a game server registration success message from the TCP (websocket) ServerDB service.
/// This message indicates the game server registration with ServerDB was accepted.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMsgRegistrationSuccess(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Set the registration status
	self->registered = TRUE;

	// Forward the received registration success event to the internal broadcast.
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_REGISTRATION_SUCCESS, "SNSLobbyRegistrationSuccess", msg, msgSize);
}

/// <summary>
/// Event handler for receiving a game server registration failure message from the TCP (websocket) ServerDB service.
/// This message indicates the game server registration with ServerDB was rejected.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMsgRegistrationFailure(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Set the registration status
	self->registered = FALSE;

	// Forward the received registration failure event to the internal broadcast.
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_REGISTRATION_FAILURE, "SNSLobbyRegistrationFailure", msg, msgSize);
}

/// <summary>
/// Event handler for receiving a start session request from the TCP (websocket) ServerDB service.
/// This message directs the game to start loading a new game session with the provided request arguments.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMessageStartSession(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Set our session to active.
	self->sessionActive = TRUE;

	// Forward the received start session event to the internal broadcast.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Starting new session");
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_START_SESSION_V4, "SNSLobbyStartSessionv4", msg, msgSize);
}

/// <summary>
/// Event handler for receiving a players accepted message from the TCP (websocket) ServerDB service.
/// This message indicates that ServerDB / Matching services accepted a player into this session, and is now
/// requesting the game server accept them.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMsgPlayersAccepted(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Forward the received player acceptance success event to the internal broadcast.
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_ACCEPT_PLAYERS_SUCCESS_V2, "SNSLobbyAcceptPlayersSuccessv2", msg, msgSize);
}

/// <summary>
/// Event handler for receiving a players rejected message from the TCP (websocket) ServerDB service.
/// This message indicates that ServerDB / Matching services rejected a player from this session, and is now
/// requesting the game server kick / reject them.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMsgPlayersRejected(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Forward the received player acceptance failure event to the internal broadcast.
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_ACCEPT_PLAYERS_FAILURE_V2, "SNSLobbyAcceptPlayersFailurev2", msg, msgSize);
}

/// <summary>
/// Event handler for receiving a join session success message from the TCP (websocket) ServerDB service.
/// This message indicates that ServerDB / Matching matched a player to this server.The message provides
/// connection parameters for both parties, including encryption / verification keys for client / game server to use.
/// </summary>
/// <returns>None</returns>
VOID OnTcpMsgSessionSuccessv5(GameServerLib* self, VOID* proxymthd, EchoVR::TcpPeer sender, VOID* msg, VOID* unk, UINT64 msgSize)
{
	// Forward the received join session success event to the internal broadcast.
	// NOTE: For some reason, currently the session success message for servers parses differently than clients by some offset when setting packet encoding settings.
	// To account for this, we shift the message pointer, and its size. This is non-problematic for the delegate proxy method wrapper, which only validates minimum size.
	EchoVR::BroadcasterReceiveLocalEvent(self->broadcaster, SYMBOL_BROADCASTER_LOBBY_SESSION_SUCCESS_V5, "SNSLobbySessionSuccessv5", (CHAR*)msg - 0x10, msgSize + 0x10);
}

/// <summary>
/// Event handler for receiving a session start success message from events (internal game server broadcast).
/// This message indicates that a new session is starting.This is triggered after a SNSLobbyStartSessionv4 message
/// is processed.
/// </summary>
/// <returns>None</returns>
VOID OnMsgSessionStarting(GameServerLib* self, VOID* proxymthd, VOID* msg, UINT64 msgSize, EchoVR::Peer destination, EchoVR::Peer sender)
{
	// NOTE: `msg` here has no substance (one uninitialized byte).
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Session starting");
}

/// <summary>
/// Event handler for receiving a session error message from events (internal game server broadcast).
/// This message indicates that the game session encountered an error either when starting or running.
/// </summary>
/// <returns>None</returns>
VOID OnMsgSessionError(GameServerLib* self, VOID* proxymthd, VOID* msg, UINT64 msgSize, EchoVR::Peer destination, EchoVR::Peer sender)
{
	// NOTE: `msg` here has no substance (one uninitialized byte).
	Log(EchoVR::LogLevel::Error, "[ECHORELAY.GAMESERVER] Session error encountered");
}

/// <summary>
/// TODO: This vtable slot is not verified to be for this purpose.
/// In any case, it seems not to be called or problematic, so we'll leave this definition as a placeholder.
/// </summary>
/// <param name="unk1">TODO: Unknown</param>
/// <param name="a2">TODO: Unknown</param>
/// <param name="a3">TODO: Unknown</param>
/// <returns>TODO: Unknown</returns>
INT64 GameServerLib::UnkFunc0(VOID* unk1, INT64 a2, INT64 a3)
{
	return 1;
}

/// <summary>
/// Initializes the game server library. This is called by the game after the library has been loaded.
/// </summary>
/// <param name="lobby">The current game lobby structure to reference/leverage when operating the game server.</param>
/// <param name="broadcaster">The internal game server broadcast to use to communicate with clients.</param>
/// <param name="unk2">TODO: Unknown.</param>
/// <param name="logPath">The file path where the current log file resides.</param>
/// <returns>None</returns>
VOID* GameServerLib::Initialize(EchoVR::Lobby* lobby, EchoVR::Broadcaster* broadcaster, VOID* unk2, const CHAR* logPath)
{
	// Set up our game server state.
	this->lobby = lobby;
	this->broadcaster = broadcaster;
	this->tcpBroadcasterData = lobby->tcpBroadcaster->data;

	// Subscribe to broadcaster events
	this->broadcastSessionStartCBHandle = ListenForBroadcasterMessage(this, SYMBOL_BROADCASTER_LOBBY_SESSION_STARTING, TRUE, (VOID*)OnMsgSessionStarting);
	this->broadcastSessionErrorCBHandle = ListenForBroadcasterMessage(this, SYMBOL_BROADCASTER_LOBBY_SESSION_ERROR, TRUE, (VOID*)OnMsgSessionError);

	// Subscribe to websocket events.
	this->tcpBroadcastRegSuccessCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_SUCCESS, (VOID*)OnTcpMsgRegistrationSuccess);
	this->tcpBroadcastRegFailureCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_FAILURE, (VOID*)OnTcpMsgRegistrationFailure);
	this->tcpBroadcastStartSessionCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_START_SESSION, (VOID*)OnTcpMessageStartSession);
	this->tcpBroadcastPlayersAcceptedCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_ACCEPTED, (VOID*)OnTcpMsgPlayersAccepted);
	this->tcpBroadcastPlayersRejectedCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_REJECTED, (VOID*)OnTcpMsgPlayersRejected);
	this->tcpBroadcastSessionSuccessCBHandle = ListenForTcpBroadcasterMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_SESSION_SUCCESS_V5, (VOID*)OnTcpMsgSessionSuccessv5);

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Initialized game server");
	//lobby->hosting |= 0x1;

	// If we built the module in debug mode, print the base address into logs for debugging purposes.
	#if _DEBUG
	Log(EchoVR::LogLevel::Debug, "[ECHORELAY.GAMESERVER] EchoVR base address = 0x%p", (VOID*)EchoVR::g_GameBaseAddress);
	#endif

	// This should return a valid pointer to simply dereference.
	return this;
}

/// <summary>
/// Terminates the game server library. This is called by the game prior to unloading the library.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::Terminate() 
{
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Terminated game server");
}

/// <summary>
/// Updates the game server library. This is called by the game at a frequent interval.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::Update()
{
	// TODO: This is temporary code to test if the profile JSON is updated (but not sent to server).
	// If it is not updated in this structure, one of the "apply loadout" or "save loadout" operations may trigger the update?
	for (int i = 0; i < this->lobby->entrantData.count; i++)
	{
		// Obtain the entrant at the given index.
		EchoVR::Lobby::EntrantData* entrantData = (this->lobby->entrantData.items + i);

		// TODO: If the entrant is marked dirty...
		if (entrantData->userId.accountId != 0 && entrantData->dirty)
		{
		
		}
	}
}

/// <summary>
/// TODO: Unknown. This is called during initialization with a value of 6. Maybe it is platform/game server privilege/role related?
/// </summary>
/// <param name="unk">TODO: Unknown.</param>
/// <returns>None</returns>
VOID GameServerLib::UnkFunc1(UINT64 unk)
{
	// Note: This function is called prior to Initialize.
}

/// <summary>
/// Requests registration of the game server with central TCP/websocket services (ServerDB). 
/// This is called by the game after the library has been initialized.
/// </summary>
/// <param name="serverId">The identifier to use for the game server when registering with ServerDB.</param>
/// <param name="radId">TODO: Unknown.</param>
/// <param name="regionId">A 64-bit symbol identifier indicating the region that the game server should be registering to.</param>
/// <param name="versionLock">A version to use when locking out clients which request matching with a differing version.</param>
/// <param name="localConfig">The game config containing service endpoints (located in ./_local/config.json from the root of game folder).</param>
/// <returns>None</returns>
VOID GameServerLib::RequestRegistration(INT64 serverId, CHAR* radId, EchoVR::SymbolId regionId, EchoVR::SymbolId versionLock, const EchoVR::Json* localConfig)
{
	// Store the registration information.
	this->serverId = serverId;
	this->regionId = regionId;
	this->versionLock = versionLock;

	// Obtain the serverdb URI from our config (or fallback to default)
	CHAR* serverDbServiceUri = EchoVR::JsonValueAsString((EchoVR::Json*)localConfig, (CHAR*)"serverdb_host", (CHAR*)"ws://localhost:777/serverdb", false);
	EchoVR::UriContainer serverDbUriContainer;
	memset(&serverDbUriContainer, 0, sizeof(serverDbUriContainer));
	if (EchoVR::UriContainerParse(&serverDbUriContainer, serverDbServiceUri) != ERROR_SUCCESS)
	{
		Log(EchoVR::LogLevel::Error, "[ECHORELAY.GAMESERVER] Failed to register game server: error parsing serverdb service URI");
		return;
	}

	// Connect to the serverdb websocket service
	this->tcpBroadcasterData->CreatePeer(&this->serverDbPeer, (const EchoVR::UriContainer*)&serverDbUriContainer);

	// Obtain address information about our game server broadcaster
	sockaddr_in gameServerAddr = (*(sockaddr_in*)&this->broadcaster->data->addr);

	// Create a registration request.
	// Note: Only IP address is in network order (big endian).
	ERLobbyRegistrationRequest regRequest;
	regRequest.serverId = this->serverId;
	regRequest.port = (UINT16)this->broadcaster->data->broadcastSocketInfo.port;
	regRequest.internalIp = gameServerAddr.sin_addr.S_un.S_addr;
	regRequest.regionId = this->regionId;
	regRequest.versionLock = this->versionLock;

	// Send the registration request.
	SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_REQUEST, &regRequest, sizeof(regRequest));

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Requested game server registration");
}

/// <summary>
/// Requests unregistration of the game server with central TCP/websocket services (ServerDB). 
/// This is called by the game during game server library unloading.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::Unregister() {
	// Reset our game server library state.
	registered = FALSE;
	sessionActive = FALSE;
	serverId = -1;
	regionId = -1;
	versionLock = -1;

	// TODO: These probably aren't necessary, but it would be good to..
	// - Set lobbytype to public
	// - Clear the JSON for lobby

	// Unregister our broadcaster message listeners
	EchoVR::BroadcasterUnlisten(this->broadcaster, this->broadcastSessionStartCBHandle);
	EchoVR::BroadcasterUnlisten(this->broadcaster, this->broadcastSessionErrorCBHandle);

	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastRegSuccessCBHandle);
	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastRegFailureCBHandle);
	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastStartSessionCBHandle);
	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastPlayersAcceptedCBHandle);
	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastPlayersRejectedCBHandle);
	EchoVR::TcpBroadcasterUnlisten(this->lobby->tcpBroadcaster, this->tcpBroadcastSessionSuccessCBHandle);

	// Disconnect from server db.
	this->tcpBroadcasterData->DestroyPeer(this->serverDbPeer);

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Unregistered game server");
}

/// <summary>
/// Signals the ending of the current game server session with central TCP/websocket services (ServerDB).
/// This is called by the game when a game has ended, or a session was started but no players joined for some time,
/// causing the game server to load back to the mainmenu, awaiting further orders from ServerDB.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::EndSession() {
	// If there is a running session, inform the websocket so it can track the state change.
	if (sessionActive)
	{
		ERLobbyEndSession message;
		SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_END_SESSION, &message, sizeof(message));
	}
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Signaling end of session");
}

/// <summary>
/// Signals the locking of player sessions in the current game server session with central TCP/websocket services (ServerDB),
/// indicating players should no longer be able to join the current game session.
/// This is called by the game after a game has been started by some time, to avoid players from joining during
/// the later halves of game sessions.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::LockPlayerSessions() {
	// If there is a running session, inform the websocket so it can track the state change.
	if (sessionActive)
	{
		ERLobbyPlayerSessionsLocked message;
		SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_PLAYER_SESSIONS_LOCKED, &message, sizeof(message));
	}

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Signaling game server locked");
}

/// <summary>
/// Signals the unlocking of player sessions in the current game server session with central TCP/websocket services (ServerDB),
/// indicating players should be able to join the current game session.
/// </summary>
/// <returns>None</returns>
VOID GameServerLib::UnlockPlayerSessions() {
	// If there is a running session, inform the websocket so it can track the state change.
	if (sessionActive)
	{
		ERLobbyPlayerSessionsUnlocked message;
		SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_PLAYER_SESSIONS_UNLOCKED, &message, sizeof(message));
	}

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Signaling game server unlocked");
}

/// <summary>
/// Signals acceptance of player sessions by the game server, with central TCP/websocket services (ServerDB).
/// This is called by the game after a peer connects to the game server, typically after ServerDB signals its
/// acceptance to game server, and gives peer the appropriate information to join.Both parties must have
/// exchanged packet encoding settings via receipt of SNSLobbySessionSuccessv5 from ServerDB / Matching to communicate.
/// </summary>
/// <param name="playerUuids">An array of player session UUIDs which have been accepted by the game server. </param>
/// <returns>None</returns>
VOID GameServerLib::AcceptPlayerSessions(EchoVR::Array<GUID>* playerUuids) {
	// If we have an active session, signal to serverdb that we are accepting the provided player UUIDs.
	if (sessionActive)
	{
		SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_ACCEPT_PLAYERS, playerUuids->items, playerUuids->count * sizeof(GUID));
	}
	else
	{
		// TODO: Receive local event "SNSLobbyAcceptPlayersFailurev2"
	}

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Accepted %d players into game server", playerUuids->count);
}

/// <summary>
/// Signals rejection of player sessions by the game server, with central TCP/websocket services (ServerDB).
/// This is called by the game after a peer connects to the game server, but the game server did not accept them,
/// possibly due to inability to communicate(e.g.invalid packet encoder settings for one party), or due to
/// general communication / peer state errors.
/// </summary>
/// <param name="playerUuid">A single player session UUID which have been removed by the game server.</param>
/// <returns>None</returns>
VOID GameServerLib::RemovePlayerSession(GUID* playerUuid) {
	// If we have an active session, signal to serverdb that we are removing the provided player UUID.
	if (sessionActive)
	{
		SendServerdbTcpMessage(this, SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_REMOVE_PLAYER, (VOID*)playerUuid, sizeof(GUID));
	}

	// Log the interaction.
	Log(EchoVR::LogLevel::Info, "[ECHORELAY.GAMESERVER] Removed a player from game server");
}
