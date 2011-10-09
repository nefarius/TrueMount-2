#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include "NCodeHook/NCodeHookInstantiation.h"
#include <string>

#pragma comment(lib, "NCodeHook/distorm.lib")

// Dummy export
__declspec(dllexport) void dummyExport() {}

// Pointer to original function
typedef int (WINAPI *MessageBoxFPtr)(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType);

// Definitions
MessageBoxFPtr origMessageBoxW = NULL;
NCodeHookIA32 nch;

// The fake function
int WINAPI MessageBoxHook(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType)
{
	HANDLE npipe;
	// Convert everything to UNICODE string
	std::wstring text(lpText);
	std::wstring caption(lpCaption);

	// Send test of message box back to parent process
	if( WaitNamedPipe(L"\\\\.\\pipe\\TrueCryptMessage", 3000) )
	{
		npipe = CreateFile(L"\\\\.\\pipe\\TrueCryptMessage",
			GENERIC_READ | GENERIC_WRITE,
			0, NULL, OPEN_EXISTING, 0, NULL);
		if( npipe != INVALID_HANDLE_VALUE )
		{
			DWORD dwRead;
			// Send dialog box message text
			WriteFile(npipe, 
				(LPCVOID)&text[0], 
				text.size() * sizeof(wchar_t), 
				&dwRead, 
				NULL);
			// Send caption
			WriteFile(npipe,
				(LPCVOID)&caption[0],
				caption.size() * sizeof(wchar_t),
				&dwRead,
				NULL);
			// Send message box type
			WriteFile(npipe,
				(LPCVOID)&uType,
				sizeof(UINT),
				&dwRead,
				NULL);

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
		{
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
		break;
	case DLL_PROCESS_DETACH:
		nch.removeHook(MessageBoxHook);
		break;
	}

	return TRUE;
}