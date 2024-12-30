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

#define MotorCnt    4
#define PlacePosCnt 6

//--- SFeeder ----------------------------------
typedef struct SFeeder
{
    INT32 used;
    INT32 speed;
    INT32 slope;
    INT32 turns;
    INT32 maxTurns;
} SFeeder;

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
    INT32            gripperNo;
    char             name[STR_64];
    EnGripperType    type;
} SGripper;

//--- EnCamTrigger -------------------------
typedef enum EnCamTrigger
{
    CAM_early,
    CAM_late
} EnCamTrigger;

//--- SCam ----------------------------------
typedef struct SCam
{
    INT32 camPrgNo;
    INT32 intensity;
    INT32 sideUsed;
    EnCamTrigger trigger;
} SCam;

//--- STray -----------------
typedef struct STray
{
    INT32 plateNo;
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

//--- SPosition ----------------------------------
typedef struct SPosition
{
    INT32 z;
    INT32 x;
    INT32 cy;
    INT32 c;
} SPosition;

//--- SPickSettings ----------------------------------
typedef struct SPickSettings
{
    INT32 approachHeight;
    INT32 approachSpeed;
    INT32 closeTime;
} SPickSettings;

//--- EnTestMode ------------------
typedef enum EnTestMode
{
    TEST_Off,
    TEST_FLY,
    TEST_STOP
} EnTestMode;

//--- STestSettings ----------------------------------
typedef struct STestSettings
{
    EnTestMode testMode;
} STestSettings;

//--- EnPlaceMode ------------------
typedef enum EnPlaceMode
{
    PLACE_Z,
    PLACE_X,
} EnPlaceMode;

//--- SPlaceSettings ----------------------------------
typedef struct SPlaceSettings
{
   EnPlaceMode placeMode;
   INT32    approachHeight;
   INT32    approachSpeed;
   INT32    openTime;
} SPlaceSettings;

//--- SPosition ----------------------------------
typedef struct SPickPlace
{
    SPosition parkPos;
    SPickSettings pickSettings;
    SPosition pickPos;
    STestSettings testSettings;
    SPosition testPos;
    SPlaceSettings placeSettings;
    SPosition placePos[PlacePosCnt];
} SPickPlace;

//--- SJob --------------------------------------------
typedef struct SJob
{
    char        name[STR_64];
    char        image[STR_64];
    char        description[STR_256];
    INT32       speed;
    INT32       count;
    SFeeder     feeder;
    SCam        cam;
    STray       tray;
    SGripper    gripper;
    SPickPlace  pickPlace;
} SJob;

//--- SMotorSettings ----------------------------------
typedef struct SMotorSettings
{
    INT32 powerHold;
    INT32 powerMove;
    INT32 acc;
    INT32 speed;
    INT32 testPosStart;
    INT32 testPosEnd;
    INT32 testSpeed;
} SMotorSettings;

//--- SMotorTest --------------------------
typedef struct SMotorTest
{
    INT32 motorNo;
    INT32 direction;
    INT32 testSpeed;
    SMotorSettings settings;
} SMotorTest;

//--- SConfig ----------------------------------
typedef struct SConfig
{
    SMotorSettings motor[MotorCnt];
} SConfig;

//--- EnJobState ------------------
typedef enum EnJobState
{
    JS_undef,
    JS_off,
    JS_starting,
    JS_running,
    JS_stopping,
    JS_done,
    JS_aborted
} EnJobState;

//--- SFeedyStatus ---------------------------
typedef struct SFeedyStatus
{
    INT32	    connected;
    EnJobState	jobState;
    INT32       motorPos[4];
    INT32       testSpeed;
} SFeedyStatus;


#pragma pack()