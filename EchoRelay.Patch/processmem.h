#pragma once
#include "pch.h"

/// <summary>
/// Copies memory from a source buffer of a given size to a destination process memory buffer.
/// </summary>
/// <param name="pDestAddr">The process address where the source buffer should be copied to.</param>
/// <param name="pSrcAddr">The source buffer to copy to process memory.</param>
/// <param name="szSrcSize">The size of the data to copy.</param>
/// <returns>None</returns>
VOID ProcessMemcpy(PVOID pDestAddr, PVOID pSrcAddr, size_t szSrcSize)
{
    // Change the memory protection on the given address range, write process memory, then restore the original protection.
    DWORD dwOldProtect;
    if (VirtualProtect(pDestAddr, szSrcSize, PAGE_EXECUTE_READWRITE, &dwOldProtect))
    {
        WriteProcessMemory(GetCurrentProcess(), pDestAddr, pSrcAddr, szSrcSize, NULL);
        VirtualProtect(pDestAddr, szSrcSize, dwOldProtect, &dwOldProtect);
    }
}

/// <summary>
/// Sets a buffer of the given size in process memory to the provided byte value.
/// </summary>
/// <param name="pDestAddr">The process address where the memory should be set.</param>
/// <param name="val">The value to set each byte to.</param>
/// <param name="szDestSize">The size of the destination buffer to set.</param>
/// <returns>None</returns>
VOID ProcessMemset(PVOID pDestAddr, BYTE val, size_t szDestSize)
{
    // Memset a new buffer, copy it over, and free it.
    char* pbScratchPad = (char*)malloc(szDestSize);
    if (pbScratchPad != NULL)
    {
        memset(pbScratchPad, val, szDestSize);
        ProcessMemcpy(pDestAddr, pbScratchPad, szDestSize);
        free(pbScratchPad);
    }
}
