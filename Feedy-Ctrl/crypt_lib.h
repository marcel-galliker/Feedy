#pragma once

#ifdef linux
#else
	#define EXPORT EXTERN_C _declspec(dllexport) 

	#include "ge_common.h"

	#ifdef __cplusplus
	extern "C"{
	#endif

	EXPORT int encrypt(byte *buffer, int blen, byte *crypt, int clen);
	EXPORT int decrypt(byte *crypt, int clen, byte *buffer, int blen);

	#ifdef __cplusplus
	}
	#endif
#endif