#pragma once

#include <Windows.h>
#include <d3d11.h>

#ifndef SAFE_DELETE
#define SAFE_DELETE(p) { if (p) {delete (p); (p)=NULL; } }
#endif // !SAFE_DELETE

#ifndef SAFE_DELETE_ARRAY
#define SAFE_DELETE_ARRAY(p) { if (p) {delete[] (p); (p)=NULL; } }
#endif // !SAFE_DELETE_ARRAY

#ifndef SAFE_RELEASE
#define SAFE_RELEASE(p) { if (p) { (p)->Release(); (p)=NULL; } }
#endif // !SAFE_RELEASE
