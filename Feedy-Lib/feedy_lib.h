// *************************************************************************************************
//																				
//	ez_lib.h: 
//																				
// *************************************************************************************************
//
//  This is a program that receives commands over tcp/ip and drives a eZ-Inkjet
//
//
//
// *************************************************************************************************
//
//    Copyright 2022 GRAPH-TECH-USA. All rights reserved.		
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************#pragma once

#pragma once

#include "ge_common.h"
#include "feedy_def.h"

#ifdef linux
	int ez_hex2byte   (char *str, BYTE *bytes, int len);
#else
	
	#ifdef __cplusplus
	extern "C"{
	#endif

	#define EXPORT EXTERN_C _declspec(dllexport) 

	EXPORT int ez_hex2byte   (char *str, BYTE *bytes, int len);

	#ifdef __cplusplus
	}
	#endif

#endif // linux