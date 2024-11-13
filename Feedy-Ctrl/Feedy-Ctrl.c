// *************************************************************************************************
//																				
//	eZ_Server: receive commands over tcp/ip and drive the eZ-Inkjet
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
// *************************************************************************************************

//	Shortcut for autostart:			%AppData%\Microsoft\Windows\Start Menu\Programs\Startup

// Compile: Debug   - Win32
// Compile: Release - Win32 


// ---------- INCLUDES ----------------------------------------------------------------------------
#include <stdio.h>	
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include "args.h"
#include "rt_sok_api.h"
#include "ge_utilities.h"
#include "ge_trace.h"
#include "FdGlobals.h"
#include "AppData.h"
#include "error.h"
#include "version.h"
#include "gui_server.h"

// ---------- DEFINES -----------------------------------------------------------------------------
#define MAX_RECORD_LEN		250
#define RECS				10
#define MIC_REM_NAME_LEN	500

enum COMMstatus {
	MET_NO_CONN,			//0, not connected
	MET_ERROR_CONN,			//1, error connecting, try agin
	MET_CONNECTED,			//2, connected
	MET_START_ENGINE,		//3, starting engine
	MET_ENGINE_STARTED,		//4, engine started, production can begin
};

// ---------- STRUCTURES --------------------------------------------------------------------------


// ---------- GLOBAL VARIABLES --------------------------------------------------------------------


// ----------- PROTOTYPES -------------------------------------------------------------------------

// ----------- PROGRAM -----------------------------------------------------------------------------
int		main(int argc, char *argv[])
{
	args_init(argc, argv);

	FdGlobals_init();
	{
		char path[MAX_PATH];
		char logdir[MAX_PATH];
		sprintf(path, "%s%sEZ-PrintEngine.txt", PATH_TEMP, PATH_APP);
		printf("TrInit(%d, %s)\n", arg_trace, path);
		TrInit("EZ-PrintEngine", arg_trace, path, EzUserFilePath(PATH_LOGS, NULL, NULL, logdir));
	}
	TrPrintf(0, "Ez-PrintEngine %s  %s", __DATE__, __TIME__);
	TrPrintf(0, "Version %s\n", EzVersion);
	Error(LOG, 0, "Ez-PrintEngine Starting");

	SetPriorityClass(GetCurrentProcess(), HIGH_PRIORITY_CLASS);

	feedy_init();

	appdata_load(FeedyAppData.jobName);
	feedy_load_job(FeedyAppData.jobName);

	gui_server();

	return 0;
}
