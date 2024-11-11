// *************************************************************************************************
//																				
//	diskread.h: 
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

#define EXPORT EXTERN_C _declspec(dllexport) 

#ifdef __cplusplus
extern "C"{
#endif

#define MAX_FIELDS 64

EXPORT void diskread_test(char *path);

EXPORT int  diskread_open(char *path, int convertFromPC8);
EXPORT int  diskread_seek(int recNo, int filePos);
EXPORT int  diskread_seek_next(void);
EXPORT int  diskread_seek_key(char *key);
EXPORT void diskread_close(void);

EXPORT void  diskread_create_file(char *path, char *header);


EXPORT int   diskread_start_recNo(int start, int end);
EXPORT int   diskread_read_record(int *perr);
EXPORT int	 diskread_set_record(char *record);
EXPORT char* diskread_get_name_data(char *fieldname, char *buf, int bufsize);
EXPORT void  diskread_get_no_data(int fieldNo, char *buf, int bufsize);
EXPORT void  diskread_set_record_printed(int recNo);
EXPORT int   diskread_get_record_printed(int recNo);

EXPORT int	 diskread_recordCnt(void);

EXPORT int	 diskread_set_recNo(int recNo);
EXPORT void	 diskread_set_speed(int mmin, int cph);
EXPORT void	 diskread_set_dpi(int dpiX, int dpiY);
EXPORT int	 diskread_set_end_recNo(int recNo);

EXPORT int   diskread_get_recNo(void);
EXPORT int   diskread_get_actual_text(char *in, char *out, int outlen, int removeSpaces);
EXPORT int   diskread_get_field_errors(char fieldNotFound[MAX_FIELDS][64]);
EXPORT int   diskread_fieldCnt(void);
EXPORT void  diskread_fieldName(int fieldNo, char* name);

EXPORT void  diskread_keys_init(int fieldNo);
EXPORT int   diskread_keys_ready(void);

#ifdef __cplusplus
}
#endif