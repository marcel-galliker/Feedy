// *************************************************************************************************
//																				
//	gui_server.h: 
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

#include "FdTcpIp.h"

void gui_server(void);

void gui_send(SOCKET socket, void *pmsg);

