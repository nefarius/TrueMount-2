#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <string>
using namespace std;
#include "NInjectLib/IATModifier.h"

VOID ErrorExit(LPTSTR lpszFunction, BOOL bExit = TRUE);

int WINAPI WinMain( __in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in LPSTR lpCmdLine, __in int nShowCmd )
{
	// Without arguments no work
	if(strlen(lpCmdLine) <= 0)
		return ERROR_BAD_ARGUMENTS;

	// The name and path of the IPC Hook DLL
	LPSTR szDllName = "IPCHook.dll";
	CHAR szDllPath[MAX_PATH];

	// Extract the path to TrueCrypt
	TCHAR szTrueCryptPath[MAX_PATH];
	LPTSTR lpPathBegin = strchr(lpCmdLine, '"') + 1;
	LPTSTR lpPathEnd = strchr(lpPathBegin, '"');
	int iLength = lpPathEnd - lpPathBegin;
	strncpy_s(szTrueCryptPath, sizeof(szTrueCryptPath), lpPathBegin, iLength);
#ifdef _DEBUG
	MessageBox(NULL, lpPathEnd + 1, "TCArgs", MB_ICONINFORMATION|MB_OK);
#endif

	// Get absolute DLL location
	GetModuleFileName(hInstance, szDllPath, MAX_PATH);
	strcpy_s(strrchr(szDllPath, '\\') + 1, sizeof(szDllPath), szDllName);

	// Get Operating System Version
	OSVERSIONINFO osVer;
	ZeroMemory(&osVer, sizeof(osVer));
	osVer.dwOSVersionInfoSize = sizeof(osVer);
	if(!GetVersionEx(&osVer))
		ErrorExit("GetVersionEx");

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
			iatModifier.writeIAT(szDllPath);
			ResumeThread(lpProcInfo.hThread);
		}
		catch (std::exception& e)
		{
#ifdef _DEBUG
			MessageBox(NULL, "Injection failed!", "Fail", MB_ICONERROR|MB_OK);
#endif
			ResumeThread(lpProcInfo.hThread);
			CloseHandle(lpProcInfo.hThread);
			CloseHandle(lpProcInfo.hProcess);
		}
	}
	else
		ErrorExit("CreateProcess");

	// Wait till process exited
	WaitForSingleObject(lpProcInfo.hProcess, INFINITE);
	// Cleanup
	CloseHandle(lpProcInfo.hProcess);
	CloseHandle(lpProcInfo.hThread);

	// Send an OK message back home
	HANDLE npipe;
	if( WaitNamedPipe("\\\\.\\pipe\\TrueCryptMessage", 3000) )
	{
		npipe = CreateFile("\\\\.\\pipe\\TrueCryptMessage",
			GENERIC_READ | GENERIC_WRITE,
			0, NULL, OPEN_EXISTING, 0, NULL);
		if( npipe != INVALID_HANDLE_VALUE )
		{
			DWORD dwRead;
			WriteFile(npipe, (LPCVOID)L"OK", sizeof(WCHAR) * 2, &dwRead, NULL);
			CloseHandle(npipe);
		}
		else
			ErrorExit("CreatePipe");
	}

	return ERROR_SUCCESS;
}

VOID ErrorExit(LPSTR lpszFunction, BOOL bExit)
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
		(lstrlen((LPCSTR)lpMsgBuf) + lstrlen((LPCSTR)lpszFunction) + 40) * sizeof(CHAR));
	sprintf_s((LPSTR)lpDisplayBuf, 
		LocalSize(lpDisplayBuf) / sizeof(CHAR), 
		"%s failed with error %d: %s", 
		lpszFunction, dw, lpMsgBuf); 
	MessageBox(NULL, (LPCSTR)lpDisplayBuf, "Error", MB_OK|MB_ICONERROR); 

	// Clean up
	LocalFree(lpMsgBuf);
	LocalFree(lpDisplayBuf);

	if(bExit)
		ExitProcess(dw);
}
