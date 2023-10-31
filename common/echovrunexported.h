#pragma once

#include "pch.h"
#include "echovr.h"

namespace EchoVR
{
	// Obtain a handle for the game
	CHAR* g_GameBaseAddress = (CHAR*)GetModuleHandle(NULL);

	/// <summary>
	/// Registers a callback for a certain type of websocket message.
	/// </summary>
	/// <returns>None</returns>
	typedef VOID TcpBroadcasterListenFunc(
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
	/// Listens for an event on the broadcaster.
	/// </summary>
	/// <returns>TODO: Unverified, probably success result.</returns>
	typedef UINT64 BroadcasterListenFunc(
		EchoVR::Broadcaster* broadcaster,
		EchoVR::SymbolId messageId,
		BOOL isReliableMsgType,
		VOID* px,
		BOOL prepend
	);
	BroadcasterListenFunc* BroadcasterListen = (BroadcasterListenFunc*)(g_GameBaseAddress + 0xF80ED0);

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
	/// A wrapper for WriteLog, simplifying logging operations.
	/// </summary>
	/// <returns>None</returns>
	VOID Log(EchoVR::LogLevel level, const CHAR* format, ...) {
		va_list args;
		va_start(args, format);
		WriteLog(level, 0, format, args);
		va_end(args);
	}


	/// <summary>
	/// TODO: Seemingly parses an HTTP/HTTPS URI to be connected to.
	/// </summary>
	/// <returns>TODO: Unknown</returns>
	typedef UINT64 HttpConnectFunc(VOID* unk, CHAR* uri);
	HttpConnectFunc* HttpConnect = (HttpConnectFunc*)(g_GameBaseAddress + 0x1F60C0);

	/// <summary>
	/// Loads the local config (located at ./_local/config.json) for the provided game instance.
	/// </summary>
	typedef UINT64 LoadLocalConfigFunc(PVOID pGame);
	LoadLocalConfigFunc* LoadLocalConfig = (LoadLocalConfigFunc*)(g_GameBaseAddress + 0x179EB0);
}
