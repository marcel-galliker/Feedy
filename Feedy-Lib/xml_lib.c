// *************************************************************************************************
//																				
//	xml_lib.c: 
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

#include "ge_file.h"
#include "ge_xml_file.h"
#include "xml_lib.h"


//--- prototypes ----------------------
static int _xml_job(HANDLE file, SJob *pjob, EN_Xml_Action action);
static int _xml_feeder(HANDLE file, SFeeder *pbin, EN_Xml_Action action);
static int _xml_cam(HANDLE file, SCam *pcam, EN_Xml_Action action);
static int _xml_tray(HANDLE file, STray *ptray, EN_Xml_Action action);
static int _xml_gripper(HANDLE file, SGripper *pgrp, EN_Xml_Action action);
static int _xml_position(HANDLE file, char *name, int no, SPosition *ppo, EN_Xml_Action action);
static int _xml_pickplace(HANDLE file, SPickPlace *ppp, EN_Xml_Action action);
static int _xml_pick_settings(HANDLE file, SPickSettings *pset, EN_Xml_Action action);
static int _xml_test_settings(HANDLE file, STestSettings *pset, EN_Xml_Action action);
static int _xml_place_settings(HANDLE file, SPlaceSettings *pset, EN_Xml_Action action);

static int _xml_motor(HANDLE file, int no, SMotorSettings *pmtr, EN_Xml_Action action);


//--- xml_job_file -----------------------------------
int xml_job_file (char *filepath, SJob *pJob, EN_Xml_Action action)
{
	HANDLE file = xml_create();
	char path[MAX_PATH];
	char name[MAX_PATH];
	char ext[10];

	ge_split_path(filepath, path, name, ext);
	if (!*name) return REPLY_ERROR;
	sprintf(&path[strlen(path)], "%s.xml", name);

	if (action==READ)
	{
		memset(pJob, 0, sizeof(SJob));
	}

	xml_load(file, path);
	_xml_job(file, pJob, action);
	strcpy(pJob->name, name);
	if (action==WRITE) 
	{
		ge_mkdir_path(path);
		xml_save(file, path);
	}
	xml_destroy(file);
	
	return REPLY_OK;
}

//--- _xml_job ----------------------------------
static int _xml_job(HANDLE file, SJob *pjob, EN_Xml_Action action)
{
	if (xml_chapter(file, "Job", -1, action)==REPLY_OK)
	{
		xml_str		(file, "description",	 action, pjob->description,		sizeof(pjob->description),      "");
		xml_str	    (file, "image",		action,	pjob->image, sizeof(pjob->image),  "");
		xml_int32	(file, "speed", action, &pjob->speed, 100);
		xml_int32	(file, "count", action, &pjob->count, 0);

		_xml_feeder (file, &pjob->feeder, action);
		_xml_cam	(file, &pjob->cam, action);
		_xml_tray	(file, &pjob->tray, action);
		_xml_gripper(file, &pjob->gripper, action);
		_xml_pickplace(file, &pjob->pickPlace, action);

		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_bin ---------------------------------------------------------------
static int _xml_feeder(HANDLE file, SFeeder *pfdr, EN_Xml_Action action)
{
	if (xml_chapter(file, "Feeder", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "used", action,	&pfdr->used,  0);
		xml_int32	(file, "speed", action, &pfdr->speed, 100);
		xml_int32	(file, "slope", action, &pfdr->slope, 1);
		xml_int32	(file, "turns", action, &pfdr->turns, 1);
		xml_int32	(file, "maxTurns", action, &pfdr->maxTurns, 10);

		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_cam -------------------------------------------------
static int _xml_cam(HANDLE file, SCam *pcam, EN_Xml_Action action)
{
	if (xml_chapter(file, "Cam", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "camPrgNo", action,	&pcam->camPrgNo, 0);
		xml_int32	(file, "intensity", action,	&pcam->intensity,  0);
		xml_int32	(file, "sideUsed",	action, &pcam->sideUsed, 100);
		xml_int32	(file, "trigger",	action, (INT32*)&pcam->trigger, 1);

		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_tray ------------------------------------
static int _xml_tray(HANDLE file, STray *ptray, EN_Xml_Action action)
{
	if (xml_chapter(file, "Tray", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "plateNo",		action,	&ptray->plateNo,  0);
		xml_int32	(file, "time1",		action,	&ptray->time1,  0);
		xml_int32	(file, "pause",		action,	&ptray->pause,  0);
		xml_int32	(file, "time2",		action,	&ptray->time2,  0);
		xml_int32	(file, "repeats",	action,	&ptray->repeats,  0);
		xml_int32	(file, "waitShake", action,	&ptray->waitShake,  0);
		xml_int32	(file, "waitFill",	action,	&ptray->waitFill,  0);
		xml_int32	(file, "tries",		action,	&ptray->tries,  0);
		xml_int32	(file, "startFeed",	action,	&ptray->startFeed,  0);
		xml_int32	(file, "stopFeed",	action,	&ptray->stopFeed,  0);

		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_gripper --------------------------------------
static int _xml_gripper(HANDLE file, SGripper *pgrp, EN_Xml_Action action)
{
	if (xml_chapter(file, "Gripper", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "gripperNo",		action,	&pgrp->gripperNo,  0);
		xml_int32	(file, "type",		action,	(INT32*)&pgrp->type,  0);
		xml_str	    (file, "name",		action,	pgrp->name, sizeof(pgrp->name),  "");
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_position ------------------
static int _xml_position(HANDLE file, char *name, int no, SPosition *ppos, EN_Xml_Action action)
{
	if (xml_chapter(file, name, no, action)==REPLY_OK)
	{
		xml_int32	(file, "y",		action,	&ppos->z,  0);
		xml_int32	(file, "x",		action,	&ppos->x,  0);
		xml_int32	(file, "cy",	action,	&ppos->cy,  0);
		xml_int32	(file, "c",		action,	&ppos->c,  0);
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_pick_settings ------------------------------------------
static int _xml_pick_settings(HANDLE file, SPickSettings *pset, EN_Xml_Action action)
{
	if (xml_chapter(file, "Pick", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "approachHeight",action,	&pset->approachHeight,  0);
		xml_int32	(file, "approachSpeed",	action,	&pset->approachSpeed,	0);
		xml_int32	(file, "closeTime",		action,	&pset->closeTime,		0);
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_test_settings ---------------------------------------
static int _xml_test_settings(HANDLE file, STestSettings *pset, EN_Xml_Action action)
{
	if (xml_chapter(file, "Test", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "testMode",		action,	(INT32*)&pset->testMode,		0);
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_place_settings -----------------------------------------
static int _xml_place_settings(HANDLE file, SPlaceSettings *pset, EN_Xml_Action action)
{
	if (xml_chapter(file, "Place", -1, action)==REPLY_OK)
	{
		xml_int32	(file, "placeMode",		action,	(INT32*)&pset->placeMode,		0);
		xml_int32	(file, "approachHeight",action,	&pset->approachHeight,  0);
		xml_int32	(file, "approachSpeed",	action,	&pset->approachSpeed,	0);
		xml_int32	(file, "openTime",		action,	&pset->openTime,		0);
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- _xml_pickplace --------------------------------
static int _xml_pickplace(HANDLE file, SPickPlace *ppp, EN_Xml_Action action)
{
	if (xml_chapter(file, "PickPlace", -1, action)==REPLY_OK)
	{
		_xml_position(file, "ParkPos", -1, &ppp->parkPos, action);
		_xml_pick_settings(file, &ppp->pickSettings, action);
		_xml_position(file, "PickPos", -1, &ppp->pickPos, action);
		_xml_test_settings(file, &ppp->testSettings, action);
		_xml_position(file, "TestPos", -1, &ppp->testPos, action);
		_xml_place_settings(file, &ppp->placeSettings, action);
		for (int i=0; i<PlacePosCnt; i++)
			_xml_position(file, "PlacePos", i, &ppp->placePos[i], action);

		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}

//--- xml_last_job ------------------
int xml_last_job(char *filepath, char *jobName, int strlen, EN_Xml_Action action)
{
	HANDLE file = xml_create();

	xml_load(file, filepath);

	if (xml_chapter(file, "AppData", -1, action)==REPLY_OK)
	{
		xml_str		(file,	"jobName", action, jobName, strlen, "");
		xml_chapter	(file, "..", -1, action);
	}
	xml_chapter(file, "..", -1, action);
	if (action==WRITE)
	{
		ge_mkdir_path(filepath);
		xml_save(file, filepath);
	}
	xml_destroy(file);

	return REPLY_OK;
}

//--- xml_cfg_file ----------------------
int xml_cfg_file   (char *filepath, SConfig *pcfg, EN_Xml_Action action)
{
	HANDLE file = xml_create();

	xml_load(file, filepath);
	if (action==READ)
	{
		memset(pcfg, 0, sizeof(SConfig));
	}
	if (xml_chapter(file, "Config", -1, action)==REPLY_OK)
	{	
		for (int i=0; i<SIZEOF(pcfg->motor); i++)
			_xml_motor(file, i, &pcfg->motor[i], action);
		xml_chapter(file, "..", -1, action);
	}

	if (action==WRITE)
		xml_save(file, filepath);
	xml_destroy(file);

	return REPLY_OK;
}

//--- _xml_motor ---------------------------------------------------------
static int _xml_motor(HANDLE file, int no, SMotorSettings *pmtr, EN_Xml_Action action)
{
	if (xml_chapter(file, "Motor", no, action)==REPLY_OK)
	{
		xml_int32	(file, "powerHold",		action,	&pmtr->powerHold,  0);
		xml_int32	(file, "powerMove",		action,	&pmtr->powerMove,  0);
		xml_int32	(file, "acc",			action,	&pmtr->acc,  0);
		xml_int32	(file, "speed",			action,	&pmtr->speed,  0);
		xml_int32	(file, "testPosStart",	action,	&pmtr->testPosStart,  0);
		xml_int32	(file, "testPosEnd",	action,	&pmtr->testPosEnd,  0);
		xml_int32	(file, "testSpeed",		action,	&pmtr->testSpeed,  0);
		xml_chapter	(file, "..", -1, action);
		return TRUE;
	}
	else return FALSE;
}