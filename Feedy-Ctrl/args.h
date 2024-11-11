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
extern int arg_simuMeteor;
extern int arg_simuHyper;
extern int arg_simuPrinter;
extern int arg_simuSpeed;

void args_init(int argc, char** argv);