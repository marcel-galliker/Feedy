// *************************************************************************************************
//																				
//	Error.h: 
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
// *************************************************************************************************

#pragma once

//--- includes ---------------------------
#include "rt_sok_api.h"

//--- defines ----------------------------

#define LOG			ET_LOG,			__FILE__,__LINE__
#define WARN		ET_WARN,		__FILE__,__LINE__
#define ERR_CONT	ET_ERROR_CONT,	__FILE__,__LINE__
#define ERR_STOP	ET_ERROR_STOP,	__FILE__,__LINE__
#define ERR_ABORT	ET_ERROR_ABORT,	__FILE__,__LINE__

typedef enum EErrorTpeET{
	ET_UNDEF,
	ET_LOG,			// "LOG"
	ET_WARN,		// "WRN"
	ET_ERROR_CONT,	// "ERR_C":	Red message but continue printing
	ET_ERROR_STOP,	// "ERR_S":	Red message and stop production (disable PD)
	ET_ERROR_ABORT	// "ERR_A": Red message and stop immediately
} EErrorType;

//--- prototypes -------------------------------------------
void err_reset(void);
int  Error_Once(INT64 *perr, int bit);
int	 Error(int type, const char *file, int line, int errNo, char *format, ...);
int  Error_Send(int msgLen, char* msg);
void Error_SendLogFName(SOCKET socket);


