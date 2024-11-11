// *************************************************************************************************
//																				
//	key.h: 
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

void key_clear(void);
void key_trace(void);
int  key_add  (int recNo, int filePos, char *key);
int  key_find (char *key, int *recNo, int *filepos);

