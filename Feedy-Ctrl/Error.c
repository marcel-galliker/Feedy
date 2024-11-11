// *************************************************************************************************
//																				
//	Error.c: 
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

//--- includes ---------------------------
#include <stdarg.h>
#include <windows.h>
#include <stdlib.h>
#include "ge_common.h"
#include "ge_file.h"
#include "ge_time.h"
#include "ge_trace.h"
#include "FdTcpIp.h"
#include "FdGlobals.h"
#include "gui_server.h"
#include "feedy.h"
#include "Error.h"

#define SEP	";"

//--- statics ----------------------------------------------------
static char _LogDate[32]="";
static FILE *_LogFile = NULL;

//--- _ParseMsg ---------------------------------------------------
static int _ParseMsg(char *format, va_list arglist, char *msg, char *args)
{
	int  argNo = 0;
	int  msgLen=0;
	int  argLen=0;
	char *pch = format;
	char *pfmt=NULL;

	for(pch = format; *pch || msgLen+1>=MAX_PATH; pch++)
	{
		if (pfmt)
		{
			if (*pch>'A')
			{
				msgLen += snprintf(&msg[msgLen], MAX_PATH-msgLen-1, "{%d}", argNo++);
				char fmt[32];
				memset(fmt, 0, sizeof(fmt));
				memcpy(fmt, pfmt, pch-pfmt+1);
				#pragma warning(suppress: 6054)
				argLen += vsprintf(&args[argLen], fmt, arglist);
				args[argLen++]=*SEP;
				pfmt = NULL;
				if(fmt[strlen(fmt)-1]=='f') va_arg(arglist , double);
				else						va_arg(arglist , int*);
			}
		}
		else if (*pch == '%') pfmt=pch;
		else msg[msgLen++]=*pch;
	}
	return msgLen;
}

//--- Error_Once ----------------------------------------
int Error_Once(INT64 *perr, int bit)
{
	INT64 mask= (INT64)0x01<<bit;
	int err = !(*perr & mask);
	*perr |= mask;
	return err;
}

//--- Error ------------------------------------------------------------------------
int Error(int type, const char *file, int line, int errNo, char *format, ...)
{
	va_list arglist;
	char typestr[16];
	char msg[2*MAX_PATH];
	char args[MAX_PATH];
	const char *filename=file;
	int msgLen=0;

	memset(msg,  0, sizeof(msg));
	memset(args, 0, sizeof(args));

	switch(type)
	{
	case ET_UNDEF: break;
	case ET_LOG:  strcpy(typestr, "LOG");   break;
	case ET_WARN: strcpy(typestr, "WARN");  break;
	default:	  strcpy(typestr, "ERR"); break;
	}
	
	va_start(arglist, format);
	vsprintf(msg, format, arglist);
	TrPrintf(-1, "%s: %s [%s:%d]", typestr, msg, filename, line);

	msgLen = ge_get_system_date_str(msg, '-');
	msg[msgLen++]='@';
	msgLen += ge_get_system_time_str(&msg[msgLen]);
	for (const char *pch=file; *pch; pch++)
		if (*pch=='\\') filename=pch+1;


	msgLen += sprintf(&msg[msgLen], SEP "%s" SEP "%s" SEP "%d" SEP "%d" SEP, typestr, filename, line, errNo);
	msgLen += _ParseMsg(format, arglist, &msg[msgLen], args);
	msgLen += sprintf(&msg[msgLen], SEP "%s", args);
	va_end(arglist);

	Error_Send(msgLen, msg);

	if (type==ET_ERROR_ABORT) feedy_abort(INVALID_SOCKET);

	return REPLY_ERROR;
}

//--- Error_Send ---------------------------------------
int Error_Send(int msgLen, char* msg)
{
	char date[32];

	ge_get_system_date_str(date, '-');
	//--- open log ---------------------------------
	if (strcmp(date, _LogDate))
	{
		if (_LogFile!=NULL) fclose(_LogFile);
		_LogFile=NULL;
	}
	if (_LogFile==NULL)
	{
		char path[MAX_PATH];
		//--- recycle old files ---
		EzUserFilePath(PATH_LOGS, NULL, NULL, path);
		ge_recycle_old_files(path, 30);

		//--- open new file ---
		EzUserFilePath(PATH_LOGS, date, "csv", path);
		ge_mkdir_path(path);
		_LogFile = ge_fopen(path, "a", _SH_DENYNO);
		strcpy(_LogDate, date);

		fseek(_LogFile, 0, SEEK_END);
		int pos=ftell(_LogFile);
		if (pos==0)
		{
			fprintf(_LogFile, "Time" SEP "Type" SEP "File" SEP "Line" SEP "ErrNo" SEP "Message" SEP "Arg" "\n");
		}
	}

	//--- write to file -----------------------------
	if (_LogFile!=NULL)
	{
		fprintf(_LogFile, "%s\n", msg);
		fflush(_LogFile);
	}

	//--- send error ---------------------------------
	{
		SGuiMsg errMsg;
		memset(&errMsg, 0, sizeof(errMsg));
		errMsg.hdr.msgLen = sizeof(errMsg.hdr) + msgLen;
		errMsg.hdr.msgId  = ERROR_EVENT;
		memcpy(errMsg.data, msg, msgLen);

		gui_send(INVALID_SOCKET, &errMsg);
	}
	return REPLY_ERROR;
}

//--- err_reset ------------------------------------------
void err_reset(void)
{
	SMsgHdr hdr;
	hdr.msgLen = sizeof(hdr);
	hdr.msgId  = ERROR_RESET;
	gui_send(INVALID_SOCKET, &hdr);
}