// *************************************************************************************************
//																				
//	job.c: 
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

//--- includes ------------------------------------------------------
#include "ge_common.h"
#include "ge_file.h"
#include "ge_xml_file.h"
#include "AppData.h"
#include "FdGlobals.h"
#include "error.h"
#include "gui_server.h"
#include "xml_lib.h"
#include "job.h"

//--- static variables --------------------------------------------
static SJob		_SaveJob;

//--- job_list -------------------------------------------------
void job_list(SOCKET socket)
{
	SFileMsg msg;
	char path[MAX_PATH];

	msg.hdr.msgLen = sizeof(msg.hdr);
	msg.hdr.msgId  = LIST_JOB_START;
	gui_send(socket, &msg);

	FeedyJobFilePath(NULL, NULL, NULL, path);

	{
		msg.hdr.msgLen = sizeof(msg);
		msg.hdr.msgId  = LIST_JOB_FILE;
		HANDLE search;
		UINT64 time;
		UINT32 size, isDir;

		search = ge_search_open(path, "*");
		while (ge_search_next(search, msg.filename, sizeof(msg.filename), &time, &size, &isDir))
		{
			if (isDir && msg.filename[0]!='.') gui_send(socket, &msg);
		}			
	}
	
	msg.hdr.msgLen = sizeof(msg.hdr);
	msg.hdr.msgId  = LIST_JOB_END;
	gui_send(socket, &msg);	
}

//--- job_load_file --------------------------
void job_load_file	(char *filename)
{
	SFileMsg msg;
	strcpy(msg.filename, filename);
	job_load(INVALID_SOCKET, &msg);
}

//--- job_load ----------------------------------------------
void job_load(SOCKET socket, SFileMsg *pmsg)
{
	char	path[MAX_PATH];
	SJob	job;

//	Error(LOG, 0, "job_load >>%s<<", pmsg->filename);
	if (*pmsg->filename==0) strcpy(pmsg->filename, FeedyAppData.jobName);
	if (*pmsg->filename==0) return;

	FeedyJobFilePath(pmsg->filename, pmsg->filename, "xml", path);
	
	if (xml_job_file(path, &job, READ)!=REPLY_OK)
	{
		Error(ERR_ABORT, 10, "Job >>%s<< not found", pmsg->filename);
		return;
	}
	
	//--- JOB ----------------------------------------
	{
		SJobMsg msg;
		memset(&msg, 0, sizeof(msg));
		msg.hdr.msgLen = sizeof(msg);
		msg.hdr.msgId  = LOAD_JOB;
		memcpy(&msg.job, &job, sizeof(msg.job));
		gui_send(socket, &msg);
	}
}

//--- job_save---------------------------------------
void job_save	(SOCKET socket, SJobMsg *pmsg)
{
	char path[MAX_PATH];
	strcpy(FeedyAppData.jobName, pmsg->job.name);
	appdata_save(pmsg->job.name);
	FeedyJobFilePath(pmsg->job.name, pmsg->job.name, "xml", path);

	xml_job_file(path, &pmsg->job, WRITE);

	{
		SFileMsg msg;
		msg.hdr.msgLen = sizeof(msg);
		msg.hdr.msgId  = LIST_JOB_FILE;
		strncpy(msg.filename, _SaveJob.name, sizeof(msg.filename));
		gui_send(INVALID_SOCKET, &msg);
	}
}
