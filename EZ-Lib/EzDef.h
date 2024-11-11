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

//--- SJob --------------------------------------------
typedef struct SJob
{
    char        name[STR_64];
    char        description[STR_256];
} SJob;

#pragma pack()