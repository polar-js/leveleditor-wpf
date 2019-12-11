// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" {
    __declspec(dllexport) HRESULT __cdecl Init();
}

extern "C" {
    __declspec(dllexport) HRESULT __cdecl Cleanup();
}

extern "C" {
    __declspec(dllexport) HRESULT __cdecl BeginScene();
}

extern "C" {
    __declspec(dllexport) HRESULT __cdecl EndScene();
}

extern "C" {
    __declspec(dllexport) HRESULT __cdecl SubmitQuad();
}