// *************************************************************************************************
//																				
//	args.c: 
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

#include "ge_common.h"
#include "ge_utilities.h"
#include "ge_file.h"
#include "args.h"

char arg_exeDir[200];
int arg_trace		= 0;	// trace is off!

//--- args_init -----------------------------------
void args_init(int argc, char** argv)
{
	int help=FALSE;
	
	char path[MAX_PATH];
#ifdef linux
	printf("GetExe-Filename!! >>%s<<\n", argv[0]);
	ge_split_path(argv[0], arg_exeDir, NULL, NULL);
#else
	GetModuleFileNameA(NULL, path, sizeof(path));
	ge_split_path(path, arg_exeDir, NULL, NULL);
#endif

	/*
	if (strstart(arg_exeDir, "\\\\"))
	{
		strncpy(arg_exeDir, "D:", 2);
		for (char *pch=&arg_exeDir[2]; *pch; pch++)
		{
			if (*pch=='\\') 
			{
				strcpy(&arg_exeDir[2], pch);
				break;
			}
		}
		SetCurrentDirectoryA(arg_exeDir);
	}
	*/
	int i;
	for (i=1; i<argc; i++)
	{
		if      (!stricmp(argv[i], "-trace"))		arg_trace		= atoi(argv[++i]);
		else help=TRUE;
	}

	//--- help ----------------------------
	if (help) 
	{
		printf("Parameters:\n");
		printf(" -trace <level>: Set the trace level\n");
	}
}