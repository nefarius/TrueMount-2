#include <Windows.h>
#include <tchar.h>
#include <string>
using namespace std;
#include "NInjectLib/IATModifier.h"

VOID ErrorExit(LPTSTR lpszFunction, BOOL bExit = TRUE);

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
#ifdef _DEBUG
	MessageBox(NULL, lpPathEnd + 1, L"TCArgs", MB_ICONINFORMATION|MB_OK);
#endif

	// Get absolute DLL location
	GetModuleFileName(hInstance, szDllPath, MAX_PATH);
	_tcscpy_s(_tcsrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);
	wstring wsDllPath(szDllPath);
	string strDllPath(wsDllPath.begin(), wsDllPath.end());

#ifdef _DEBUG
	MessageBox(NULL, szTrueCryptPath, _T("TC"), MB_ICONINFORMATION|MB_OK);
	MessageBoxA(NULL, strDllPath.c_str(), NULL, MB_OK|MB_ICONINFORMATION);
#endif

	// Get Operating System Version
	OSVERSIONINFO osVer;
	ZeroMemory(&osVer, sizeof(osVer));
	osVer.dwOSVersionInfoSize = sizeof(osVer);
	if(!GetVersionEx(&osVer))
		ErrorExit(_T("GetVersionEx"));

#ifdef _DEBUG
	if(osVer.dwMajorVersion == 5)
		MessageBox(NULL, _T("Windows XP"), _T("OS"), MB_ICONINFORMATION|MB_OK);
	else if(osVer.dwMajorVersion == 6)
		MessageBox(NULL, _T("Windows Vista/7"), _T("OS"), MB_ICONINFORMATION|MB_OK);
#endif

	// Define and clean process information
	STARTUPINFO lpStartupInfo;
	ZeroMemory(&lpStartupInfo, sizeof(lpStartupInfo));
	lpStartupInfo.cb = sizeof(lpStartupInfo);
	PROCESS_INFORMATION lpProcInfo;
	ZeroMemory(&lpProcInfo, sizeof(lpProcInfo));

	// Fire up TrueCrypt
	if(CreateProcess(szTrueCryptPath,
		lpPathEnd + 1,
		NULL,
		NULL,
		FALSE,
		CREATE_SUSPENDED,
		NULL,
		NULL,
		&lpStartupInfo,
		&lpProcInfo))
	{
		try 
		{
			Process process(lpProcInfo.dwProcessId);
			IATModifier iatModifier(process);
			// retrieve image base address so IATModifier is able to find the import directory
			iatModifier.setImageBase(process.getImageBase(lpProcInfo.hThread));
			// modify import directory so our injected dll is loaded first
			iatModifier.writeIAT(strDllPath);
			ResumeThread(lpProcInfo.hThread);
		}
		catch (std::exception& e)
		{
#ifdef _DEBUG
			MessageBox(NULL, _T("Injection failed!"), _T("Fail"), MB_ICONERROR|MB_OK);
#endif
			ResumeThread(lpProcInfo.hThread);
			CloseHandle(lpProcInfo.hThread);
			CloseHandle(lpProcInfo.hProcess);
		}
	}
	else
		ErrorExit(_T("CreateProcess"));

	// Wait till process exited
	WaitForSingleObject(lpProcInfo.hProcess, INFINITE);
	// Cleanup
	CloseHandle(lpProcInfo.hProcess);
	CloseHandle(lpProcInfo.hThread);

	// Send an OK message back home
	HANDLE npipe;
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
		else
			ErrorExit(_T("CreatePipe"));
	}

	return ERROR_SUCCESS;
}

VOID ErrorExit(LPTSTR lpszFunction, BOOL bExit)
{
	// Retrieve the system error message for the last-error code
	LPVOID lpMsgBuf;
	LPVOID lpDisplayBuf;
	DWORD dw = GetLastError(); 

	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | 
		FORMAT_MESSAGE_FROM_SYSTEM |
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		dw,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR) &lpMsgBuf,
		0, NULL );

	// Display the error message and exit the process
	lpDisplayBuf = (LPVOID)LocalAlloc(LMEM_ZEROINIT, 
		(lstrlen((LPCTSTR)lpMsgBuf) + lstrlen((LPCTSTR)lpszFunction) + 40) * sizeof(TCHAR));
	_stprintf_s((LPTSTR)lpDisplayBuf, 
		LocalSize(lpDisplayBuf) / sizeof(TCHAR), 
		TEXT("%s failed with error %d: %s"), 
		lpszFunction, dw, lpMsgBuf); 
	MessageBox(NULL, (LPCTSTR)lpDisplayBuf, TEXT("Error"), MB_OK|MB_ICONERROR); 

	// Clean up
	LocalFree(lpMsgBuf);
	LocalFree(lpDisplayBuf);

	if(bExit)
		ExitProcess(dw);
}
