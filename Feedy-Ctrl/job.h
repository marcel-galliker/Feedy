// *************************************************************************************************
//																				
//	feedy.h: 
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

#include "rt_sok_api.h"
#include "ge_xml_file.h"
#include "FdTcpIp.h"

void job_list		(SOCKET socket);
void job_load		(SOCKET socket, SFileMsg *pmsg);
void job_load_file	(char *filename);
void job_save		(SOCKET socket, SJobMsg *pmsg);
