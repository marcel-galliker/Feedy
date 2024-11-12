// *************************************************************************************************
//																				
//	printer.c: 
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
#include "ge_time.h"
#include "args.h"
#include "AppData.h"
#include "FdGlobals.h"
#include "error.h"
#include "config.h"
#include "job.h"
#include "feedy.h"
#include "gui_server.h"
#include "ge_trace.h"

//--- static variables --------------------------------------------

static  HANDLE		_FeedyThreadHandle=NULL;

static int			_ErrorFlag;

//--- prototypes ---------------------------------
static void _FeedyThread (void *par);

//--- feedy_init --------------------------------
void feedy_init(void)
{
	_ErrorFlag = 0;
}

//--- feedy_reset_error -------------------------------
void feedy_reset_error()
{
	_ErrorFlag = 0;
}

//--- feedy_load_job ---------------------------------------
void feedy_load_job(char *jobName)
{
	strcpy(FeedyAppData.jobName, jobName);
	appdata_save(FeedyAppData.jobName);
	job_load_file(FeedyAppData.jobName);
	TrPrintf(-1, "feedy_load_job(%s)", jobName);
	cfg_load(INVALID_SOCKET);
}

//--- feedy_start ------------------------------------------------------
void feedy_start(SOCKET socket)
{
	Error(LOG, 0, "feedy_start jobState=%d", FeedyStatusMsg);
	err_reset();
	{
		SMsgHdr msg;
		msg.msgLen = sizeof(msg);
		msg.msgId  = RUN_JOB;
		gui_send(INVALID_SOCKET, &msg);
	}
}

//--- feedy_stop -------------------------------------------------------
void feedy_stop(SOCKET socket)
{
	Error(LOG, 0, "feedy_stop jobState=%d", FeedyStatusMsg.jobState);
	if (FeedyStatusMsg.jobState==JS_stopping) 
	{
		FeedyStatusMsg.jobState = JS_aborted;
		feedy_abort(socket);
	}
	else
	{
		Error(LOG, 23, "Stopping");
		FeedyStatusMsg.jobState = JS_stopping;
		feedy_abort(socket);
	}
}

//--- feedy_abort -------------------------------------------------------
void feedy_abort(SOCKET socket)
{
	EnJobState jobState=FeedyStatusMsg.jobState;
	if (FeedyStatusMsg.jobState>JS_off)
	{
		FeedyStatusMsg.jobState=JS_off;
		{
			switch(jobState)
			{
			case JS_done:     Error(ERR_CONT, 24, "Job done");		break;
			case JS_stopping: Error(ERR_CONT, 25, "Job Stopped");	break;
			default:		  
							  if (!(_ErrorFlag&1))
							  {
								  Error(ERR_CONT, 26, "Job Aborted");
								  _ErrorFlag|=1;
							  }
							  break;
			}
		}
		SMsgHdr msg;
		msg.msgLen = sizeof(msg);
		msg.msgId  = ABORT_JOB;
		gui_send(INVALID_SOCKET, &msg);
	}
}

//--- _FeedyThread -----------------------------
static void _FeedyThread (void *par)
{
	SetThreadPriority (GetCurrentThread(), THREAD_PRIORITY_HIGHEST);
	while (TRUE)
	{
		Sleep(10);
	}
}