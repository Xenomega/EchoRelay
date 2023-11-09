#pragma once

#include "pch.h"
#include "echovr.h"

namespace EchoVR
{
	// Obtain a handle for the game
	CHAR* g_GameBaseAddress = (CHAR*)GetModuleHandle(NULL);

	/// <summary>
	/// Obtains a pool item/block/memory page from a given pool for the given index.
	/// </summary>
	typedef BYTE* PoolFindItemFunc(PVOID pool, UINT64 index);
	PoolFindItemFunc* PoolFindItem = (PoolFindItemFunc*)(g_GameBaseAddress + 0x2CA9E0);

	/// <summary>
	/// Registers a callback for a certain type of websocket message.
	/// </summary>
	/// <returns>An identifier for the callback registration, to be used for unregistering.</returns>
	typedef UINT16 TcpBroadcasterListenFunc(
		EchoVR::TcpBroadcaster* broadcaster,
		EchoVR::SymbolId messageId,
		INT64 unk1,
		INT64 unk2,
		INT64 unk3,
		VOID* delegateProxy,
		BOOL prepend
	);
	TcpBroadcasterListenFunc* TcpBroadcasterListen = (TcpBroadcasterListenFunc*)(g_GameBaseAddress + 0xF81100);

	/// <summary>
	/// Unregisters a callback for a certain type of websocket message, using the return value from its registration.
	/// </summary>
	/// <returns>None</returns>
	UINT64 TcpBroadcasterUnlisten(
		EchoVR::TcpBroadcaster* broadcaster,
		UINT16 cbResult
	) 
	{
		// Obtain the listeners pool from the broadcaster structure.
		BYTE* listeners = (BYTE*)broadcaster->data + 352;

		// Obtain our block index and offset within it.
		UINT64 blockCapacity = *(UINT64*)(listeners + 40);
		UINT64 blockIndex = cbResult / blockCapacity;
		UINT64 indexInBlock = cbResult % blockCapacity;

		// Obtain the item from the pool
		UINT64 itemPage = NULL;
		if (blockIndex)
		{
			itemPage = *(UINT64*)(PoolFindItem(listeners, (blockIndex - 1) >> 1) + 8 * (blockIndex & 1));
		}
		else
		{
			itemPage = *(UINT64*)(listeners + 8);
		}

		// Free the item from the page at its given offset.
		UINT64 itemData = itemPage + 16;
		UINT64 result = itemData + (UINT32)(-((INT32)itemData) & 7);
		UINT32* flags = (UINT32*)(result + (indexInBlock * 80) + 12);
		*flags |= 1u; // mark the page as free
		return result;
	}

	/// <summary>
	/// Sends a message to a game server broadcaster.
	/// </summary>
	/// <returns>TODO: Unverified, probably success result or size.</returns>
	typedef INT32 BroadcasterSendFunc(
		EchoVR::Broadcaster* broadcaster,
		EchoVR::SymbolId messageId,
		INT32 mbThreadPriority, // note: most use 0
		VOID* item,
		UINT64 size,
		VOID* buffer,
		UINT64 bufferLen,
		EchoVR::Peer peer,
		UINT64 dest,
		FLOAT priority,
		EchoVR::SymbolId unk
	);
	BroadcasterSendFunc* BroadcasterSend = (BroadcasterSendFunc*)(g_GameBaseAddress + 0xF89AF0);

	/// <summary>
	/// Receives/relays a local event on the broadcaster, triggering a listener.
	/// </summary>
	/// <returns>TODO: Unverified, probably success result.</returns>
	typedef UINT64 BroadcasterReceiveLocalEventFunc(
		EchoVR::Broadcaster* broadcaster,
		EchoVR::SymbolId messageId,
		const CHAR* msgName,
		VOID* msg,
		UINT64 msgSize
	);
	BroadcasterReceiveLocalEventFunc* BroadcasterReceiveLocalEvent = (BroadcasterReceiveLocalEventFunc*)(g_GameBaseAddress + 0xF87AA0);

	/// <summary>
	/// Registers a callback for a certain type of game broadcaster message.
	/// </summary>
	/// <returns>An identifier for the callback registration, to be used for unregistering.</returns>
	typedef UINT16 BroadcasterListenFunc(
		EchoVR::Broadcaster* broadcaster,
		EchoVR::SymbolId messageId,
		BOOL isReliableMsgType,
		VOID* px,
		BOOL prepend
	);
	BroadcasterListenFunc* BroadcasterListen = (BroadcasterListenFunc*)(g_GameBaseAddress + 0xF80ED0);

	/// <summary>
	/// Unregisters a callback for a certain type of game broadcast message, using the return value from its registration.
	/// </summary>
	/// <returns>None</returns>
	typedef UINT64 BroadcasterUnlistenFunc(
		EchoVR::Broadcaster* broadcaster,
		UINT16 cbResult
	);
	BroadcasterUnlistenFunc* BroadcasterUnlisten = (BroadcasterUnlistenFunc*)(g_GameBaseAddress + 0xF8DF20);

	/// <summary>
	/// Obtains a JSON string value(with a default fallback value if it could not be obtained).
	/// </summary>
	/// <returns>The resulting string returned from the JSON get string operation.</returns>
	typedef CHAR* JsonValueAsStringFunc(
		EchoVR::Json* root,
		CHAR* keyName,
		CHAR* defaultValue,
		BOOL reportFailure
	);
	JsonValueAsStringFunc* JsonValueAsString = (JsonValueAsStringFunc*)(g_GameBaseAddress + 0x5FE290);

	/// <summary>
	/// Parses a URI string into a URI container structure.
	/// </summary>
	/// <returns>The result of the URI parsing operation.</returns>
	typedef HRESULT UriContainerParseFunc(
		EchoVR::UriContainer* uriContainer,
		CHAR* uri
	);
	UriContainerParseFunc* UriContainerParse = (UriContainerParseFunc*)(g_GameBaseAddress + 0x621EC0);

	/// <summary>
	/// Builds the CLI argument options and help descriptions list.
	/// </summary>
	/// <returns>TODO: Unverified, probably success result</returns>
	typedef UINT64 BuildCmdLineSyntaxDefinitionsFunc(
		PVOID pGame,
		PVOID pArgSyntax
	);
	BuildCmdLineSyntaxDefinitionsFunc* BuildCmdLineSyntaxDefinitions = (BuildCmdLineSyntaxDefinitionsFunc*)(g_GameBaseAddress + 0xFEA00);

	/// <summary>
	/// Adds an argument to the CLI argument syntax object.
	/// </summary>
	/// <returns>None</returns>
	typedef VOID AddArgSyntaxFunc(
		PVOID pArgSyntax,
		const CHAR* sArgName,
		UINT64 minOptions,
		UINT64 maxOptions,
		BOOL validate
	);
	AddArgSyntaxFunc* AddArgSyntax = (AddArgSyntaxFunc*)(g_GameBaseAddress + 0xD31B0);

	/// <summary>
	/// Adds an argument help string to the CLI argument syntax object.
	/// </summary>
	/// <returns>None</returns>
	typedef VOID AddArgHelpStringFunc(
		PVOID pArgSyntax,
		const CHAR* sArgName,
		const CHAR* sArgHelpDescription
	);
	AddArgHelpStringFunc* AddArgHelpString = (AddArgHelpStringFunc*)(g_GameBaseAddress + 0xD30D0);

	/// <summary>
	/// Processes the provided command line options for the running process.
	/// </summary>
	/// <returns>TODO: Unverified, probably success result</returns>
	typedef UINT64 PreprocessCommandLineFunc(
		PVOID pGame
	);
	PreprocessCommandLineFunc* PreprocessCommandLine = (PreprocessCommandLineFunc*)(g_GameBaseAddress + 0x116720);

	/// <summary>
	/// Writes a log to the logger, if all conditions such as log level are met.
	/// </summary>
	/// <returns>None</returns>
	typedef VOID WriteLogFunc(
		EchoVR::LogLevel logLevel,
		UINT64 unk,
		const CHAR* format,
		va_list vl
	);
	WriteLogFunc* WriteLog = (WriteLogFunc*)(g_GameBaseAddress + 0xEBE70);

	/// <summary>
	/// TODO: Seemingly parses an HTTP/HTTPS URI to be connected to.
	/// </summary>
	/// <returns>TODO: Unknown</returns>
	typedef UINT64 HttpConnectFunc(
		VOID* unk, 
		CHAR* uri
	);
	HttpConnectFunc* HttpConnect = (HttpConnectFunc*)(g_GameBaseAddress + 0x1F60C0);

	/// <summary>
	/// Loads the local config (located at ./_local/config.json) for the provided game instance.
	/// </summary>
	typedef UINT64 LoadLocalConfigFunc(
		PVOID pGame
	);
	LoadLocalConfigFunc* LoadLocalConfig = (LoadLocalConfigFunc*)(g_GameBaseAddress + 0x179EB0);

	/// <summary>
	/// Switches net game state to a given new state (loading level, logging in, logged in, lobby, etc).
	/// </summary>
	typedef VOID NetGameSwitchStateFunc(
		PVOID pGame, 
		NetGameState state
	);
	NetGameSwitchStateFunc* NetGameSwitchState = (NetGameSwitchStateFunc*)(g_GameBaseAddress + 0x1B8650);

	/// <summary>
	/// Schedules a return to the lobby in the net game.
	/// </summary>
	typedef VOID NetGameScheduleReturnToLobbyFunc(
		PVOID pGame
	);
	NetGameScheduleReturnToLobbyFunc* NetGameScheduleReturnToLobby = (NetGameScheduleReturnToLobbyFunc*)(g_GameBaseAddress + 0x1A89F0);

	/// <summary>
	/// The game's definition for GetProcAddress.
	/// Reference: https://learn.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-getprocaddress
	/// </summary>
	typedef FARPROC GetProcAddressFunc(
		HMODULE hModule,
		LPCSTR  lpProcName
	);
	GetProcAddressFunc* GetProcAddress = (GetProcAddressFunc*)(g_GameBaseAddress + 0xEAEF0);

	/// <summary>
	/// The game's definition for SetWindowTitle.
	/// Reference: https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowtexta
	/// </summary>
	typedef UINT64 SetWindowTextAFunc(
		HWND hWnd, 
		LPCSTR lpString
	);
	SetWindowTextAFunc* SetWindowTextA_ = (SetWindowTextAFunc*)(g_GameBaseAddress + 0x5105F0);
}
