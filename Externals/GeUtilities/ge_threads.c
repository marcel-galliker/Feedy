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
#include "ge_threads.h"

#ifdef linux
	#include <pthread.h>
	#include <semaphore.h>
	#include <dirent.h>
	#include <unistd.h>
#endif

//--- ge_mutex_create ---------------------------------------------

HANDLE rx_mutex_create()
{
#ifdef linux
	pthread_mutex_t 	*mutex = (pthread_mutex_t*) malloc(sizeof(pthread_mutex_t));
	pthread_mutex_init(mutex, NULL);
	return (HANDLE)mutex;
#else
	return CreateMutex(NULL, FALSE, NULL);
#endif
}

//--- ge_mutex_destroy --------------------------------------------------
int    ge_mutex_destroy(HANDLE *mutex)
{
#ifdef linux
	pthread_mutex_destroy((pthread_mutex_t*)*mutex);
#else
	CloseHandle(*mutex);
#endif
	*mutex = NULL;
	return REPLY_OK;
}

//--- ge_mutex_lock(WIN32) ----------------------------------------------
int    ge_mutex_lock(HANDLE mutex)
{
#ifdef linux
	pthread_mutex_lock((pthread_mutex_t*)mutex);
#else
	WaitForSingleObject(mutex, INFINITE);
#endif
	return REPLY_OK;
}

//--- ge_mutex_unlock(WIN32) -------------------------------------------
int    ge_mutex_unlock(HANDLE mutex)
{
#ifdef linux
	pthread_mutex_unlock((pthread_mutex_t*)mutex);
#else
	ReleaseMutex(mutex);
#endif
	return REPLY_OK;
}

//--- ge_sem_create(linux) ---------------------------------------------
HANDLE ge_sem_create()
{
#ifdef linux
	sem_t 	*sem = (sem_t*) malloc(sizeof(sem_t));
	sem_init(sem, 0, 0); // used between threads of one process
	return (HANDLE)sem;
#else
	return CreateSemaphore(NULL, 0, 1, NULL);
#endif
}

//--- ge_sem_destroy ---------------------------------------------------
int ge_sem_destroy(HANDLE *sem)
{
#ifdef linux
	sem_destroy((sem_t*)*sem);
#else
	CloseHandle(*sem);
#endif
	*sem=NULL;
	return REPLY_OK;
}
//--- ge_sem_wait(linux) ----------------------------------------------
int    ge_sem_wait(HANDLE sem, int time)
{
#ifdef linux
	if (time)
	{ 
		struct timespec t;
		UINT64 ms;
		int ret;
		clock_gettime(CLOCK_REALTIME, &t);
		t.tv_sec  += time / 1000;
		t.tv_nsec += (time % 1000) * 1000000;
		if (t.tv_nsec > 1000000000)
		{
			t.tv_sec++;
			t.tv_nsec -= 1000000000;
		}
		
		ret = sem_timedwait((sem_t*)sem, &t);
		if (ret)
			ret = ret;
		return ret;
	}
	else if (sem == NULL) return REPLY_ERROR;
	else sem_wait((sem_t*)sem);
	return REPLY_OK;
#else

	int ret;
	if (!time) time=INFINITE;
	ret= WaitForSingleObject(sem, time);
	if (ret==WAIT_OBJECT_0) return REPLY_OK;
	else if (ret==-1) return GetLastError();
	else return ret;
#endif
}

//--- ge_sem_post(linux) -------------------------------------------
int    ge_sem_post(HANDLE sem)
{
	if (sem == NULL) return REPLY_ERROR;
#ifdef linux
	sem_post((sem_t*)sem);
	return REPLY_OK;
#else
	long old;
	if (ReleaseSemaphore(sem, 1, &old)) return REPLY_OK;
	else return GetLastError();
#endif
}

//--- ge_thread_start ---------------------------------------------
HANDLE ge_thread_start(thread_main start, void *arg)
{
#ifdef linux
	pthread_t handle; 
	int ret = pthread_create(&handle, NULL, start, arg);
	return (HANDLE)handle;
#else
	return CreateThread( 
		NULL,									/* no security attributes */
		0,										/* default stack size */
		start,							/* function to call */
		NULL,									/* parameter for function */
		0,										/* 0=thread runs immediately after being called */
		NULL									/* returns thread identifier */
		);
#endif
}

//--- ge_thread_set_priority -------------------------------
int ge_thread_set_priority(int prio)
{
#ifdef linux
	pthread_t this_thread = pthread_self();
	struct sched_param params;
	int min = sched_get_priority_min(SCHED_FIFO);
	int max = sched_get_priority_max(SCHED_FIFO);
	if (prio < max) params.sched_priority = prio;
	else            params.sched_priority = max;
	return pthread_setschedparam(this_thread, SCHED_FIFO, &params);
#else
	SetThreadPriority(GetCurrentThread(), prio);
	return prio;
#endif	
}

//--- ge_thread_sleep ----------------
void ge_thread_sleep(int ms)
{
	#ifdef linux
		usleep(ms*1000);
	#else
		Sleep(ms);
	#endif
}
//--- ge_popen -----------------------------------------
#ifdef linux
HANDLE ge_popen(const char *cmd)
{
	return popen(cmd, "r");
}
#else
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
#endif

//--- hgetline ----------------------------------------
#ifdef linux
	char *hgetline(char *buffer, int bufSize, HANDLE handle)
	{
		return fgets(buffer, bufSize, handle);
	}
#else
	char *hgetline (char *buffer, int bufSize, HANDLE handle)
	{
		char *pch=buffer;
		INT32 len;
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
#endif

//--- ge_process_running_cnt -------------------------------------------
#ifdef linux

	//--- _process_match --------------------------------
	static int _process_match(INT32 processId, const char *pname, const char *arg);
	static int _process_match(INT32 processId, const char *pname, const char *arg)
	{
		FILE	*file;
		char	fname[MAX_PATH];
		char	cmdline[1024];
		char	*a;
		size_t	size;

		// readllink("/proc/%d/cmdline", processId);
		sprintf(fname, "/proc/%d/cmdline", processId);
		if (file = fopen(fname, "r"))
		{
			memset(cmdline, 0, sizeof(cmdline));
			size = fread(cmdline, 1, sizeof(cmdline) - 2, file);
			fclose(file);
			// printf("Process %d pname >>%s<<\n", processId, pname);
			// printf("cmdline >>%s<<\n", cmdline);

			if (!strstr(cmdline, pname)) return FALSE;
			if (arg == NULL) return TRUE;
			a = cmdline;
			while (1) 
			{
				a += strlen(a) + 1;
				if (!*a) return FALSE;
				// printf("arg >>%s<<\n", a);
				if (strstr(a, arg)) return TRUE;
			}
		}
		return FALSE;
	}

//--- rx_process_running_cnt ------------------------------
	int rx_process_running_cnt(const char *process, const char *arg)
	{
		int cnt = 0;
		DIR *pDir;
		struct dirent *pEntry;
		INT32 process_id;
		const char *process_name;

		const char *ch;
		process_name = process;
		for (ch = process; *ch; ch++)
		{
			if (*ch == '/') process_name = ch + 1;
		}

		pDir = opendir("/proc/");
		while (pEntry = readdir(pDir))
		{
			if ((pEntry->d_type & DT_DIR)					// directory
			&& (process_id = atoi(pEntry->d_name))			// && numbered
			&& (_process_match(process_id, process_name, arg)	// && match)
			))
			{
				cnt++;
			}
		}
		closedir(pDir);
		//	TrPrintfL(1, "rx_process_running_cnt(%s)=%d", process_name, cnt);
		return cnt;
	}
#else
	int ge_process_running_cnt(const char *process, const char *arg)//, HANDLE *phandle, HMODULE *pmodule)
	{
		int start, count=0;
		INT32 len;
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
#endif

