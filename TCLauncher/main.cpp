#include <Windows.h>
#include <tchar.h>

//#define _NOINJECT

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
	MessageBox(NULL, lpPathEnd + 5, L"TCArgs", MB_ICONINFORMATION|MB_OK);
#endif

	// Get absolute DLL location
	GetModuleFileName(NULL, szDllPath, MAX_PATH);
	_tcscpy_s(_tcsrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);

	// Define and clean process information
	STARTUPINFO lpStartupInfo;
	ZeroMemory(&lpStartupInfo, sizeof(lpStartupInfo));
	lpStartupInfo.cb = sizeof(lpStartupInfo);
	PROCESS_INFORMATION lpProcInfo;
	ZeroMemory(&lpProcInfo, sizeof(lpProcInfo));

	// Fire up TrueCrypt in suspended mode
	if(!CreateProcess(szTrueCryptPath,
		lpPathEnd + 1,
		NULL,
		NULL,
		FALSE,
		CREATE_SUSPENDED|DETACHED_PROCESS,
		NULL,
		NULL,
		&lpStartupInfo,
		&lpProcInfo))
	{
		ErrorExit(_T("CreateProcess"));
	}

#ifndef _NOINJECT
	// Get address of kernel32.dll
	HMODULE hKernel32 = GetModuleHandle(_T("Kernel32"));
	if(NULL == hKernel32)
		return FALSE;

	// Open target process with required permissions
	HANDLE hProcess = OpenProcess(
		PROCESS_CREATE_THREAD | 
		PROCESS_QUERY_INFORMATION | 
		PROCESS_VM_OPERATION | 
		PROCESS_VM_WRITE | 
		PROCESS_VM_READ,
		FALSE, lpProcInfo.dwProcessId);

	if(NULL == hProcess)
		ErrorExit(_T("OpenProcess"));

	// Allocate memory in the remote process for DLL path
	LPVOID pLibRemote = VirtualAllocEx(hProcess, NULL, sizeof(szDllPath), MEM_COMMIT, PAGE_READWRITE);
	if( pLibRemote == NULL )
		ErrorExit(_T("VirtualAllocEx"));

	// Write szLibPath to the allocated memory
	if(0 == WriteProcessMemory(hProcess, pLibRemote, (void*)szDllPath, sizeof(szDllPath), NULL))
		ErrorExit(_T("WriteProcessMemory"));

	// Launch new Thread in target process and load DLL
	HANDLE hThread = CreateRemoteThread(hProcess, NULL, 0,	
		(LPTHREAD_START_ROUTINE)GetProcAddress(hKernel32, "LoadLibraryW"), pLibRemote, 0, NULL);

	DWORD hLibModule = 0;
	if( hThread != NULL )
	{	
		WaitForSingleObject( hThread, INFINITE );

		// Get handle of loaded module
		GetExitCodeThread( hThread, &hLibModule );
		if (hLibModule == NULL)
			ErrorExit(_T("LoadLibraryW in remote Thread"));
		CloseHandle( hThread );
		VirtualFreeEx(hProcess, pLibRemote, sizeof(szDllPath), MEM_RELEASE);
	}
	else
		ErrorExit(_T("CreateRemoteThread"));

	CloseHandle(hProcess);
#endif

	// Resume main thread
	if(ResumeThread(lpProcInfo.hThread) == (DWORD)-1)
		ErrorExit(_T("ResumeThread"));
	
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
