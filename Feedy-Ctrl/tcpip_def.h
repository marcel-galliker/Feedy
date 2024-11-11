// ************************************************************************************************
//																				
//	tcpip_def.h: 
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

//--- TCP_IP_PORT on EZ_PrintEngine --------------------------------------------------
#define TCP_PORT		10001

//--- messages to Ez-PrintEngine ------ format -----------				--- description -------------------
#define CMD_START_JOB	 "#startjob:"	// <jobname> '\n'				//  starting a new job
#define CMD_FIELDNAMES	 "#fieldnames:"	// <name1>';'<name2>... '\n'	//  fieldnames: ';'-Separated CSV
#define CMD_DATA		 "#data:"		// <data>';'<data>..... '\n'	//  data: ';'-Separated CSV
#define CMD_QUALITY_COPY "#QualityCopy:"// <text>';'<text>..... '\n'	//  text: ';'-Separated CSV
#define CMD_END_JOB		 "#endjob"		// '\n'							//	stopping a job	

//--- messages from Ez-PrintEngine ---- format -----------  --- description -------------------
#define REP_INITIALZIED		"#Initialized"
#define REP_READY_TO_PRINT	"#ReadyToPrint"
#define REP_PRINT_DONE		"#PrintDone"
#define REP_ERROR			"#error:"	// <error string> '\n'	//  Error message