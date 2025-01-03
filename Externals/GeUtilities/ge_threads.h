// *************************************************************************************************
//																				
//	ge_threads.h: 
//																				
// *************************************************************************************************
//
//  This is a program that receives commands over tcp/ip and drives a eZ-Inkjet
//
//
//
// *************************************************************************************************
//
//    Copyright 2022 Galliker-Engineering GmbH. All rights reserved.		
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************

#pragma once

#ifdef linux
	#include <pthread.h>
#endif

//--- includes ------------------------------------------------------
#include "ge_common.h"

HANDLE ge_mutex_create	();
int    ge_mutex_destroy	(HANDLE *mutex);
int    ge_mutex_lock	(HANDLE mutex);
int    ge_mutex_unlock	(HANDLE mutex);


HANDLE ge_sem_create	();
int	   ge_sem_destroy	(HANDLE *sem);
int    ge_sem_wait		(HANDLE sem, int time);
int    ge_sem_post		(HANDLE sem);


typedef void *(*thread_main)(void *args);

HANDLE ge_thread_start(thread_main start, void *arg);
int ge_thread_set_priority(int prio);
void ge_thread_sleep(int ms);

#ifdef linux
	#define THREAD_PRIORITY_HIGHEST 100
#endif


int    ge_process_running_cnt	(const char *process, const char *arg);
HANDLE ge_popen(const char *cmd);
char *hgetline (char *buffer, int bufSize, HANDLE handle);