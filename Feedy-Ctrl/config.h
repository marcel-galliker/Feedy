// *************************************************************************************************
//																				
//	job.h: 
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
#include "FdTcpIp.h"

void cfg_list	(SOCKET socket);
void cfg_load	(SOCKET socket);
void cfg_save	(SOCKET socket, SConfigMsg *pmsg);

