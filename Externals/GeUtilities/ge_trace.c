// *************************************************************************************************
//																				
//	ge_trace: 
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


//--- includes -----------------------------------------------
#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include "ge_time.h"
#include "ge_file.h"

#include <sys/types.h>
#include <sys/stat.h>
#include <sys/utime.h>
#include <direct.h>
#include <time.h>
#include <shlobj.h>

//--- statics -----------------------------------------------
static int	_TraceLevel=-1;
static int  _ScreenTrace=TRUE;
static char	_AppName[64];

static FILE *_TraceFile=NULL;

//--- TrInit -----------------------------------
void TrInit (char *appName, int level, char *path, char *logdir)
{
	if (_TraceFile) return;
	strncpy(_AppName, appName, sizeof(_AppName));
	_TraceLevel  = level;
	if (path && _TraceLevel>=0)
	{
		ge_mkdir_path(path);

		ge_remove_old_files(logdir, 30);
		struct __stat64 st;
		if (_stat64(path, &st)==0)
		{
			char dir[MAX_PATH];
			char name[MAX_PATH];
			char ext[MAX_PATH];
			ge_split_path(path, dir, name, ext);

			char time_stamp[100];
			ge_get_date_time_str(st.st_mtime, time_stamp);

			char logpath[MAX_PATH];
			sprintf(logpath, "%s%s - %s%s", logdir, name, time_stamp, ext);
			rename(path, logpath);
		}

		_TraceFile = ge_fopen(path, "w", _SH_DENYNO);
		printf("Tracing to file >>%s<<\n", path);
	}
}

//--- TrPrintf ---------------------------------------------------------
#define		TRACE_LEN	512 
void TrPrintf (int level, char *format, ...)
{
	int	len;
	char str[TRACE_LEN];

	if (level <= _TraceLevel) 
	{
		int time=ge_ticks();
		len = sprintf(str, "%03d.%03d: ", time/1000, time%1000);
		len = _vsnprintf(&str[len], TRACE_LEN-len, format, (va_list)&format + sizeof(format));
		if (len<0 || (len>TRACE_LEN-(int)strlen(_AppName)-10)) {
			memcpy(&str[TRACE_LEN-strlen(_AppName)-10], "...\n", 5);
			str[TRACE_LEN-strlen(_AppName)-10+4]=0;
		}

		if (_TraceFile!=NULL) 
		{
			fprintf(_TraceFile, str);
			fprintf(_TraceFile, "\n");
			fflush(_TraceFile);
		}
		if (_ScreenTrace)
		{
			#ifdef OutputDebugStringA
						OutputDebugStringA(str);
						OutputDebugStringA("\n");
			#else
						printf(str);
						printf("\n");
			#endif	
		}
	}
} /* end TrPrintf */