// *************************************************************************************************
//																				
//	xml_lib.c: 
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

#include "ge_file.h"
#include "ge_xml_file.h"
#include "xml_lib.h"


//--- prototypes ----------------------
static int _xml_job(HANDLE file, SJob *pjob, EN_Xml_Action action);

//--- xml_job_file -----------------------------------
int xml_job_file (char *filepath, SJob *pJob, EN_Xml_Action action)
{
	HANDLE file = xml_create();
	char path[MAX_PATH];
	char name[MAX_PATH];
	char ext[10];

	ge_split_path(filepath, path, name, ext);
	if (!*name) return REPLY_ERROR;
	sprintf(&path[strlen(path)], "%s.xml", name);

	if (action==READ)
	{
		memset(pJob, 0, sizeof(SJob));
	}

	xml_load(file, path);
	_xml_job(file, pJob, action);
	strcpy(pJob->name, name);
	if (action==WRITE) 
	{
		ge_mkdir_path(path);
		xml_save(file, path);
	}
	xml_destroy(file);
	
	return REPLY_OK;
}

//--- _xml_job ----------------------------------
static int _xml_job(HANDLE file, SJob *pjob, EN_Xml_Action action)
{
	if (xml_chapter(file, "Job", -1, action)==REPLY_OK)
	{
		xml_str		(file,	"description",	 action, pjob->description,		sizeof(pjob->description),      "");
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- xml_last_job ------------------
int xml_last_job(char *filepath, char *jobName, int strlen, EN_Xml_Action action)
{
	HANDLE file = xml_create();

	xml_load(file, filepath);

	if (xml_chapter(file, "AppData", -1, action)==REPLY_OK)
	{
		xml_str		(file,	"jobName", action, jobName, strlen, "");
		xml_chapter	(file, "..", -1, action);
	}
	xml_chapter(file, "..", -1, action);
	if (action==WRITE)
	{
		ge_mkdir_path(filepath);
		xml_save(file, filepath);
	}
	xml_destroy(file);

	return REPLY_OK;
}

//--- xml_cfg_file ----------------------
int xml_cfg_file   (char *filepath, SConfig *pcfg, EN_Xml_Action action)
{
	HANDLE file = xml_create();

	xml_load(file, filepath);
	if (action==READ)
	{
		memset(pcfg, 0, sizeof(SConfig));
	}
	if (xml_chapter(file, "Printer", -1, action)==REPLY_OK)
	{	
		xml_chapter(file, "..", -1, action);
	}

	if (action==WRITE)
		xml_save(file, filepath);
	xml_destroy(file);

	return REPLY_OK;
}
