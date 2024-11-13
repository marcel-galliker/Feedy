// *************************************************************************************************
//																				
//	gui_msg.c: 
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

#include "rt_sok_api.h"
#include "ge_file.h"
#include "ge_time.h"
#include "ge_trace.h"
#include "ge_utilities.h"
#include "gui_server.h"
#include "FdGlobals.h"
#include "job.h"
#include "error.h"
#include "feedy.h"
#include "config.h"
#include "gui_msg.h"

//--- prototypes ---------------------------------------------------
static void _handle_gui_status(SOCKET socket);
static void _test_feeder(SOCKET socket, SFeeder *pfdr);
static void _test_tray  (SOCKET socket, STray *pfdr);

//--- gui_disconnected ----------------------------------
void gui_disconnected(SOCKET socket)
{
}

//--- gui_handle_msg -----------------------------------------------
void gui_handle_msg(SOCKET socket, SGuiMsg *pmsg)
{
	if (FALSE)
	{
		int i;
		UCHAR *ch;
		TrPrintf(-1, "Message: ");
		for ( i=0, ch=(char*)pmsg; i<pmsg->hdr.msgLen; i++, ch++)
		{
		//	if (*ch >= 0x20) TrPrintf( 0, "%c", *ch);
		//	else			 
			TrPrintf( 0, "<%02X>", *ch);
		}
		TrPrintf( 0, "\n");
	}

 	switch(pmsg->hdr.msgId)
	{
	case GET_STATUS:			_handle_gui_status(socket); break;

	case LIST_JOB_START:		job_list			(socket);						 break;
	case LOAD_JOB:				cfg_load			(socket);
								job_load			(socket, (SFileMsg*)pmsg);
								break;

	case SAVE_JOB:				job_save			(socket, (SJobMsg*)pmsg);		 break;

	case LOAD_CFG:				cfg_load	(socket);						 break;
	case SAVE_CFG:				cfg_save	(socket, (SConfigMsg*)pmsg); break;

	case RUN_JOB:				feedy_start		(socket);						 break;
	case STOP_JOB:				feedy_stop		(socket);						 break;
	case ABORT_JOB:				feedy_abort		(socket);						 break;

	case TEST_FEEDER:			_test_feeder(socket, (SFeeder*)pmsg->data); break;
	case TEST_TRAY:				_test_tray(socket, (STray*)pmsg->data); break;

	case STEP_MOTOR:			feedy_step_motor	(socket, (SMotorTest*)pmsg->data); break;
	case START_MOTOR:			feedy_start_motor	(socket, (SMotorTest*)pmsg->data); break;
	case STOP_MOTOR:			feedy_stop_motor	(socket, (SMotorTest*)pmsg->data); break;
	case RUN_MOTOR:				feedy_run_motor		(socket, (SMotorTest*)pmsg->data); break;

	case ERROR_RESET:			feedy_reset_error(); break; // hyper_reset_error(); break;

	default: TrPrintf(-1, "gui_handle_msg: unknown ID=0x%08x\n", pmsg->hdr.msgId);
	}
}

//--- handle_gui_status --------------------------------------------
static void _handle_gui_status(SOCKET socket)
{
	gui_send_status(INVALID_SOCKET);
}

//--- _test_feeder -------------------------------------------
static void _test_feeder(SOCKET socket, SFeeder *pfdr)
{
	TrPrintf(-1, "_test_feeder(used=%d, speed=%d, slope=%d, turns=%d, maxTurns=%d)", pfdr->used, pfdr->speed, pfdr->slope, pfdr->turns, pfdr->maxTurns);
}

//--- _test_tray ----------------------------------------------
static void _test_tray  (SOCKET socket, STray *pfdr)
{
	TrPrintf(-1, "_test_tray");
}
