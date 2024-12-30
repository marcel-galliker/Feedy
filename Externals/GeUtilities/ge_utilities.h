// *************************************************************************************************
//																				
//	ge_utilities: 
//																				
// *************************************************************************************************
//
//  This is a program that receives commands over tcp/ip and drives a eZ-Inkjet
//
//
//
// *************************************************************************************************
//
//    Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.	
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************

#pragma once

#include "ge_common.h"

void char_to_wchar(wchar_t *puni, const char *pansi, int len);
int stricmp(const char *str1, char *str2);


#ifdef WIN32
	void	TimetToFileTime( time_t t, LPFILETIME pft );
	time_t	FiletimeToTimet(const LPFILETIME pft);
#else
	time_t	FiletimeToTimet(const UINT64 pft);
#endif

char* char_to_lower(const char *str, char *out);
int  strstart(char *str, char *start);
int  strend(char *str, char *end);
char *ge_strncpy(char *dst, char* src, int maxDstLen);
char *ge_getline(char *str);
char * win_error(int err, char *str, int size);
int  wtoi(UINT16 *wstr);

char *bin2hex(char *str, void *data, int len);	// converts to hex-string
void *hex2bin(char *str, void *data, int len);	// converts from hex-string

UINT32 swap(UINT32 *value);