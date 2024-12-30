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

//--- includes ------------------------------------------------------
#include "ge_common.h"
#include "ge_file.h"
#include "xml_lib.h"
#include "FdGlobals.h"
#include "gui_server.h"
#include "config.h"

//--- static variables --------------------------------------------

//--- prototypes ----------------------------------------------

//--- cfg_load -----------------------------
void cfg_load	(SOCKET socket)
{
	char path[MAX_PATH];
	EzUserFilePath(PATH_SETTINGS, "Feedy", "xml", path);
	xml_cfg_file(path, &FeedyCfg, READ);

	SConfigMsg reply;
	reply.hdr.msgLen = sizeof(reply);
	reply.hdr.msgId  = LOAD_CFG;
	memcpy(&reply.cfg, &FeedyCfg, sizeof(reply.cfg));
	gui_send(socket, &reply);
}

//--- cfg_save -------------------------------------
void cfg_save	(SOCKET socket, SConfigMsg *pmsg)
{
	char path[MAX_PATH];
	EzUserFilePath(PATH_SETTINGS, "Feedy", "xml", path);
	xml_cfg_file(path, &pmsg->cfg, WRITE);
}