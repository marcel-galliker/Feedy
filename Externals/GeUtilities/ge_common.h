// ****************************************************************************
//
//	ge_common.h
//
// ****************************************************************************
//
//	Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************

#pragma once


#define SIZEOF(arr) (sizeof(arr) / sizeof(*(arr)))

#ifdef linux
	#include <stdio.h>
#else
	#include <io.h>
	#include <stdio.h>
	#include <Windows.h>
#endif

#define STR_32 32
#define STR_64 64
#define STR_256 256


#define REPLY_OK	0
#define REPLY_ERROR	1

//--- SBmpInfo ------------------------------------
typedef struct SBmpInfo
{
    INT32 heightPx;
    INT32 widthPx;
    INT32 bitsPerPixel;
    INT32 dpiX;
    INT32 dpiY;
    BYTE  *buffer;
    INT32 bufSize;
    INT32 dataSize;
    INT32 lineLen;		// in bytes
} SBmpInfo;
