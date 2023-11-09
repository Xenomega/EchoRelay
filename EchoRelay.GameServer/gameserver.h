#pragma once

#include "pch.h"
#include "echovr.h"

/// <summary>
/// A symbol representing the game server's special websocket service.
/// </summary>
const EchoVR::SymbolId SYMBOL_GAMESERVER_DB = 0x25E886012CED8064;

/// <summary>
/// A game server library implementation which connects to EchoRelay's ServerDB implementation.
/// </summary>
class GameServerLib : public EchoVR::IServerLib {
public:
	INT64 UnkFunc0(VOID* unk1, INT64 a2, INT64 a3);
	VOID* Initialize(EchoVR::Lobby* lobby, EchoVR::Broadcaster* broadcaster, VOID* unk2, const CHAR* logPath);
	VOID Terminate();
	VOID Update();
	VOID UnkFunc1(UINT64 unk);


	VOID RequestRegistration(INT64 serverId, CHAR* radId, EchoVR::SymbolId regionId, EchoVR::SymbolId lockedVersion, const EchoVR::Json* localConfig);
	VOID Unregister();
	VOID EndSession();
	VOID LockPlayerSessions();
	VOID UnlockPlayerSessions();
	VOID AcceptPlayerSessions(EchoVR::Array<GUID>* playerUuids);
	VOID RemovePlayerSession(GUID* playerUuid);

	// Game related fields

	EchoVR::Lobby* lobby;
	EchoVR::Broadcaster* broadcaster;
	EchoVR::TcpBroadcasterData* tcpBroadcasterData;


	// ServerDB related fields

	EchoVR::TcpPeer serverDbPeer;
	BOOL registered;


	// Session related fields.

	BOOL sessionActive;
	UINT64 serverId;
	EchoVR::SymbolId regionId;
	EchoVR::SymbolId versionLock;


	// Callbacks

	UINT16 broadcastSessionStartCBHandle;
	UINT16 broadcastSessionErrorCBHandle;

	UINT16 tcpBroadcastRegSuccessCBHandle;
	UINT16 tcpBroadcastRegFailureCBHandle;
	UINT16 tcpBroadcastStartSessionCBHandle;
	UINT16 tcpBroadcastPlayersAcceptedCBHandle; 
	UINT16 tcpBroadcastPlayersRejectedCBHandle;
	UINT16 tcpBroadcastSessionSuccessCBHandle;
};
