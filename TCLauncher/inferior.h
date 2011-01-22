#ifndef _INFERIOR_H_
#define _INFERIOR_H_

#include <Windows.h>
#include <tchar.h>
#include <TlHelp32.h>

BOOL FileExists(LPCTSTR fileName);
VOID ErrorExit(LPTSTR lpszFunction, BOOL bExit = FALSE);
DWORD InjectDll(DWORD dwProcessId, LPCTSTR dllName);
DWORD EjectDll(DWORD dwProcessId, LPCTSTR dllName);
BOOL CreateAndInjectDll(LPCTSTR lpProcessName, LPTSTR lpArgs, LPCTSTR dllName);
HANDLE CreateInvisible(LPCTSTR lpProcName, LPTSTR lpArgs, LPDWORD dwPid);
BOOL WaitAndInjectDll(LPCTSTR lpModuleBaseName, LPCTSTR dllName);
BOOL MultiByteToUnicode(LPCSTR multiByteStr, LPWSTR unicodeStr, DWORD size);
BOOL UnicodeToMultiByte(LPCWSTR unicodeStr, LPSTR multiByteStr, DWORD size);

#endif // _INFERIOR_H_