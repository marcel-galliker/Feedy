//
// *************************************************************************************************
//
//    Copyright 2022 Galliker-Engineering.ch. All rights reserved.		
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************

#pragma once

#include "ge_common.h"

//--- constants ---------------------------
#pragma pack(1)


//--- SConfig -------------------------------------------
typedef struct SConfig
{
    UINT32 cfg;
} SConfig;

//--- SFeeder ----------------------------------
typedef struct SFeeder
{
    INT32 used;
    INT32 speed;
    INT32 slope;
    INT32 turns;
    INT32 maxTurns;
} SFeeder;

//--- EnCamTrigger -------------------------
typedef enum EnCamTrigger
{
    CAM_early,
    CAM_late
} EnCamTrigger;

//--- SCam ----------------------------------
typedef struct SCam
{
    INT32 intensity;
    INT32 sideUsed;
    EnCamTrigger trigger;
} SCam;
//--- STray -----------------
typedef struct STray
{
    INT32 no;
    INT32 time1;
    INT32 pause;
    INT32 time2;
    INT32 repeats;
    INT32 waitShake;
    INT32 waitFill;
    INT32 tries;
    INT32 startFeed;
    INT32 stopFeed;
} STray;

//--- EnGripperType ------------------
typedef enum EnGripperType
{
    GRP_Vacc,
    GRP_Std,
    GRP_Inverse
} EnGripperType;

//--- SGripper ----------------------------------
typedef struct SGripper
{
    INT32            no;
    EnGripperType    type;
    INT32            time1;
    INT32            time2;
} SGripper;

//--- SJob --------------------------------------------
typedef struct SJob
{
    char        name[STR_64];
    char        description[STR_256];
    INT32       camPrgNo;
    INT32       speed;
    INT32       count;
    SFeeder     feeder;
    SCam        cam;
    STray       tray;
    SGripper    gripper;
} SJob;

#pragma pack()