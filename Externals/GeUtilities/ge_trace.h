// *************************************************************************************************
//																				
//	ge_trace: 
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

void TrInit   (char *appName, int level, char *path, char *logDir); // path==NULL: no mfile trace
void TrPrintf (int level, char *format, ...);