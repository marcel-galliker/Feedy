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

static SMotorTest	_MotorTest;

static int			_ErrorFlag;

//--- prototypes ---------------------------------
static void _FeedyThread (void *par);

//--- feedy_init --------------------------------
void feedy_init(void)
{
	_ErrorFlag = 0;
	/*--- create a receiver thread for that client ---*/
	_FeedyThreadHandle=CreateThread ( 
		NULL,									/* no security attributes */
		0,										/* default stack size */
		(LPTHREAD_START_ROUTINE) &_FeedyThread,	/* function to call */
		NULL,									/* parameter for function */
		0,										/* 0=thread runs immediately after being called */
		NULL									/* returns thread identifier */
	);

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
	Error(LOG, 0, "feedy_start jobState=%d", FeedyStatus);
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
	Error(LOG, 0, "feedy_stop jobState=%d", FeedyStatus.jobState);
	if (FeedyStatus.jobState==JS_stopping) 
	{
		FeedyStatus.jobState = JS_aborted;
		feedy_abort(socket);
	}
	else
	{
		Error(LOG, 23, "Stopping");
		FeedyStatus.jobState = JS_stopping;
		feedy_abort(socket);
	}
}

//--- feedy_abort -------------------------------------------------------
void feedy_abort(SOCKET socket)
{
	EnJobState jobState=FeedyStatus.jobState;
	if (FeedyStatus.jobState>JS_off)
	{
		FeedyStatus.jobState=JS_off;
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

//--- feedy_motor_step -----------------------------------
void feedy_motor_step	(SOCKET socket, SMotorTest *pmsg)
{
	memset(&_MotorTest, 0, sizeof(_MotorTest));
	FeedyStatus.motorPos[pmsg->motorNo] += pmsg->direction;
	gui_send_status(INVALID_SOCKET);
}

//--- feedy_motor_start -----------------------------------------
void feedy_motor_start(SOCKET socket, SMotorTest *pmsg)
{
	memcpy(&_MotorTest, pmsg, sizeof(_MotorTest));
}

//--- feedy_motor_stop -------------------------------------------
void feedy_motor_stop	(SOCKET socket, SMotorTest *pmsg)
{
	memset(&_MotorTest, 0, sizeof(_MotorTest));
}

//--- feedy_motor_run --------------------------------------------
void feedy_motor_run	(SOCKET socket, SMotorTest *pmsg)
{
	memcpy(&_MotorTest, pmsg, sizeof(_MotorTest));
}

//--- feedy_motor_move2pos -----------------------------------
void feedy_motor_move2pos(SOCKET socket, SPosition *pmsg)
{
	FeedyStatus.motorPos[0] = pmsg->z;
	FeedyStatus.motorPos[1] = pmsg->x;
	FeedyStatus.motorPos[2] = pmsg->cy;
	FeedyStatus.motorPos[3] = pmsg->c;
	FeedyStatus.testSpeed   = FeedyJob.speed;
	gui_send_status(INVALID_SOCKET);
}

//--- _FeedyThread -----------------------------
static void _FeedyThread (void *par)
{
	SetThreadPriority (GetCurrentThread(), THREAD_PRIORITY_HIGHEST);
	while (TRUE)
	{
		Sleep(10);

		if (_MotorTest.direction)
		{			
			FeedyStatus.motorPos[_MotorTest.motorNo] += (100*_MotorTest.direction * _MotorTest.testSpeed) / 100;
			FeedyStatus.testSpeed = _MotorTest.testSpeed;
			if (FeedyStatus.motorPos[_MotorTest.motorNo]<_MotorTest.settings.testPosStart)
			{
				FeedyStatus.motorPos[_MotorTest.motorNo]=_MotorTest.settings.testPosStart;
				memset(&_MotorTest, 0, sizeof(_MotorTest));
			}

			if (FeedyStatus.motorPos[_MotorTest.motorNo] > _MotorTest.settings.testPosEnd)
			{
				FeedyStatus.motorPos[_MotorTest.motorNo] = _MotorTest.settings.testPosEnd;
				memset(&_MotorTest, 0, sizeof(_MotorTest));
			}

			gui_send_status(INVALID_SOCKET);
			Sleep(100);
		}
	}
}