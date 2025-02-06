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

#include <stdio.h>
#define REPLY_OK	0
#define REPLY_ERROR	1
#define FALSE    0
#define TRUE     1

#ifdef linux

    #include <stdlib.h>
    #include <string.h>
    #include <time.h>

    #define MAX_PATH 256

    typedef signed char         UCHAR, *PUCHAR;

    typedef signed char         INT8, *PINT8;
    typedef signed char         BYTE, *PBYTE;
    typedef signed short        INT16, *PINT16;
    typedef signed int          INT32, *PINT32;

    typedef unsigned char       UINT8, *PUINT8;
    typedef unsigned short      UINT16, *PUINT16;
    typedef unsigned int        UINT32, *PUINT32;
    #define INT64               int64_t
    #define UINT64              int64_t

    #define FILETIME            INT64

    typedef void*               HANDLE;

    #define REC_SEP "/"
#else
	#include <io.h>
	#include <Windows.h>
    #define REC_SEP "\\"

    typedef signed char         INT8, *PINT8;
    typedef signed short        INT16, *PINT16;
    typedef signed int          INT32, *PINT32;
    typedef signed __int64      INT64, *PINT64;
    typedef unsigned char       UINT8, *PUINT8;
    typedef unsigned short      UINT16, *PUINT16;
    typedef unsigned int        UINT32, *PUINT32;
    typedef unsigned __int64    UINT64, *PUINT64;

#endif

#define STR_32 32
#define STR_64 64
#define STR_256 256

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
