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
#include "EzDef.h"

#define EXPORT EXTERN_C _declspec(dllexport) 

#ifdef __cplusplus
extern "C"{
#endif

EXPORT int ez_hex2byte   (char *str, byte *bytes, int len);

#ifdef __cplusplus
}
#endif