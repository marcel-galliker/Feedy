// *************************************************************************************************
//																				
//	version.h: 
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

//	format of version string:
//	-> git tag="v1.0"
//	Version-String = "<git-tag.mayor>.<git-tag.minor>.<git revision>.<compiler.incremental>"
//  the file "version.c" is updated by EZ-Version.exe with each compilation of the release configuration
//  (exclude version.c from the git repositroy) 

extern const char EzVersion[32];
extern const char EzBranch[64];
extern const char EzCommit[16];
