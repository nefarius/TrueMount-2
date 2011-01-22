#include <Windows.h>
#include "NCodeHookInstantiation.h"

#pragma comment(lib, "distorm.lib")

typedef int (WINAPI *MessageBoxFPtr)(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType);

#pragma data_seg("SHARED")
MessageBoxFPtr origMessageBoxW = NULL;
NCodeHookIA32 nch;
volatile BOOL bHooked = FALSE;
#pragma data_seg()

#pragma comment(linker, "/section:SHARED,RWS")


int WINAPI MessageBoxHook(HWND hWnd, LPCTSTR lpText, LPCTSTR lpCaption, UINT uType)
{
	HANDLE npipe;

	if( WaitNamedPipe(L"\\\\.\\pipe\\TrueCryptMessage", 3000) )
	{
		npipe = CreateFile(L"\\\\.\\pipe\\TrueCryptMessage",
			GENERIC_READ | GENERIC_WRITE,
			0, NULL, OPEN_EXISTING, 0, NULL);
		if( npipe != INVALID_HANDLE_VALUE )
		{
			DWORD dwRead;
			WriteFile(npipe, (LPCVOID)&lpText, sizeof(lpText), &dwRead, NULL);
			CloseHandle(npipe);
		}
	}

	return origMessageBoxW(NULL, L"Lulz", L"Hai", MB_ICONINFORMATION|MB_OK);

	if(origMessageBoxW != NULL)
		return origMessageBoxW(hWnd, lpText, L"TrueMount", uType);

	SetLastError(ERROR_HOOK_NOT_INSTALLED);
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