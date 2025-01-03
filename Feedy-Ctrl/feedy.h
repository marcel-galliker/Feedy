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

#include "ge_sockets.h"
#include "FdTcpIp.h"

void feedy_init		(void);
void feedy_load_job	(char *jobName);
void feedy_reset_error(void);

void feedy_start	(SOCKET socket);
void feedy_stop		(SOCKET socket);
void feedy_abort	(SOCKET socket);

void feedy_motor_step	(SOCKET socket, SMotorTest *pmsg);
void feedy_motor_start	(SOCKET socket, SMotorTest *pmsg);
void feedy_motor_stop	(SOCKET socket, SMotorTest *pmsg);
void feedy_motor_run	(SOCKET socket, SMotorTest *pmsg);
void feedy_motor_move2pos(SOCKET socket, SPosition *pmsg);
