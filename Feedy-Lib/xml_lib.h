// *************************************************************************************************
//																				
//	xml_lib.h: 
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
// *************************************************************************************************#pragma once

#pragma once

#define EXPORT EXTERN_C _declspec(dllexport) 

#ifdef __cplusplus
extern "C"{
#endif

#include "ge_common.h"
#include "ge_xml_file.h"
#include "FeedyDef.h"

EXPORT int xml_last_job   (char *filepath, char *jobname, int strlen, EN_Xml_Action action);
EXPORT int xml_job_file   (char *filepath, SJob *pJob, EN_Xml_Action action);
EXPORT int xml_cfg_file	  (char *filepath, SConfig *pcfg, EN_Xml_Action action);

#ifdef __cplusplus
}
#endif