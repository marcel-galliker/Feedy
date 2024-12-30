// ****************************************************************************
//
//	ge_xml_file.h
//
// ****************************************************************************
//
//	Copyright 2013 by Radex AG, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************

#pragma once

#include "ge_common.h"

#ifdef linux
#else
	#include <windows.h>
#endif

#ifdef __cplusplus
extern "C"{
#endif

HANDLE xml_create();
void   xml_destroy(HANDLE setup);

int xml_load(HANDLE hsetup, const char *filename);
int xml_save(HANDLE hsetup, const char *filename);
char *xml_to_str(HANDLE hsetup);

char	*xml_get_error_str(HANDLE hsetup);

typedef enum 
{
	READ,
	WRITE
} EN_Xml_Action;

int  xml_chapter		 (HANDLE hsetup, const char *name, int no, EN_Xml_Action action);
int  xml_chapter_next  (HANDLE hsetup, EN_Xml_Action action, char *name, int size);
int  xml_chapter_add	 (HANDLE hsetup, char *name);
int  xml_chapter_delete(HANDLE hsetup);
void xml_text		(HANDLE hsetup, EN_Xml_Action action,  char  *val, int size, const char* def);
void xml_str		(HANDLE hsetup, const char *name, EN_Xml_Action action,  char  *val, int size, const char* def);
void xml_uchar	(HANDLE hsetup, const char *name, EN_Xml_Action action, UCHAR  *val, UCHAR  def);
void xml_uchar_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, UCHAR  *val, int cnt, INT32 def);
// void xml_uint8	(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT8  *val, UINT8  def);
void xml_int16	(HANDLE hsetup, const char *name, EN_Xml_Action action, INT16  *val, INT16  def);
void xml_uint16	(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT16 *val, UINT16 def);
void xml_int16_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, INT16  *val, int cnt, INT16 def);
void xml_int32	(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32  *val, INT32  def);
void xml_int32_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32  *val, int cnt, INT32 def);
void xml_uint32	(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT32 *val, UINT32 def);
void xml_double	(HANDLE hsetup, const char *name, EN_Xml_Action action, double *val, double def);
void xml_enum		(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32  *val, char *enumstr);
void xml_int64	(HANDLE hsetup, const char *name, EN_Xml_Action action, INT64  *val, INT64  def);
void xml_uint64	(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT64 *val, UINT64 def);
void xml_filetime(HANDLE hsetup, const char *name, EN_Xml_Action action, FILETIME *val);
void xml_double	(HANDLE hsetup, const char *name, EN_Xml_Action action, double *val, double def);
void xml_mac_addr	(HANDLE hsetup, const char *name, EN_Xml_Action action, INT64  *val, INT64  def);
void xml_binary	(HANDLE hsetup, const char *name, EN_Xml_Action action, void   *val, int size, int *len);
	
void xml_str_next	(HANDLE hsetup, HANDLE *attribute, char *name, int namesize, char  *val, int valsize);


#ifdef __cplusplus
}
#endif