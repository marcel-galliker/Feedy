// ****************************************************************************
//
//	ge_file.h
//
// ****************************************************************************
//
//	Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************

#pragma once

#ifdef __cplusplus
extern "C"{
#endif

#include "ge_common.h"

#ifdef linux
	#define _SH_DENYRW      0x10    /* deny read/write mode */
	#define _SH_DENYWR      0x20    /* deny write mode */
	#define _SH_DENYRD      0x30    /* deny read mode */
	#define _SH_DENYNO      0x40    /* deny none mode */
	#define _SH_SECURE      0x80    /* secure mode */
#endif

typedef HANDLE SEARCH_Handle;

int		ge_file_exists	  (const char *path);
int		ge_dir_exists	  (const char *path); 
time_t	ge_file_get_mtime (const char *path);
void    ge_file_set_mtime (const char *path, time_t time);
INT64	ge_file_get_size  (const char *path);
void	ge_file_set_readonly(const char *path, int readonly);
INT64	ge_file_seek	  (FILE *file, INT64 offset, int origin);		
int		ge_file_del_if_older(const char *path, const char *compPath);	// return 0=not deleted
int 	ge_file_mount(const char *remotePath, const char *mntPath, const char *user, const char *pwd);

SEARCH_Handle	ge_search_open (const char *path, const char *match);
int				ge_search_next (SEARCH_Handle hsearch, char *filename, int size, UINT64 *writeTime, UINT32 *filesize, UINT32 *isdir);
int				ge_search_close(SEARCH_Handle hsearch);

int				ge_fnmatch(const char * match, const char *str);

int		ge_mkdir(const char *dirname);
void	ge_mkdir_path(const char *dirname);
int		ge_rmdir(const char *dirname);
FILE *  ge_fopen(const char * path, const char * mode, int sharemode);
void	ge_split_path(const char *path, char *dir, char *name, char *ext);

int		ge_recycle_file(char *path);
int		ge_remove_old_files(const char *searchStr, int days);
int		ge_recycle_old_files(const char *searchStr, int days);

char*	ge_user_path(char *path);
char*	ge_appdata_path(char *path, char *appName, char *filename);

#ifdef __cplusplus
}
#endif