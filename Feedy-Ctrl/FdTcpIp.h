// *************************************************************************************************
//																				
//	FdTcpIp.h: 
//																				
// *************************************************************************************************
//
//  This is a program that receives commands over tcp/ip and drives a eZ-Inkjet
//
//
//
// *************************************************************************************************
//
//    Copyright 2022 Galliker-Engineering.ch. All rights reserved.		
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************
#pragma once

#include "FeedyDef.h"

#define MAX_MSG_SIZE	4096

//--- defines --------------------------------------------
#define GUI_HOST				"localhost"
#define GUI_PORT				10000

#define GET_STATUS				0x01000001
#define REP_STATUS				0x02000002

#define LIST_JOB_START			0x01001001
#define LIST_JOB_FILE			0x01001002
#define LIST_JOB_END			0x01001003

#define LOAD_CFG				0x01001011
#define SAVE_CFG				0x01001012

#define LOAD_JOB				0x01001021
#define LOAD_JOB_LOADED			0x01001022

#define SAVE_JOB				0x01001023

#define RUN_JOB					0x01001031
#define STOP_JOB				0x01001032
#define ABORT_JOB				0x01001033

#define REP_VERSION				0x03002001

#define ERROR_RESET				0x01003001
#define ERROR_EVENT				0x01003002

#pragma pack(1)

//--- message header --------------------------------------------
typedef struct SMsgHdr
{
	INT32  msgLen;
	UINT32 msgId;
} SMsgHdr;

//--- SGuiMsg ---------------------------
typedef struct SGuiMsg
{
	SMsgHdr hdr;
	char	data[MAX_MSG_SIZE];
} SGuiMsg;

//--- SConfigMsg ---------------------------
typedef struct SConfigMsg
{
	SMsgHdr      hdr;
	SConfig  cfg;
} SConfigMsg;

typedef enum EnJobState
{
	JS_undef,		// 0
	JS_off,			// 1
	JS_starting,	// 2
	JS_printing,	// 3
	JS_stopping,	// 4
	JS_done,		// 5
	JS_aborted		// 6
} EnJobState;

//--- SStatusMsg ---------------------------
typedef struct SStatusMsg
{
	SMsgHdr		hdr;
	INT32		connected;
	EnJobState	jobState;	
} SStatusMsg;

//=== COMMON =================================
//--- SFileMsg -------------------------
typedef struct SFileMsg
{
	SMsgHdr hdr;			// LOAD_LAYOUT_START / SAVE_LAYOUT_START / DEL_LAYOUT_START
	char	filename[STR_64];
} SFileMsg;

//==== JOB ====================================================

//--- SJobMsg -------------------------
typedef struct SJobMsg
{
	SMsgHdr hdr;			// LOAD_LAYOUT_START / SAVE_LAYOUT_START / DEL_LAYOUT_START
	SJob	job;
} SJobMsg;

#pragma pack()
