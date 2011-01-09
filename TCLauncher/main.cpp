#include <Windows.h>
#include "inferior.h"

#pragma comment(lib, "inferior.lib")

int WINAPI _tWinMain( __in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in LPWSTR lpCmdLine, __in int nShowCmd )
{
	LPTSTR szDllName = _T("IPCHook.dll");
	TCHAR szDllPath[MAX_PATH];

	GetModuleFileName(NULL, szDllPath, MAX_PATH);
	_tcscpy_s(_tcsrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);

	if(!CreateAndInjectDll(lpCmdLine, NULL, szDllPath))
		ErrorExit(_T("Injection"));

	return ERROR_SUCCESS;
}