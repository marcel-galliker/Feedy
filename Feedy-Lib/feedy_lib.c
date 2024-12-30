// *************************************************************************************************
//																				
//	ez_lib.c: 
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

#include <stdarg.h>
#include "feedy_lib.h"
#include "ge_file.h"

static BYTE _val(char ch, int *perror)
{
	switch(ch)
	{
	case '0': return 0x00;
	case '1': return 0x01;
	case '2': return 0x02;
	case '3': return 0x03;
	case '4': return 0x04;
	case '5': return 0x05;
	case '6': return 0x06;
	case '7': return 0x07;
	case '8': return 0x08;
	case '9': return 0x09;
	case 'A': return 0x0a;
	case 'B': return 0x0b;
	case 'C': return 0x0c;
	case 'D': return 0x0d;
	case 'E': return 0x0e;
	case 'F': return 0x0f;
	default:  *perror = REPLY_ERROR; return 0;
	}
}

//--- ez_hex2byte -----------------------------------
int ez_hex2byte   (char *str, BYTE *bytes, int len)
{
	int l=(int)strlen(str);
//	if (l!=2*len) return REPLY_ERROR;
	int reply=REPLY_OK;
	int i;
	for (i=0; i<len && reply==REPLY_OK; i++)
	{
		bytes[i] = (_val(*str++, &reply) <<4) | _val(*str++, &reply);
	}
	return reply;
}

//--- Error ------------------------------------------------------------------------
int Error(int type, const char *file, int line, int errNo, char *format, ...)
{
	va_list arglist;
	va_start(arglist, format);
	vprintf(format, arglist);
	va_end(arglist);
	return 0;
}

