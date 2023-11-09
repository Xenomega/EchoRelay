#include "echovrunexported.h"
#include "patches.h"
#include "processmem.h"
#include <detours.h>

/// <summary>
/// Indicates whether the patches have been applied (to avoid re-application).
/// </summary>
BOOL initialized = FALSE;

/// <summary>
/// A CLI argument flag indicating whether the game is booting as a dedicated server.
/// </summary>
BOOL isServer = FALSE;
/// <summary>
/// A CLI argument flag indicating whether the game is booting as an offline client.
/// </summary>
BOOL isOffline = FALSE;
/// <summary>
/// A CLI argument flag indicating whether the game is booting in headless mode (no graphics/audio).
/// </summary>
BOOL isHeadless = FALSE;
/// <summary>
/// A CLI argument flag indicating whether the game is booting in a windowed mode, rather than with a VR headset.
/// </summary>
BOOL isWindowed = FALSE;

/// <summary>
/// Indicates whether the game was launched with `-noovr`.
/// </summary>
BOOL isNoOVR = FALSE;

/// <summary>
/// The window handle for the current game window.
/// </summary>
HWND hWindow = NULL;

/// <summary>
/// The local config stored in ./_local/config.json.
/// </summary>
EchoVR::Json* localConfig = NULL;

/// <summary>
/// A timestep value in ticks/updates per second, to be used for headless mode (due to lack of GPU/refresh rate throttling).
/// If non-zero, sets the timestep override by the given tick rate per second.
/// If zero, removes tick rate throttling.
/// </summary>
UINT64 headlessTimeStep = 120;

/// <summary>
/// Reports a fatal error with a message box, then exits the game.
/// </summary>
/// <param name="msg">The window message to display.</param>
/// <param name="title">The window title to display.</param>
/// <returns>None</returns>
VOID FatalError(const CHAR* msg, const CHAR* title)
{
    // If no title or msg was provided, set it to a generic value.
    if (title == NULL)
        title = "Echo Relay: Error";
    if (msg == NULL)
        msg = "An unknown error occurred.";

    // Show a message box.
    MessageBoxA(NULL, msg, title, MB_OK);

    // Force process exit with an error code.
    exit(1);
}

/// <summary>
/// Patches a given function pointer with an hook function (matching the equivalent function signature as the original).
/// </summary>
/// <param name="ppPointer">The function to detour.</param>
/// <param name="pDetour">The function hook to use as a detour.</param>
/// <returns>None</returns>
VOID PatchDetour(PVOID* ppPointer, PVOID pDetour)
{
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());
    DetourAttach(ppPointer, pDetour);
    DetourTransactionCommit();
}

/// <summary>
/// A detour hook for the game's "write log" function. It intercepts overly noisy logs and ensures they are outputted over stdout/stderr for headless mode.
/// </summary>
/// <param name="logLevel">The level the message was logged with.</param>
/// <param name="unk">TODO: Unknown</param>
/// <param name="format">The format string to log with.</param>
/// <param name="vl">The list of variables to use to format the format string before logging.</param>
/// <returns>None</returns>
VOID WriteLogHook(EchoVR::LogLevel logLevel, UINT64 unk, const CHAR* format, va_list vl)
{
    // Filter out very noisy messages by quitting early.
    if (!strcmp(format, "[DEBUGPRINT] %s %s"))
    {
        // If the overall template matched, format it
        CHAR formattedLog[0x1000];
        memset(formattedLog, 0, sizeof(formattedLog));
        vsprintf_s(formattedLog, format, vl);

        // If the final output matches the strings below, we do not log.
        if (!strcmp(formattedLog, "[DEBUGPRINT] PickRandomTip: context = 0x41D2C432172E0810")) // noisy in main menu / loading screen
            return;
    }
    else if (!strcmp(format, "[NETGAME] No screen stats info for game mode %s")) // noisy in social lobby
        return;

    // Print the ANSI color code prefix for the given log level.
    switch (logLevel)
    {
        case EchoVR::LogLevel::Debug:
            printf("\u001B[36m");
            break;

        case EchoVR::LogLevel::Warning:
            printf("\u001B[33m");
            break;

        case EchoVR::LogLevel::Error:
            printf("\u001B[31m");
            break;

        case EchoVR::LogLevel::Info:
        default:
            printf("\u001B[0m");
            break;
    }

    // Print the output to our allocated console.
    vprintf(format, vl);
    printf("\n");

    // Print the ANSI color code for restoring the default text style.
    printf("\u001B[0m");

    // Call the original method
    EchoVR::WriteLog(logLevel, unk, format, vl);
}

/// <summary>
/// A wrapper for WriteLog, simplifying logging operations.
/// </summary>
/// <returns>None</returns>
VOID Log(EchoVR::LogLevel level, const CHAR* format, ...) {
    va_list args;
    va_start(args, format);
    if (isHeadless)
        WriteLogHook(level, 0, format, args);
    else
        EchoVR::WriteLog(level, 0, format, args);
    va_end(args);
}

/// <summary>
/// Patches the game to enable headless mode, spawning a console window and applying patches to avoid game crashes.
/// </summary>
/// <param name="pGame">The pointer to the instance of the game structure.</param>
/// <returns>None</returns>
VOID PatchEnableHeadless(PVOID pGame)
{
    // Disable audio by clearing the same bits as the `-noaudio` command would.
    UINT32* flags = (UINT32*)((CHAR*)pGame + 468);
    *flags &= 0xFFFFFFFD; // clear second bit

    // Create a console
    // Note: We do this because attaching to the parent process console would already be detached due to /SUBSYSTEM:WINDOWS.
    // Attaching two processes to a console at once would be messy and.
    AllocConsole();
    
    // Redirect our standard streams to the new console.
    FILE* fConsole;
    freopen_s(&fConsole, "CONIN$", "r", stdin);
    freopen_s(&fConsole, "CONOUT$", "w", stderr);
    freopen_s(&fConsole, "CONOUT$", "w", stdout);
     
    // Enable ANSI color coding on the console.
    HANDLE hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
    HANDLE hStdErr = GetStdHandle(STD_ERROR_HANDLE);
    DWORD consoleMode;

    GetConsoleMode(hStdOut, &consoleMode);
    consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
    SetConsoleMode(hStdOut, consoleMode);

    GetConsoleMode(hStdErr, &consoleMode);
    consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
    SetConsoleMode(hStdErr, consoleMode);

    // Install our hook to capture logs to the console.
    PatchDetour(&(PVOID&)EchoVR::WriteLog, WriteLogHook);

    // Patch the engine initialization/configuration to skip initialization of the rendering providers.
    BYTE pbPatch[] = {
        0xA8, 0x00 // TEST al, 0 (replaces a test against 1, to skip the renderer initialization).
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xFF581, pbPatch, sizeof(pbPatch));

    // Patch effects resource loading to be skipped over.
    BYTE pbPatch2[] = {
        0xEB, 0x41 // JMP 0x43
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x62CA91, pbPatch2, sizeof(pbPatch2));

    // If a timestep is set as non-zero, patch to enable `-fixedtimestep`.
    if (headlessTimeStep != 0)
    {
        // Set the flag for `-fixedtimestep`.
        UINT64* flags = (UINT64*)((CHAR*)pGame + 2088);
        *flags |= 0x2000000;
    }
}

/// <summary>
/// Patches the game to run as a dedicated server, exposing its game server broadcast port, adjusting its log file path.
/// </summary>
/// <returns>None</returns>
VOID PatchEnableServer()
{
    // Patch the flags for our game to indicate we are a game server. This replaces checks to see if we
    // are a server, with code to set the flag permanently, and skips over the rest of the checking code.
    BYTE pbPatch[] = {
        0x48, 0x83, 0x08, 0x06, // OR QWORD ptr[rax], 0x6 (bit 2 = load sessions received from broadcast, bit 3 = patch flag to set as dedicated server)

        0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, // NOP instructions to replace server flag checks (with above setting operation) until it sets the 'enabled' flag.
        0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
        0x90, 0x90, 0x90, 0x90
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x1580C3, pbPatch, sizeof(pbPatch));

    // Patch to avoid enabling "r14netserver" logging, as this depends on files we do not have and will panic.
    BYTE pbPatch2[] = {
        0x48, 0x89, 0xC3, 0x90 // NOPs to avoid a comparison->move to overwrite "r14netserver"
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xFFA58, pbPatch2, sizeof(pbPatch2));

    // Patch the update the logging subject to "r14(server)"
    BYTE pbPatch3[] = {
        0xEB, 0x0E
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xFFB0E, pbPatch3, sizeof(pbPatch3));

    // Patch the ./sourcedb/rad15/json/r14/config/netconfig_*.json file parsing routine so the "allow_incoming" key is always interpreted as `true`.
    // This is necessary for a game server to accept players. Otherwise they will be denied, disallowing client connections.
    BYTE pbPatch4[] = {
        0xB8, 0x01, 0x00, 0x00, 0x00 // MOV eax, 1 (set the flag to `true`).
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xF7F904, pbPatch4, sizeof(pbPatch4));

    // Patch the CLI pre-processing method to assume the process was provided "-spectatorstream".
    // This causes the game to enter a "load lobby" state, which as a game server, starts the game server on startup.
    // Otherwise, you would need to manually click the "play" button before the server began serving.
    BYTE pbPatch5[] = {
        0x90, 0x90, 0x90, 0x90, 0x90, 0x90 // NOP the jump that is taken if "-spectatorstream" is not provided.
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x116F3D, pbPatch5, sizeof(pbPatch5));
}

/// <summary>
/// Patches the game to run as an offline client, loading a game of the configuration specified by -gametype, -level, and -region CLI arguments.
/// </summary>
/// <returns>None</returns>
VOID PatchEnableOffline()
{
    // Patch "starting multiplayer"
    BYTE pbPatch[] = {
        0xE8, 0xCD, 0x02, 0x00, 0x00
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xFDE0E, pbPatch, sizeof(pbPatch));

    // Patch "incidents"
    BYTE pbPatch3[] = {
        0x75, 0x0A, // TODO: Can probably be made JMP (0xEB) / NOP
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x17F0B1, pbPatch3, sizeof(pbPatch3));

    // TODO: Title
    BYTE pbPatch4[] = {
        0x74, 0x12, // TODO: Can probably be made JMP (0xEB) / NOP
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x17F77B, pbPatch4, sizeof(pbPatch4));

    // Force transaction service to load
    BYTE pbNopConditionalJump[] = {
        0x90, 0x90, // NOP condition jump
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x17F817, pbNopConditionalJump, sizeof(pbNopConditionalJump));
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x17F823, pbNopConditionalJump, sizeof(pbNopConditionalJump));

    // Skip failed logon service code
    BYTE pbPatch5[] = {
        0xE9, 0x92, 0x00, 0x00, 0x00, 0x00 // JMP 0x97
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x1AC83E, pbPatch5, sizeof(pbPatch5));

    // Redirect "beginning tutorial"
    BYTE pbPatch6[] = {
        0xE8, 0xD6, 0x17, 0x68, 0xFF
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0xA7C685, pbPatch6, sizeof(pbPatch6));
}

/// <summary>
/// Patches the game to allow -noovr (demo accounts) without use of spectator stream. This provides a temporary player profile.
/// </summary>
/// <returns>None</returns>
VOID PatchNoOvrRequiresSpectatorStream()
{
    // Patch "-noovr requires -spectatorstream" to allow us to use -noovr independently.
    BYTE pbPatch[] = {
        0xEB, 0x35 // JMP (past the respective code).
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x11690D, pbPatch, sizeof(pbPatch));
}

/// <summary>
/// Patches the dead lock monitor, which monitors threads to ensure they have not stopped processing. If one does, it triggers a fatal error.
/// This patch is provided to ensure breakpoints set during testing do not trigger the deadlock monitor, thereby killing the process.
/// </summary>
/// <returns>None</returns>
VOID PatchDeadlockMonitor()
{
    // Patch out the deadlock monitor thread's validation routine. This is necessary during debugging, as this thread acts as a watchdog
    // for updates and will panic if an update has not occurred for some time (e.g. waiting too long between breakpoints when debugging).
    BYTE pbPatch[] = {
        0x90, 0x90 // NOPs (to replace the JLE instruction which checks failing deadlock conditions).
    };
    ProcessMemcpy(EchoVR::g_GameBaseAddress + 0x1D3881, pbPatch, sizeof(pbPatch));
}

/// <summary>
/// A detour hook for the game's method it uses to transition from one net game state to another.
/// </summary>
/// <param name="game">A pointer to the game instance.</param>
/// <param name="state">The state to transition to.</param>
/// <returns>None</returns>
VOID NetGameSwitchStateHook(PVOID pGame, EchoVR::NetGameState state)
{
    // Hook the net game switch state function, so we can redirect "load level failed" to a ready state again.
    // This way if a client requests a non-existent level, the game server library isn't unloaded due to a state
    // transition to "load failed" (because the level failed to load)
    if (isServer && state == EchoVR::NetGameState::LoadFailed)
    {
        // Schedule a return to lobby. We are already at lobby, but this will quickly end the session, removing
        // all players, and start listening for a new one, to keep the server recycling itself appropriately.
        // Note: This is an ugly hack, as the client will get an irrelevant connection failure message (server is full, 
        // failed to connect, etc). But at least it doesn't cause the server to get stuck in a "not ready" state in some menu.
        Log(EchoVR::LogLevel::Debug, "[ECHORELAY.PATCH] Dedicated server failed to load level. Resetting session to keep game server available.");
        EchoVR::NetGameScheduleReturnToLobby(pGame);
        return;
    }

    // Call the original function
    EchoVR::NetGameSwitchState(pGame, state);
}

/// <summary>
/// A detour hook for the game's method it uses to build CLI argument definitions. 
/// Adds additional definitions to the structure, so that they may be parsed successfully without error.
/// </summary>
/// <param name="game">A pointer to the game instance.</param>
/// <param name="pArgSyntax">A pointer to the CLI argument structure tracking all CLI arguments.</param>
UINT64 BuildCmdLineSyntaxDefinitionsHook(PVOID pGame, PVOID pArgSyntax)
{
    // Add all original CLI argument options.
    UINT64 result = EchoVR::BuildCmdLineSyntaxDefinitions(pGame, pArgSyntax);

    // Add our additional options
    EchoVR::AddArgSyntax(pArgSyntax, "-server", 0, 0, FALSE);
    EchoVR::AddArgHelpString(pArgSyntax, "-server", "[EchoRelay] Run as a dedicated game server");

    EchoVR::AddArgSyntax(pArgSyntax, "-offline", 0, 0, FALSE);
    EchoVR::AddArgHelpString(pArgSyntax, "-offline", "[EchoRelay] Run the game in offline mode");

    EchoVR::AddArgSyntax(pArgSyntax, "-windowed", 0, 0, FALSE);
    EchoVR::AddArgHelpString(pArgSyntax, "-windowed", "[EchoRelay] Run the game with no headset, in a window");

    EchoVR::AddArgSyntax(pArgSyntax, "-timestep", 1, 1, FALSE);
    EchoVR::AddArgHelpString(pArgSyntax, "-timestep", "[EchoRelay] Sets the fixed update interval when using -headless (in ticks/updates per second). 0 = no fixed time step, 120 = default");

    return result;
}

/// <summary>
/// A detour hook for the game's command line pre-processing method, used to parse command line arguments.
/// </summary>
/// <param name="pGame">A pointer to the game instance.</param>
UINT64 PreprocessCommandLineHook(PVOID pGame)
{
    // Check which were set with command line arguments.
    int argc;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLineW(), &argc);
    for (int i = 0; i < argc; i++)
    {
        if (lstrcmpW(argv[i], L"-server") == 0)
            isServer = TRUE;
        else if (lstrcmpW(argv[i], L"-offline") == 0)
            isOffline = TRUE;
        else if (lstrcmpW(argv[i], L"-headless") == 0)
            isHeadless = TRUE;
        else if (lstrcmpW(argv[i], L"-windowed") == 0)
            isWindowed = TRUE;
        else if (lstrcmpW(argv[i], L"-noovr") == 0)
            isNoOVR = TRUE;
        else if (lstrcmpW(argv[i], L"-timestep") == 0)
        {
            // Verify a timestep argument was provided.
            if (i + 1 < argc)
                headlessTimeStep = std::wcstoull((const WCHAR*)argv[i + 1], nullptr, 10);
            else
                FatalError("No argument provided for -timestep. You must provide a positive number for a fixed tick rate, or a zero value for unthrottled.", NULL);
        }
    }

    // Verify server and offline flags are not enabled.
    if (isServer && isOffline)
        FatalError("-server and -offline arguments cannot be provided at the same time.", NULL);

    // If offline flag was provided, enable offline.
    if (isOffline)
        PatchEnableOffline();

    // If the headless flag was provided, enable it.
    if (isHeadless)
        PatchEnableHeadless(pGame);

    // If the windowed, server, or headless flags were provided, apply the windowed mode patch to not use a VR headset.
    if (isWindowed || isServer || isHeadless)
    {
        // Set the game to run in windowed mode.
        UINT64* flags = (UINT64*)((CHAR*)pGame + 31456);
        *flags |= 0x0100000; // Spectator stream uses 0x2100000 (an additional flag). This changes level setting in some way(?). Seemingly unnecessary here.
    }

    // Apply patches to force the game to load as a server.
    if (isServer)
        PatchEnableServer();

    // Update the window title
    if (hWindow != NULL && isNoOVR)
        EchoVR::SetWindowTextA_(hWindow, "Echo VR - [DEMO]");

    // Run the original method
    UINT64 result = EchoVR::PreprocessCommandLine(pGame);
    return result;
}

/// <summary>
/// A detour hook for the game's function to load the local config.json for the game instance.
/// </summary>
/// <param name="pGame">A pointer to the game struct to load the config for.</param>
UINT64 LoadLocalConfigHook(PVOID pGame)
{
    // If a timestep override was provided, configure it.
    // This is placed here, as by this time, the structure to dereference will be initialized.
    if (isHeadless && headlessTimeStep != 0)
    {
        // Patch the fixed time step based on tick count.
        // Fixed time step is in microseconds, tick rate is per second.
        UINT32* timeStep = (UINT32*)(*(CHAR**)(EchoVR::g_GameBaseAddress + 0x020A00E8) + 0x90);
        *timeStep = 1000000 / headlessTimeStep;
    }

    // Store a reference to the local config.
    localConfig = (EchoVR::Json*)((CHAR*)pGame + 0x63240);
    return EchoVR::LoadLocalConfig(pGame);
}

/// <summary>
/// A detour hook for the game's method it uses to connect to an HTTP(S) endpoint. This is used to redirect additional hardcoded endpoints in the game.
/// </summary>
/// <param name="unk">TODO: Unknown</param>
/// <param name="uri">The HTTP(S) URI string to connect to.</param>
UINT64 HttpConnectHook(PVOID unk, CHAR* uri)
{
    // If we have a local config, check for additional service overrides.
    if (localConfig != NULL)
    {
        // Perform overrides for different hosts
        CHAR* originalApiHostPrefix = (CHAR*)"https://api.";
        CHAR* originalOculusGraphHost = (CHAR*)"https://graph.oculus.com";
        if (!strncmp(uri, originalApiHostPrefix, strlen(originalApiHostPrefix)))
        {
            // Check for JSON keys definition host overrides.
            uri = EchoVR::JsonValueAsString(localConfig, (CHAR*)"api_host", uri, false);
            uri = EchoVR::JsonValueAsString(localConfig, (CHAR*)"apiservice_host", uri, false);
        }
        else if (!strncmp(uri, originalOculusGraphHost, strlen(originalOculusGraphHost)))
        {
            // Check for JSON keys definition host overrides.
            uri = EchoVR::JsonValueAsString(localConfig, (CHAR*)"graph_host", uri, false);
            uri = EchoVR::JsonValueAsString(localConfig, (CHAR*)"graphservice_host", uri, false);
        }
    }

    // Call the original function
    return EchoVR::HttpConnect(unk, uri);
}

/// <summary>
/// A detour hook for the game's method to wrap GetProcAddress.
/// </summary>
/// <param name="hModule">The module to get the procedure address from.</param>
/// <param name="lpProcName">The exported procedure name.</param>
/// <returns>The address of the procedure, or NULL if it was not found.</returns>
FARPROC GetProcAddressHook(HMODULE hModule, LPCSTR lpProcName)
{
    // If this is a server, unloading pnsdemo.dll or pnsovr.dll currently causes dereferencing of freed memory 
    // during a RadPluginShutdown call. This could be due to the timing on the patch for dedicated servers causing 
    // some structures to initialize incorrectly or something.
    // For now, we resolve this by simply force exiting the server.
    
    // If we're performing a plugin shutdown, check if this is a user platform DLL such as pnsdemo.dll or pnsovr.dll, 
    // which exports a "Users" method.
    if (isServer && strcmp(lpProcName, "RadPluginShutdown") == 0)
    {
        // If this is a user platform dll, exit the whole process with a success code instead of continuing to gracefully unload.
        if (EchoVR::GetProcAddress(hModule, "Users") != NULL)
            exit(0);
    }

    // Call the original function.
    return EchoVR::GetProcAddress(hModule, lpProcName);
}

/// <summary>
/// A detour hook for the game's definition of SetWindowTextA.
/// </summary>
/// <param name="hWnd">The window handle to update the title for.</param>
/// <param name="lpString">The title string to be used.</param>
/// <returns>True if successful, false otherwise.</returns>
BOOL SetWindowTextAHook(HWND hWnd, LPCSTR lpString)
{
    // Store a reference to the window
    hWindow = hWnd;

    // Call the original function and return the result.
    return EchoVR::SetWindowTextA_(hWnd, lpString);
}

/// <summary>
/// Verifies the version of the game is supported.
/// </summary>
/// <returns>None</returns>
BOOL VerifyGameVersion()
{
    // Definitions to read image file header.
    #define IMG_SIGNATURE_OFFSET    0x3C
    #define IMG_SIGNATURE_SIZE      0x04

    // Obtain the image file header for the game.
    DWORD* signatureOffset = (DWORD*)(EchoVR::g_GameBaseAddress + IMG_SIGNATURE_OFFSET);
    IMAGE_FILE_HEADER* coffFileHeader = (IMAGE_FILE_HEADER*)(EchoVR::g_GameBaseAddress + (*signatureOffset + IMG_SIGNATURE_SIZE));
    
    // Verify the timestamp for Echo VR (version 34.4.631547.1).
    // Timestamp should be Wednesday, May 3, 2023 10:28:06 PM.
    // Note: For other executables, this may not hold. Reproducible builds have pushed this
    // towards being a build signature. In any case, it works as a naive integrity check.
    return coffFileHeader->TimeDateStamp == 0x6452dff6;
}

/// <summary>
/// Initializes the patcher, executing startup patchs on the game and installing detours/hooks on various game functions.
/// </summary>
/// <returns>None</returns>
VOID Initialize()
{
    // If we already initialized the library, stop.
    if (initialized)
        return;
    initialized = true;

    // Verify the game version before patching
    if (!VerifyGameVersion())
        MessageBox(NULL, L"EchoRelay version check failed. Patches may fail to be applied. Verify you're running the correct version of Echo VR.", L"Echo Relay: Warning", MB_OK);

    // Patch our CLI argument options to add our additional options.
    PatchDetour(&(PVOID&)EchoVR::BuildCmdLineSyntaxDefinitions, BuildCmdLineSyntaxDefinitionsHook);
    PatchDetour(&(PVOID&)EchoVR::PreprocessCommandLine, PreprocessCommandLineHook);
    PatchDetour(&(PVOID&)EchoVR::NetGameSwitchState, NetGameSwitchStateHook);
    PatchDetour(&(PVOID&)EchoVR::LoadLocalConfig, LoadLocalConfigHook);
    PatchDetour(&(PVOID&)EchoVR::HttpConnect, HttpConnectHook);
    PatchDetour(&(PVOID&)EchoVR::GetProcAddress, GetProcAddressHook);
    PatchDetour(&(PVOID&)EchoVR::SetWindowTextA_, SetWindowTextAHook);

    // Run some startup patches
    PatchNoOvrRequiresSpectatorStream();

    // Patch out the deadlock monitor thread's validation routine if we're compiling in debug mode, as this will panic from process suspension.
#if _DEBUG
    PatchDeadlockMonitor();
#endif
}