#include <Windows.h>
#include "inferior.h"

#pragma comment(lib, "inferior.lib")

int WINAPI _tWinMain( __in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in LPWSTR lpCmdLine, __in int nShowCmd )
{
	LPTSTR szDllName = _T("IPCHook.dll");
	TCHAR szDllPath[MAX_PATH];

	TCHAR szTrueCryptPath[MAX_PATH];
	LPTSTR lpPathBegin = _tcschr(lpCmdLine, _T('"')) + 1;
	LPTSTR lpPathEnd = _tcschr(lpPathBegin, _T('"'));
	int iLength = lpPathEnd - lpPathBegin;
	_tcsncpy_s(szTrueCryptPath, sizeof(szTrueCryptPath), lpPathBegin, iLength);

	GetModuleFileName(NULL, szDllPath, MAX_PATH);
	_tcscpy_s(_tcsrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);

	if(!CreateAndInjectDll(szTrueCryptPath, lpPathEnd + 1, szDllPath))
		ErrorExit(_T("Injection"));

	return ERROR_SUCCESS;
}