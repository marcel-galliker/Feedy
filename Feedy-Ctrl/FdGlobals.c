// *************************************************************************************************
//																				
//	FdGlobals.c: 
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
#include "ge_file.h"
#include "ge_utilities.h"
#include "FdGlobals.h"

static void no_abort(void){return;}

//--- global variables ---------------------------------------------------
char PATH_ROOT[4];
char PATH_TEMP[MAX_PATH];


SConfig			FeedyCfg;
SAppData		FeedyAppData;
SJob			FeedyJob;
SFeedyStatus	FeedyStatus;

//--- FdGlobals_init ---------------------------------
void FdGlobals_init(void)
{
	char dir[MAX_PATH];
	GetCurrentDirectoryA(MAX_PATH, dir);
	if (strstart(dir, "\\\\"))
	{
		strcpy(PATH_ROOT, "D:\\");
	}
	else
	{
		strncpy(PATH_ROOT, dir, 3);
	}
	sprintf(PATH_TEMP, "%sTemp\\", PATH_ROOT);

	sprintf(dir, "%s"PATH_JOBS, PATH_ROOT);
	ge_mkdir_path(dir);
	sprintf(dir, "%s"PATH_LOGS, PATH_ROOT);
	ge_mkdir_path(dir);
	sprintf(dir, "%s"PATH_SETTINGS, PATH_ROOT);
	ge_mkdir_path(dir);
}

//--- EzUserFilePath --------------------------------------------------------
char * EzUserFilePath(const char* directory, const char* filename, const char *ext, char path[MAX_PATH])
{
//	ge_user_path(path);
	strcpy(path, PATH_ROOT);
	int len=(int)strlen(path);
	len += sprintf(&path[len], directory);
	if (filename!=NULL)
		len+=sprintf(&path[len], "%s", filename);
	if (ext!=NULL)
	{
		if (!strend((char*)path, (char*)ext))
			len+=sprintf(&path[len], ".%s", ext);
	}
	return path;
}

//--- FeedyJobFilePath --------------------------------------------------------
char * FeedyJobFilePath(const char* jobname, const char* filename, const char *ext, char path[MAX_PATH])
{
	//	ge_user_path(path);
	strcpy(path, PATH_ROOT);
	int len=(int)strlen(path);
	len += sprintf(&path[len], PATH_JOBS);
	if (jobname!=NULL)  len+=sprintf(&path[len], "%s\\", jobname);
	if (filename!=NULL) len+=sprintf(&path[len], "%s", filename);
	if (ext!=NULL)
	{
		if (!strend((char*)path, (char*)ext))
		{
			if (filename!=NULL) len+=sprintf(&path[len], ".%s", ext);
			else                len+=sprintf(&path[len], "*.%s", ext);
		}
	}
	return path;
}