#include <iostream>
#include "pch.h"
#include <locale>
#include <codecvt>
#include <detours.h>

// Note: A lazy hack here for Windows systems which enable paths longer than MAX_PATH.
#define PATH_SIZE   (MAX_PATH * 10)

int main()
{
    // Define our game and patch paths.
    WCHAR sCurrentExeFilePath[PATH_SIZE];
    WCHAR sCurrentExeDir[PATH_SIZE];
    WCHAR sGameExe[PATH_SIZE];
    CHAR sPatchDll[PATH_SIZE];

    // Get the current executable path.
    GetModuleFileNameW(NULL, sCurrentExeFilePath, PATH_SIZE);
    //std::wcout << L"Current executable path: " << sCurrentExeFilePath << std::endl;

    // Obtain our executable directory.
    _wsplitpath_s(sCurrentExeFilePath, sCurrentExeDir, PATH_SIZE, NULL, 0, NULL, 0, NULL, 0);
    size_t curLen = wcslen(sCurrentExeDir);
    _wsplitpath_s(sCurrentExeFilePath, NULL, 0, sCurrentExeDir + curLen, PATH_SIZE - curLen, NULL, 0, NULL, 0);
    //std::wcout << L"Current executable directory: " << sCurrentExeDir << std::endl;

    // Convert the executable directory to a non-wide character string.
    std::wstring curExeDirStrW = sCurrentExeDir;
    std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
    const std::string curExeDirStrA = converter.to_bytes(curExeDirStrW);

    // Obtain our game exe path and patch dll path.
    swprintf((WCHAR*)sGameExe, PATH_SIZE, L"%s\\echovr.exe", sCurrentExeDir);
    snprintf(sPatchDll, PATH_SIZE, "%s\\EchoRelay.Patch.dll", curExeDirStrA.c_str());
    //std::wcout << L"EchoVR path: " << sGameExe << std::endl;
    //std::cout << "Patch DLL path: " << sPatchDll << std::endl;

    // Get the command line arguments
    LPWSTR sOriginalArgs = GetCommandLineW();

    // Clone the command line arguments to a writeable buffer.
    size_t szArgsLen = wcslen(sOriginalArgs) * sizeof(WCHAR) + 1;
    WCHAR* sArgs = (WCHAR*)malloc(szArgsLen);
    if (sArgs == NULL)
    {
        std::cerr << "Failed to allocate memory for CLI argument buffer." << std::endl;
        return 1;
    }
    ZeroMemory(sArgs, szArgsLen);
    wcscpy_s(sArgs, szArgsLen, sOriginalArgs);

    // Now that we have all of the necessary information, we'll want to invoke the target process.

    // Define process information structures to capture our spawned process info.
    STARTUPINFO StartupInfo = { 0 };
    PROCESS_INFORMATION ProcInfo = { 0 };
    StartupInfo.cb = sizeof(STARTUPINFO);

    // Build our list of dlls to inject.
    LPCSTR dllsToInject[] = { sPatchDll };

    // Create the process in a suspended state with our injected DLLs.
    if (DetourCreateProcessWithDlls(sGameExe, sArgs, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL,
        NULL, &StartupInfo, &ProcInfo, ARRAYSIZE(dllsToInject), dllsToInject, NULL) == FALSE)
        return false;

    // Resume the process.
    ResumeThread(ProcInfo.hThread);

    // Close the process and thread handles.
    CloseHandle(ProcInfo.hProcess);
    CloseHandle(ProcInfo.hThread);

    // Free allocated memory.
    free(sArgs);
}