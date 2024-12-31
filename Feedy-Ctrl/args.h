// *************************************************************************************************
//																				
//	args.h: 
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

extern char arg_exeDir[200];
extern int arg_trace;

void args_init(int argc, char** argv);