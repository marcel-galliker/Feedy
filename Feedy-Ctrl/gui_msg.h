// *************************************************************************************************
//																				
//	gui_msg.h: 
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

#include "ge_sockets.h"
#include "FdTcpIp.h"

void gui_disconnected(SOCKET socket);
void gui_handle_msg(SOCKET socket, SGuiMsg *pmsg);
