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
int arg_file_trace  = FALSE;
int arg_simuPrinter = FALSE;
int arg_simuHyper	= FALSE;
int arg_simuMeteor	= FALSE;
int arg_simuSpeed	= 0;

//--- args_init -----------------------------------
void args_init(int argc, char** argv)
{
	int help=FALSE;
	
	char path[MAX_PATH];
	GetModuleFileNameA(NULL, path, sizeof(path));
	ge_split_path(path, arg_exeDir, NULL, NULL);

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
	
	for (int i=1; i<argc; i++)
	{
		if      (!stricmp(argv[i], "-simuprinter"))	arg_simuPrinter	= TRUE;
		else if (!stricmp(argv[i], "-simumeteor"))	arg_simuMeteor	= TRUE;
		else if (!stricmp(argv[i], "-simuhyper"))	arg_simuHyper	= TRUE;
		else if (!stricmp(argv[i], "-trace"))		arg_trace		= atoi(argv[++i]);
		else if (!stricmp(argv[i], "-simuspeed"))	
			arg_simuSpeed	= atoi(argv[++i]);
		else help=TRUE;
	}

	//--- help ----------------------------
	if (help) 
	{
		printf("Parameters:\n");
		printf(" -simuprinter:   Simulate the printer\n");
		printf(" -simumeteor:   Simulate the meteor interface printer\n");
		printf(" -trace <level>: Set the trace level\n");
	}
}