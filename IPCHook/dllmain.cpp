#include <Windows.h>
#include "NCodeHookInstantiation.h"
#include <string>
using namespace std;

#pragma comment(lib, "distorm.lib")

// Pointer to original function
typedef int (WINAPI *MessageBoxFPtr)(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType);

// Definitions
MessageBoxFPtr origMessageBoxW = NULL;
NCodeHookIA32 nch;
volatile BOOL bHooked = FALSE;

// The fake function
int WINAPI MessageBoxHook(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType)
{
	wstring buffer(lpText);
	HANDLE npipe;

	// Send test of message box back to parent process
	if( WaitNamedPipe(L"\\\\.\\pipe\\TrueCryptMessage", 3000) )
	{
		npipe = CreateFile(L"\\\\.\\pipe\\TrueCryptMessage",
			GENERIC_READ | GENERIC_WRITE,
			0, NULL, OPEN_EXISTING, 0, NULL);
		if( npipe != INVALID_HANDLE_VALUE )
		{
			DWORD dwRead;
			WriteFile(npipe, (LPCVOID)&buffer[0], sizeof(wchar_t) * buffer.size(), &dwRead, NULL);
			CloseHandle(npipe);
		}
	}

	// We can't do more, terminate
	TerminateProcess(GetCurrentProcess(), ERROR_SUCCESS);

	return 0;
}

BOOL APIENTRY DllMain(_In_ void * _HDllHandle, _In_ unsigned _Reason, _In_opt_ void * _Reserved)
{
	switch(_Reason)
	{
	case DLL_PROCESS_ATTACH:
		origMessageBoxW = nch.createHook(MessageBoxW, MessageBoxHook);

		if(origMessageBoxW == NULL)
			MessageBeep(1);
		break;
	case DLL_PROCESS_DETACH:
		nch.removeHook(MessageBoxHook);
		break;
	}

	return TRUE;
}