// *************************************************************************************************
//																				
//	FdGlobals.h: 
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

//--- includes -----------------------------------------------------------
#include "feedy_def.h"
#include "feedy.h"

//--- defines -------------------------------------------------------------
// #define PATH_ROOT		"D:\\"
extern  char PATH_ROOT[4];
extern  char PATH_TEMP[MAX_PATH];
#define PATH_APP		"Feedy\\"
#define PATH_SETTINGS	PATH_APP "Settings\\"
#define PATH_JOBS		PATH_APP "Jobs\\"
#define PATH_LOGS		PATH_APP "Logs\\"

#define APP_DATA_FNAME	"AppData.xml"

//--- laylout ---------------------------

//--- diskread -------------------------------
//#define MAX_FIELDS		64
#define MAX_FIELD_LEN	512
#define MAX_RECORD_SIZE	2048

//=== structures ========================================================

//--- SAppData ------------------------------------------------
typedef struct SAppData
{
	char jobName[STR_64];
} SAppData;

//--- global variables ---------------------------------------------------
extern  SConfig			FeedyCfg;
extern  SAppData		FeedyAppData;
extern	SJob			FeedyJob;
extern  SFeedyStatus FeedyStatus;

//--- functions --------------------------------------------------------
void FdGlobals_init(void); 
char * EzUserFilePath(const char* directory, const char* filename, const char *ext, char path[MAX_PATH]);
char * FeedyJobFilePath(const char* jobname, const char* filename, const char *ext, char path[MAX_PATH]);
