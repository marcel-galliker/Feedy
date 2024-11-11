// *************************************************************************************************
//																				
//	ge_threads.c: 
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

//--- includes ------------------------------------------------------
#include "ge_common.h"

//--- ge_mutex_create(WIN32) ---------------------------------------------
HANDLE ge_mutex_create()
{
	return CreateMutex(NULL, FALSE, NULL);
}

//--- ge_mutex_destroy --------------------------------------------------
int    ge_mutex_destroy(HANDLE *mutex)
{
	CloseHandle(*mutex);
	*mutex = NULL;
	return REPLY_OK;
}

//--- ge_mutex_lock(WIN32) ----------------------------------------------
int    ge_mutex_lock(HANDLE mutex)
{
	WaitForSingleObject(mutex, INFINITE);
	return REPLY_OK;
}

//--- ge_mutex_unlock(WIN32) -------------------------------------------
int    ge_mutex_unlock(HANDLE mutex)
{
	ReleaseMutex(mutex);
	return REPLY_OK;
}

//--- ge_sem_create(linux) ---------------------------------------------
HANDLE ge_sem_create()
{
	return CreateSemaphore(NULL, 0, 1, NULL);
}

//--- ge_sem_destroy ---------------------------------------------------
int ge_sem_destroy(HANDLE *sem)
{
	CloseHandle(*sem);
	*sem=NULL;
	return REPLY_OK;
}
//--- ge_sem_wait(linux) ----------------------------------------------
int    ge_sem_wait(HANDLE sem, int time)
{
	int ret;
	if (!time) time=INFINITE;
	ret= WaitForSingleObject(sem, time);
	if (ret==WAIT_OBJECT_0) return REPLY_OK;
	else if (ret==-1) return GetLastError();
	else return ret;
}

//--- ge_sem_post(linux) -------------------------------------------
int    ge_sem_post(HANDLE sem)
{
	long old;
	if (ReleaseSemaphore(sem, 1, &old)) return REPLY_OK;
	else return GetLastError();
}

//--- ge_popen -----------------------------------------
HANDLE ge_popen(const char *cmd)
{
	SECURITY_ATTRIBUTES		sa;
	STARTUPINFOA			startuopInfo;
	PROCESS_INFORMATION		processInfo;
	HANDLE					stream_rd = NULL;
	HANDLE					stream_wr = NULL;
	int		ret;

	//--- create the pipe -------------------
	sa.nLength = sizeof(SECURITY_ATTRIBUTES); 
	sa.bInheritHandle = TRUE; 
	sa.lpSecurityDescriptor = NULL; 

	if ( !CreatePipe(&stream_rd, &stream_wr, &sa, 0) ) 
	{
		printf("StdoutRd CreatePipe"); 
		return NULL;
	}

	if ( !SetHandleInformation(stream_rd, HANDLE_FLAG_INHERIT, 0) )
	{
		printf("Stdout SetHandleInformation"); 
		return NULL;
	}

	//--- create the process ----------------------------------
	memset(&startuopInfo, 0, sizeof(startuopInfo));
	startuopInfo.cb			= sizeof(startuopInfo);
	startuopInfo.dwFlags	= STARTF_USESTDHANDLES;
	startuopInfo.hStdOutput = stream_wr;
	startuopInfo.hStdError  = stream_wr;

	ret = CreateProcessA(NULL, (char*)cmd, NULL, NULL, TRUE, NORMAL_PRIORITY_CLASS | CREATE_NO_WINDOW, NULL, NULL, &startuopInfo, &processInfo);

	CloseHandle(stream_wr);
	CloseHandle(processInfo.hProcess);
	CloseHandle(processInfo.hThread);
	return stream_rd;
}

//--- hgetline ----------------------------------------
char *hgetline (char *buffer, int bufSize, HANDLE handle)
{
	char *pch=buffer;
	DWORD len;
	while(ReadFile(handle, (void*) pch, 1, &len, NULL))
	{
		if (*pch=='\n') 
		{
			pch[1]=0;
			return buffer;
		}
		pch++;
	}
	return NULL;
}

//--- ge_process_running_cnt -------------------------------------------
int ge_process_running_cnt(const char *process, const char *arg)//, HANDLE *phandle, HMODULE *pmodule)
{
	int start, count=0;
	DWORD len;
	HANDLE file;
	char str[100];

	for (start = (int)strlen(process); start > 0; start--)
	{
		if (process[start] == '\\' || process[start] == '/')
		{
			start++;
			break;
		}
	}

	file = ge_popen("tasklist.exe");
	if (file)
	{
		while (ReadFile( file, str, sizeof(str), &len, NULL))
		{
			if (strstr(str, &process[start])) count++;
		}
		CloseHandle(file);
	}
	return count;
}


