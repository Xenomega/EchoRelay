#pragma once

#include "echovr.h"


// Symbols representing messages to the broadcaster

const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_REGISTRATION_SUCCESS = 0xFEF8EFEC97A3B98;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_REGISTRATION_FAILURE = 0xCC3A40870CDBC852;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_SESSION_STARTING = 0x233E6E7E3A13BABC;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_SESSION_ERROR = 0x425393736F0CDB8B;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_TERMINATE_PROCESS = 0xF0FA52B6F8A33A49;

const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_START_SESSION_V4 = 0x96101C684E7F325;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_JOIN_REQUESTED_V4 = 0xB4D724E8564BFE88;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_ADD_ENTRANT_REQUEST_V4 = 0x5A44AB3E283D136D;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_SESSION_SUCCESS_V5 = 0x83E96504A9FC81C6;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_ACCEPT_PLAYERS_SUCCESS_V2 = 0xFCBA8F2834F8DE40;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_ACCEPT_PLAYERS_FAILURE_V2 = 0xED9A4B86F8F3640A;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_SMITE_ENTRANT = 0xCCBC52F97F2E0EF1;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_CHAT_ENTRY = 0xDCB7130D1BEB9AC4;
const EchoVR::SymbolId SYMBOL_BROADCASTER_LOBBY_VOICE_ENTRY = 0x27504F14881C1A43;

// Symbols representing messages to the serverdb.

const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_REQUEST = 0x7777777777777777; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_SUCCESS = -5369924845641990433;
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_REGISTRATION_FAILURE = -5373034290044534839;
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_SESSION_SUCCESS_V5 = 0x6d4de3650ee3110e;
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_START_SESSION = 0x7777777777770000; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_SESSION_STARTED = 0x7777777777770100;  // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_END_SESSION = 0x7777777777770200; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_PLAYER_SESSIONS_LOCKED = 0x7777777777770300; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_PLAYER_SESSIONS_UNLOCKED = 0x7777777777770400; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_ACCEPT_PLAYERS = 0x7777777777770500; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_ACCEPTED = 0x7777777777770600; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_REJECTED = 0x7777777777770700; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_PLAYERS_REMOVE_PLAYER = 0x7777777777770800; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_CHALLENGE_REQUEST = 0x7777777777770900; // unofficial
const EchoVR::SymbolId SYMBOL_TCPBROADCASTER_LOBBY_CHALLENGE_RESPONSE = 0x7777777777770A00; // unofficial

/// <summary>
/// A message sent from game server to server to register the game server.
/// </summary>
struct ERLobbyRegistrationRequest
{
	UINT64 serverId;
	UINT32 internalIp;
	UINT16 port;
	BYTE padding[4];
	EchoVR::SymbolId regionId;
	EchoVR::SymbolId versionLock;
};

/// <summary>
/// A message sent from game server to server to indicate the current session has ended.
/// </summary>
struct ERLobbyEndSession {
	CHAR unused;
};

/// <summary>
/// A message sent from game server to server to indicate the current session has been locked.
/// </summary>
struct ERLobbyPlayerSessionsLocked {
	CHAR unused;
};

/// <summary>
/// A message sent from game server to server to indicate the current session has been unlocked.
/// </summary>
struct ERLobbyPlayerSessionsUnlocked {
	CHAR unused;
};
