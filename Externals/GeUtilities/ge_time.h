// ****************************************************************************
//
//	ge_time.h
//
// ****************************************************************************
//
//	Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************

#pragma once

#include "ge_common.h"
#include <time.h>

#ifdef __cplusplus
extern "C"{
#endif

int ge_ticks();

int ge_get_date_time_str(time_t time, char *str);
int ge_get_system_date_time_str(char *str);
int ge_get_system_time_str(char *str);
int ge_get_system_date_str(char *str, char separator);
int ge_get_system_numdate_str(char *str, char separator);
// int ge_get_days_between(SYSTEMTIME *from, SYSTEMTIME *to);

#ifdef __cplusplus
}
#endif