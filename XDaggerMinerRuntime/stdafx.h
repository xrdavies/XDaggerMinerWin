#pragma once


#ifdef RUNTIME_EXPORTS
#define NATIVE_LIB_EXPORT __declspec(dllexport)
#else
#define NATIVE_LIB_EXPORT __declspec(dllimport)
#endif
