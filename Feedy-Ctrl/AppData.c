// *************************************************************************************************
// *************************************************************************************************
//																				
//	AppData.c: 
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

//--- includes ------------------------------------------------------

#include "ge_common.h"
#include "ge_file.h"
#include "ge_xml_file.h"
#include "FdGlobals.h"
#include "AppData.h"


//--- prpototypes ----------------------------------------------
static int _xml_appdata(char *jobName, EN_Xml_Action action);

//--- appdata_load --------------------------------
void appdata_load(char *jobName)
{
	_xml_appdata(jobName, READ);
}

//--- appdata_save --------------------------------
void appdata_save(char *jobName)
{
	if (*jobName) _xml_appdata(jobName, WRITE);
}

//--- _xml_appdata ----------------------------------
static int _xml_appdata(char *jobName, EN_Xml_Action action)
{
	HANDLE file = xml_create();
	char path[MAX_PATH];

//	ge_appdata_path(path, "EZ-PrintEngine", "settings.xml");
	sprintf(path, "%s%s%s", PATH_ROOT, PATH_SETTINGS, APP_DATA_FNAME);

	xml_load(file, path);

	if (xml_chapter(file, "AppData", -1, action)==REPLY_OK)
	{
		xml_str		(file,	"jobName", action, jobName, sizeof(EzAppData.jobName), "");
		xml_chapter	(file, "..", -1, action);
	}
	xml_chapter(file, "..", -1, action);
	if (action==WRITE)
	{
		ge_mkdir_path(path);
		xml_save(file, path);
	}
	xml_destroy(file);
	return REPLY_OK;
}
