#include <Windows.h>
#include "inferior.h"

#pragma comment(lib, "inferior.lib")

int WINAPI _tWinMain( __in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in LPWSTR lpCmdLine, __in int nShowCmd )
{
	// Without arguments no work
	if(_tcslen(lpCmdLine) <= 0)
		return ERROR_BAD_ARGUMENTS;

	// The name and path of the IPC Hook DLL
	LPTSTR szDllName = _T("IPCHook.dll");
	TCHAR szDllPath[MAX_PATH];

	// Extract the path to TrueCrypt
	TCHAR szTrueCryptPath[MAX_PATH];
	LPTSTR lpPathBegin = _tcschr(lpCmdLine, _T('"')) + 1;
	LPTSTR lpPathEnd = _tcschr(lpPathBegin, _T('"'));
	int iLength = lpPathEnd - lpPathBegin;
	_tcsncpy_s(szTrueCryptPath, sizeof(szTrueCryptPath), lpPathBegin, iLength);

	// Get absolute DLL location
	GetModuleFileName(NULL, szDllPath, MAX_PATH);
	_tcscpy_s(_tcsrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);

	// Create the process and inject the library
	if(!CreateAndInjectDll(szTrueCryptPath, lpPathEnd + 1, szDllPath, INFINITE))
		ErrorExit(_T("Injection"));

	HANDLE npipe;
	// Send an OK message back home
	if( WaitNamedPipe(L"\\\\.\\pipe\\TrueCryptMessage", 3000) )
	{
		npipe = CreateFile(L"\\\\.\\pipe\\TrueCryptMessage",
			GENERIC_READ | GENERIC_WRITE,
			0, NULL, OPEN_EXISTING, 0, NULL);
		if( npipe != INVALID_HANDLE_VALUE )
		{
			DWORD dwRead;
			WriteFile(npipe, (LPCVOID)_T("OK"), sizeof(TCHAR) * 2, &dwRead, NULL);
			CloseHandle(npipe);
		}
	}

	return ERROR_SUCCESS;
}