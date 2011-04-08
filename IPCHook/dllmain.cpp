#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include "NCodeHook/NCodeHookInstantiation.h"

#pragma comment(lib, "NCodeHook/distorm.lib")

// Dummy export
__declspec(dllexport) void dummyExport() {}

// Pointer to original function
typedef int (WINAPI *MessageBoxFPtr)(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType);

// Definitions
MessageBoxFPtr origMessageBoxW = NULL;
NCodeHookIA32 nch;
volatile BOOL bHooked = FALSE;

// The fake function
int WINAPI MessageBoxHook(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType)
{
#ifdef _DEBUG
	origMessageBoxW(NULL, L"Hooked", NULL, MB_ICONINFORMATION|MB_OK);
#endif

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
			WriteFile(npipe, (LPCVOID)lpText, wcslen(lpText) * sizeof(TCHAR), &dwRead, NULL);
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
#ifdef _DEBUG
		MessageBox(NULL, L"HOOKED", L"MAIN", MB_ICONINFORMATION|MB_OK);
#endif
		origMessageBoxW = nch.createHook(MessageBoxW, MessageBoxHook);

		if(origMessageBoxW == NULL)
		{
#ifdef _DEBUG
			MessageBox(NULL, L"Hook went wrong!", L"CRAP", MB_ICONINFORMATION|MB_OK);
#endif
			HANDLE npipe;
			LPWSTR lpMsg = L"ERROR_HOOK";
			// Send fail message back to parent process
			if( WaitNamedPipe(L"\\\\.\\pipe\\TrueCryptMessage", 3000) )
			{
				npipe = CreateFile(L"\\\\.\\pipe\\TrueCryptMessage",
					GENERIC_READ | GENERIC_WRITE,
					0, NULL, OPEN_EXISTING, 0, NULL);
				if( npipe != INVALID_HANDLE_VALUE )
				{
					DWORD dwRead;
					WriteFile(npipe, (LPCVOID)lpMsg, sizeof(wchar_t) * wcslen(lpMsg), &dwRead, NULL);
					CloseHandle(npipe);
				}
			}

			// We can't do more, terminate
			TerminateProcess(GetCurrentProcess(), ERROR_SUCCESS);
		}

#ifdef _DEBUG
		origMessageBoxW(NULL, L"HOOKED2", L"RLY", MB_OK|MB_ICONINFORMATION);
#endif
		break;
	case DLL_PROCESS_DETACH:
		nch.removeHook(MessageBoxHook);
		break;
	}

	return TRUE;
}