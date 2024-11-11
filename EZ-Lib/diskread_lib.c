// *************************************************************************************************
//																				
//	diskread_lib.c: 
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


#include <time.h>

#include "ge_file.h"
#include "ge_threads.h"
#include "ge_time.h"
#include "ge_trace.h"
#include "ge_utilities.h"
#include "key.h"
#include "EzGlobals.h"
#include "diskread_lib.h"

//--- defines -----------------------------------------

// format printed record: rrrrrrrrr dd-mmm-yyyy hh:mm:ss<nl>
#define TIMESTAMP_LEN 32

//--- statics -----------------------------------------
static FILE *_DataFile=NULL;
static FILE *_PrintedFile=NULL;
static FILE *_TcpFile=NULL;
static INT32 _PrintedFile_StartPos;
static char _Separator;
static int  _FieldCnt;
static int  _RecordCnt;
static int	_RecordNo;
static int  _RecordNoEnd;
static int	_ConvertFromPC8;
static time_t _DataFile_time;
static int	  _MMin;
static int	  _CpH;
static int	  _DpiX;
static int	  _DpiY;
static 	time_t _Now;
static char _FieldName[MAX_FIELDS][MAX_FIELD_LEN];
static char _DataFields[MAX_FIELDS][MAX_FIELD_LEN];
static char _FieldNotFound[MAX_FIELDS][64];
static int  _FieldErrCheck;
static int	_FieldErrCnt;
static INT64	_FileSize;
static HANDLE _Diskread_sem = NULL;

static time_t _Keys_time;

//--- prorotypes ---------------------------------------
static char* _get_datetime_str(char *fmt, char *add, int *pskip, char *buf);
static char* _get_recno_str(char *buf);
static char* _get_name_data(char *fieldname, char *next, int *pskip, char *buf);

static void _open_printed_file(char *path);
static char *_format_record_printed(int filepos, int printed, char *str);
static void _handle_pdf(char *path);
static void _handle_header(char *record);
static int  _separate_fields(char *record, char field[MAX_FIELDS][MAX_FIELD_LEN], int *perr);
static void _keys_init_thread(void *par);
static void _test(void);

static void _convert_from_pc8(UCHAR *str);

//--- diskread_test --------------------------------------------
void diskread_test(char *path)
{
	int time0=ge_ticks();
	int cnt=1000;
	int err;
	TrPrintf(TRUE, "diskread_test >>%s<<, %d times", path, cnt);
	for (int i=0; i<cnt; i++)
	{
		diskread_open(path, FALSE);
		do
		{
			diskread_seek_next();
		}
		while(diskread_read_record(&err)>=0);
		diskread_close();
	}
	time0 = ge_ticks()-time0;
	TrPrintf(TRUE, "time=%d ms", time0);
}

//--- diskread_close ------------------------------------
void diskread_close(void)
{
	if (_Diskread_sem==NULL)
	{
		_Diskread_sem=ge_sem_create();
		ge_sem_post(_Diskread_sem);
	//	TrPrintf(-1, "ge_sem_create(0x%08x), line=%d", _Diskread_sem, __LINE__);
	}
//	TrPrintf(-1, "ge_sem_wait(0x%08x), line=%d", _Diskread_sem, __LINE__);
	int ret=ge_sem_wait(_Diskread_sem, 0);
//	TrPrintf(-1, "diskread_close 0x%08x, sem=%d", _DataFile, ret);
	if (_DataFile!=NULL) fclose(_DataFile);
	_DataFile=NULL;

	if (_PrintedFile!=NULL) fclose(_PrintedFile);
	_PrintedFile=NULL;
//	TrPrintf(-1, "diskread_close 0x%08x", _DataFile);
//	TrPrintf(-1, "ge_sem_post(0x%08x), line=%d", _Diskread_sem, __LINE__);
	ge_sem_post(_Diskread_sem);
}

//--- diskread_open -----------------------------------
int diskread_open(char *path, int convertFromPC8)
{
	//static int first=TRUE;
	//if (first) TrInit("Dispkead", 0, "D:\\Temp\\diskread.txt");
	//first=FALSE;
	
	char record[MAX_RECORD_SIZE];
	diskread_close();
//	TrPrintf(-1, "ge_sem_wait(0x%08x), line=%d", _Diskread_sem, __LINE__);
	int ret=ge_sem_wait(_Diskread_sem, 0);
//	TrPrintf(-1, "diskread_open 0x%08x, sem=%d", _DataFile, ret);
//	TrPrintf(-1, "diskread_open");
	_DataFile = ge_fopen(path, "r", _SH_DENYNO);
	_ConvertFromPC8 = convertFromPC8;
	if (_DataFile!=NULL && fgets(record, sizeof(record), _DataFile))
	{
		_handle_pdf(path);
		_handle_header(record);
		_RecordNo = 1;
		_DataFile_time = ge_file_get_mtime(path);

		_open_printed_file(path);

//		TrPrintf(-1, "diskread_open OK");
//		TrPrintf(-1, "ge_sem_post(0x%08x), line=%d", _Diskread_sem, __LINE__);

		ge_sem_post(_Diskread_sem);
		return REPLY_OK;
	}
	//--- error --------------------------
//	TrPrintf(-1, "ge_sem_post(0x%08x), line=%d", _Diskread_sem, __LINE__);
	ge_sem_post(_Diskread_sem);
	diskread_close();
//	TrPrintf(-1, "diskread_open ERROR");
	return REPLY_ERROR;
}

//--- _open_printed_file ----------------------
static void _open_printed_file(char *path)
{
	char printedPath[MAX_PATH];
	char dir[MAX_PATH];
	char name[MAX_PATH];
	char header[MAX_PATH];
	char ext[10];

	ge_split_path(path, dir, name, ext);
	sprintf(printedPath, "%s%s.done.csv", dir, name);

	_PrintedFile_StartPos = sprintf(header, "Pos;Date;Time\r\n");
	if (ge_file_exists(printedPath) && ge_file_get_mtime(printedPath)>ge_file_get_mtime(path)) 
	{
		_PrintedFile = ge_fopen(printedPath, "r+b", _SH_DENYNO);
		//--- check format ---
		char record[MAX_PATH];
		memset(record, 0, sizeof(record));
		fgets(record, sizeof(record), _PrintedFile);
		if (!strcmp(record, header))
		{
			fseek(_PrintedFile, 0, SEEK_END);
			int filepos=ftell(_PrintedFile);
			_RecordCnt = (filepos-_PrintedFile_StartPos) / TIMESTAMP_LEN;
		}
		else 
		{
			fclose(_PrintedFile);
			_PrintedFile = NULL;
		}
	}
	if (_PrintedFile==NULL)
	{
		_PrintedFile = ge_fopen(printedPath, "w+b", _SH_DENYNO);
		fprintf(_PrintedFile, header);
		fflush(_PrintedFile);
		if (_DataFile!=NULL)
		{
			int filepos=ftell(_DataFile);
			int time = ge_ticks();
			char record[MAX_RECORD_SIZE];
			sprintf(name, "123456789;;                               ");
			sprintf(&name[TIMESTAMP_LEN-2], "\r\n");
			_RecordCnt=0;
			while (fgets(record, sizeof(record), _DataFile))
			{
				_RecordCnt++;
				for (int n=0, p=filepos; n<9; n++)  // sprintf takes too long
				{
					name[8-n] = '0'+p%10;p=p/10;
				}
				fwrite(name, 1, TIMESTAMP_LEN, _PrintedFile);
				filepos+=(int)strlen(record)+1;		// ftell takes too long
				if (_RecordCnt%1000==0) TrPrintf(-1, "Done.Record %d, time=%d", _RecordCnt, ge_ticks()-time);
			}
			TrPrintf(1, "RecCnt=%d time=%d", _RecordCnt,  ge_ticks()-time);
		}

		fflush(_PrintedFile);
	}
	TrPrintf(1, "RecCnt=%d", _RecordCnt);
}

//--- diskread_fieldCnt -----------------
int	diskread_fieldCnt(void)
{
	return _FieldCnt;
}

//--- diskread_recordCnt -----------------
int	diskread_recordCnt(void)
{
	return _RecordCnt;
}

//--- diskread_fieldName ------------------------
void diskread_fieldName(int fieldNo, char* name)
{
	if (fieldNo>=0 && fieldNo<_FieldCnt) strcpy(name, _FieldName[fieldNo]);
}

//--- diskread_start_recNo -------------------------------------
int   diskread_start_recNo(int start, int end)
{
	if (start<1) start=1;
	_RecordNo = start;
	_RecordNoEnd = end;
	return _RecordNo;
}

//--- diskread_set_recNo ---------------------------------------------
int	 diskread_set_recNo(int recNo)
{
	_RecordNo = recNo;
	return _RecordNo;
}

//--- diskread_get_recNo ----------------------------
int   diskread_get_recNo(void)
{
	return _RecordNo;
}

//--- diskread_set_end_recNo ---------------------------------------------
int	 diskread_set_end_recNo(int recNo)
{
	_RecordNoEnd = recNo;
	return _RecordNoEnd;
}

//--- diskread_set_speed ------------------------------
void diskread_set_speed(int mmin, int cph)
{
	_MMin = mmin;
	_CpH  = cph;
}

//--- diskread_set_dpi ------------------------------
void diskread_set_dpi(int dpiX, int dpiY)
{
	_DpiX = dpiX;
	_DpiY = dpiY;
}

//--- diskread_seek -----------------------------------
int diskread_seek(int recNo, int filePos)
{
	if (!_DataFile) return 0;
//	TrPrintf(-1, "ge_sem_wait(0x%08x), line=%d", _Diskread_sem, __LINE__);
	int ret=ge_sem_wait(_Diskread_sem, 0);
	int pos = 0;
	if (_DataFile) 
	{
//		TrPrintf(-1, "diskread_seek (recNo=%d, filePos=%d, _DataFile=0x%08x, sem=%d)", recNo, filePos, _DataFile, ret);
		pos=ftell(_DataFile);
		if (recNo>0) _RecordNo = recNo;
		fseek(_PrintedFile, _PrintedFile_StartPos+(recNo-1)*TIMESTAMP_LEN, SEEK_SET);
		fscanf(_PrintedFile, "%d", &filePos);
		fseek(_DataFile, filePos, SEEK_SET);
	}
//	TrPrintf(-1, "ge_sem_post(0x%08x), line=%d", _Diskread_sem, __LINE__);

	ge_sem_post(_Diskread_sem);
	return pos;
}

//--- diskread_create_file ------------------------------
void  diskread_create_file(char *path, char *header)
{
	_handle_header(header);

	_TcpFile = ge_fopen(path, "w", _SH_DENYNO);
	int cnt=0;
	if (_TcpFile!=NULL)
	{
		int len = (int)strlen(header);
		cnt=(int)fwrite(header, 1, len, _TcpFile);
		if (header[len]!='\n')
		{
			char ch='\n';
			fwrite(&ch, 1, 1, _TcpFile);
		}
		fflush(_TcpFile);
		_open_printed_file(path);
	}
}

//--- _handle_pdf -----------------------------------
static void _handle_pdf(char *path)
{

}

//--- _handle_header ----------------------------------
static void _handle_header(char *record)
{
	char sep[4] = {'\t', ';', ',', '~'};
	int  cnt[4];
	int len, i;
	
	while (*record<'0' || *record>'z') record++;

	//--- count separfators -----------
	memset(&cnt, 0, sizeof(cnt));
	char *pch;
	for (pch=record, len=0; *pch; pch++, len++)
	{
		for (i=0; i<SIZEOF(sep); i++)
			if (*pch==sep[i]) cnt[i]++;
	}
	
	//--- select the most often ------
	len=0;
	for (i=0; i<SIZEOF(sep); i++)
	{
		if (cnt[i]>len)
		{
			len=cnt[i]; 
			_Separator=sep[i];
		}
	}
	TrPrintf(1, "Saparator ='%c'\n", _Separator);

	//--- split the filed names --------------------------
	int err;
	memset(_FieldName, 0, sizeof(_FieldName));
	int fields = _separate_fields(record, _FieldName, &err);
	for (_FieldCnt=0; _FieldCnt<fields && *_FieldName[_FieldCnt]; _FieldCnt++)
	{
		TrPrintf(1, "FieldName[%d]=>>%s<<\n", _FieldCnt, _FieldName[_FieldCnt]);
	}
	_FieldErrCnt=0;
	_FieldErrCheck=TRUE;
}

//--- _separate_fields -----------------------------------------------
static int _separate_fields(char *record, char fields[MAX_FIELDS][MAX_FIELD_LEN], int *perr)
{
	char *pch;
	int quote=FALSE;
	char *field;
	char fieldCnt=0;

	field=(char*)&fields[fieldCnt];
	int size=MAX_FIELD_LEN;
	for (pch=record; ;pch++)
	{
		if (*pch=='\0') break;
		if (quote)
		{
			if (*pch == '\"') quote = FALSE;
			else 
			{
				*field++=*pch;
				size--;
			}
		}
		else
		{
			if (*pch == '\"') quote = TRUE;
			else if (size==1)
			{
				*field++=0;
				if (*perr==0) *perr=fieldCnt;
				fieldCnt++;
				field=(char*)&fields[fieldCnt];
				size=MAX_FIELD_LEN;
			}
			else if (*pch==_Separator || *pch=='\n' || *pch=='\r')// || *pch=='\0') 
			{
				*field++=0;
				fieldCnt++;
				if (fieldCnt>=MAX_FIELDS) 
				{
					TrPrintf(-1, "Too may fields, max=%d", MAX_FIELDS);
					break;
				}
				field=(char*)&fields[fieldCnt];
				size=MAX_FIELD_LEN;
				if (*pch=='\n' || *pch=='\r')
					while(pch[1]=='\n' || pch[1]=='\r') pch++;
			}
			else 
			{
				*field++=*pch;
				size--;
			}
		}
		if (*pch=='\0') break;
	}
	int len=0;
	//--- trim spaces ----------------
	for (int i=0; i<fieldCnt; i++)
	{
		len=(int)strlen(fields[i]);
		while (len>0 && fields[i][len-1]==' ') fields[i][--len] = '\0';
	}
	return fieldCnt;
}

//--- diskread_seek_next ----------------------------
int diskread_seek_next(void)
{
	_FieldErrCheck = FALSE;
	int end= _RecordNoEnd? _RecordNoEnd : _RecordCnt;
	if (_RecordNo+1 <= end) return ++_RecordNo;
	return -1;
}

//--- diskread_set_record ----------------------------
int diskread_set_record(char *record)
{
	int err;
	int cnt=_separate_fields(record, _DataFields, &err);
	_RecordNo++;
	if (_TcpFile!=NULL)
	{
		if (_RecordNo<10) 
		{
			fseek(_TcpFile, 0, SEEK_END);
			int filepos=ftell(_TcpFile);
			int len=(int)strlen(record);
			fwrite(record, 1, len, _TcpFile);
			if (record[len]!='\n')
			{
				char ch='\n';
				fwrite(&ch, 1, 1, _TcpFile);
			}
			fflush(_TcpFile);

			//--- update printed file-position ---
			{
				char stamp [64];
				int pos= _PrintedFile_StartPos +(_RecordNo-1)*TIMESTAMP_LEN;
				snprintf(stamp, sizeof(stamp)-1, "%10d;;                                          ", filepos);
				sprintf(&stamp[TIMESTAMP_LEN-2], "\r\n");
				fseek(_PrintedFile, pos, SEEK_SET);
				TrPrintf(-1, "Update record=%d, pos=>>%s<<", _RecordNo, stamp);
				fwrite(stamp, 1, TIMESTAMP_LEN, _PrintedFile);
				fflush(_PrintedFile);
			}
		}
		else
		{
			fclose(_TcpFile);
			_TcpFile=NULL;
		}
	}
	return err;
}

//--- diskread_read_record ----------------------------
int diskread_read_record(int *err)
{
	*err=0;
	if (_DataFile==NULL)
	{
		return _RecordNo;
	}
	else
	{
		char record[MAX_RECORD_SIZE];
		int filepos=ftell(_DataFile);
		memset(record, 0, MAX_RECORD_SIZE);
		if (fgets(record, sizeof(record), _DataFile))
		{
			int len=(int)strlen(record);
			if (_ConvertFromPC8) _convert_from_pc8(record);
			int cnt=_separate_fields(record, _DataFields, err);
			if (FALSE) //--- trace fields ----------------------
			{
				char data[MAX_FIELD_LEN];
				TrPrintf(1, "Data Record[%d]:", _RecordNo);
				for (int i=0; i<cnt; i++)
				{
					diskread_get_no_data(i, data, sizeof(data));
					TrPrintf(1, "   %s=>>%s<<", _FieldName[i], data);
				}
			}
			return filepos;
		}
	}
	return -1;
}

//--- diskread_keys_init -----------------------------------
void  diskread_keys_init(int fieldNo)
{
	if (fieldNo>0 && _DataFile_time!=_Keys_time)
	{
		_Keys_time = 0;
	//	_test();
		CreateThread ( 
			NULL,									// no security attributes 
			0,										// default stack size 
			(LPTHREAD_START_ROUTINE) &_keys_init_thread,// function to call
			#pragma warning (suppress:4022)
			fieldNo,							// parameter for function
			0,										// 0=thread runs immediately after being called
			NULL									// returns thread identifier
		);
	}
}

//--- diskread_keys_ready --------------------------------------
int diskread_keys_ready(void)
{
	return (_Keys_time!=0);
}
//--- _test ----------------------------------------
static void _test(void) 
{
	int cnt=1000;
	int time0=ge_ticks();
	int val;
	char key[64];
	_Keys_time = 0;
	
	key_clear();
	for (int i=0; i<cnt; )
	{
		val=rand()%10000;
		sprintf(key, "%05d", val);
		TrPrintf(1, "key[%d]=%s", i, key);
		if (key_add(i, val, key)==REPLY_OK) i++;
	}

	key_trace();
	_Keys_time = _DataFile_time;
}

//--- _keys_init_thread ----------------------------------------
static void _keys_init_thread(void *par)
{
	#pragma warning (suppress:4311)
	int fieldNo = (int)par;
	fseek(_DataFile, 0, SEEK_SET);
	char record[MAX_RECORD_SIZE];
	int recNo=0;
	int filepos=0;
	int err;
	int time0=ge_ticks();
	key_clear();
	while(fgets(record, sizeof(record), _DataFile))
	{
		if (filepos>0)
		{
			int cnt=_separate_fields(record, _DataFields, &err);
			key_add(recNo, filepos, _DataFields[fieldNo-1]);
		}
		filepos =ftell(_DataFile); 
		recNo++;
	}
	TrPrintf(0, "diskread_init_keys time=%d", ge_ticks()-time0);

	//--- Test with 100-contacts.csv ----------------
	if (TRUE)
	{
		key_find("delmy.ahle@hotmail.com", &recNo, &filepos);
		if (recNo>0)
		{
			diskread_seek(recNo, filepos);
			diskread_read_record(&err);
			TrPrintf(0, "%s, %s, %s, %s", _DataFields[1], _DataFields[2], _DataFields[3], _DataFields[11]);
		}
	}
	_Keys_time = _DataFile_time;
}

//--- diskread_seek_key -----------------------------------
int  diskread_seek_key(char *key)
{
	int ret;
	int recNo=0;
	int filepos=0;
	if (key_find(key, &recNo, &filepos)==REPLY_OK)
	{
		ret=diskread_seek(recNo, filepos);
	}
	else ret=-1;
	return ret;
}

//--- _convert_from_pc8 --------------------------------------------
static void _convert_from_pc8(UCHAR *str)
{
	// texts are sent as codepage 437
	//	https://de.wikipedia.org/wiki/Codepage_437

	UCHAR table[256] =	
	{	/* 0x0. */	 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f,
		/* 0x1. */	 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f,
		/* 0x2. */	 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
		/* 0x3. */	 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f,
		/* 0x4. */	 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
		/* 0x5. */	 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
		/* 0x6. */	 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f,
		/* 0x7. */	 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
		/* 0x8. */	 0xc7, 0xfc, 0xe9, 0xe2, 0xe4, 0xe0, 0xe5, 0xe7, 0xea, 0xeb, 0xe8, 0xef, 0xee, 0xec, 0xc4, 0xc5,
		/* 0x9. */	 0xc9, 0xe6, 0xc6, 0xf4, 0xf6, 0xf2, 0xfb, 0xf9, 0xff, 0xd6, 0xdc, 0xa2, 0xa3, 0xa5, 0x9e, 0x9f,
		/* 0xa. */	 0xe1, 0xed, 0xf3, 0xfa, 0xf1, 0xd1, 0xaa, 0xba, 0xbf, 0xa9, 0xac, 0xdb, 0xac, 0xad, 0xae, 0xaf,
		/* 0xb. */	 0xb0, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbd, 0xbc, 0xa1, 0xab, 0xbb,
		/* 0xc. */	 0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,	
		/* 0xd. */	 0xd0, 0xd1, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdd, 0xde, 0xdf,
		/* 0xe. */	 0xe0, 0xdf, 0xe2, 0xe3, 0xe4, 0xb5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
		/* 0xf. */	 0xf0, 0xb1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf7, 0xf7, 0xb0, 0xf9, 0xb7, 0xfb, 0xfc, 0xb2, 0xfe, 0xff,
	};

	for (int i=0;str[i]; i++)
	{
		str[i]=table[str[i]];
	}
}

//---diskread_get_actual_text -------------------------------
int diskread_get_actual_text(char *in, char* out, int outLen, int removeSpaces)
{
	char *pch;
	char *dst=out;
	char *fieldStart=NULL;
	char *test;
	char *lineStart=NULL;
	int skip;
	int	filter_empty=FALSE;

	memset(out, 0, outLen);

	time(&_Now);

	for (pch=in; *pch; pch++)
	{
		if (lineStart==NULL) lineStart=dst;
		if (*pch == FieldStart) 
			fieldStart = pch+1;
		else if (*pch == FieldEnd)
		{
			*pch='\0';
			test=fieldStart;
			fieldStart=_get_name_data(fieldStart, pch+1, &skip, dst);
			int len = (int)strlen(dst);
			if (len==0 && removeSpaces)
			{
				if (dst[-1]==' ') dst--;
			}
			else dst += len;
			*pch=FieldEnd;
			pch+=skip;
			fieldStart=NULL;
		}
		else if (fieldStart==NULL) 
		{
			/*
			if (*pch=='\n') *dst++='\0';
			else if (*pch!='\r') 
			*/
			if (filter_empty)
			{
				if (pch[0]=='\r' && pch[1]=='\n')
				{
					if (*lineStart)
					{
						*dst++ = '\r';
						*dst++ = '\n';
						*dst = 0;
					}
					pch++;
					lineStart = NULL;
				}
				else *dst++ = *pch;
			}
			else
			{
				*dst++ = *pch;
			}
		}
	}
	*dst++='\0';
	*dst++='\0';

	return 0;
}

//--- diskread_get_field_errors ---------------------
int diskread_get_field_errors(char fieldNotFound[MAX_FIELDS][64])
{
	memcpy(fieldNotFound, _FieldNotFound, sizeof(_FieldNotFound));
	int cnt=_FieldErrCnt;
	_FieldErrCnt = 0;
	return cnt;
}

//--- _format_record_printed ---------------------------------
static char *_format_record_printed(int filepos, int printed, char *str)
{
	char date[16]="           ";
	char time[16]="        ";
	if (printed)
	{
		ge_get_system_date_str(date, '-');
		ge_get_system_time_str(time);
	}
	sprintf(str, "%09d;%s;%s      ", filepos, date, time);
	sprintf(&str[TIMESTAMP_LEN-2], "\r\n");

	return str;
}

//--- diskread_set_record_printed ------------------------
void  diskread_set_record_printed(int recNo)
{
	if (_PrintedFile!=NULL && recNo>0 && recNo<=_RecordCnt)
	{
		char date[16];
		char time[16];
		char stamp[64];
		int pos= _PrintedFile_StartPos +(recNo-1)*TIMESTAMP_LEN;

		fseek(_PrintedFile, pos, SEEK_SET);
		fread (stamp, 1, TIMESTAMP_LEN, _PrintedFile);

		ge_get_system_date_str(date, '-');
		ge_get_system_time_str(time);
	//	TrPrintf(-1, "Record[%d].Printed at >>%s;%s<<", recNo, date, time);
		sprintf(&stamp[10], "%s;%s      ", date, time);
		sprintf(&stamp[TIMESTAMP_LEN-2], "\r\n");
		fseek(_PrintedFile, pos, SEEK_SET);
		fwrite(stamp, 1, TIMESTAMP_LEN, _PrintedFile);
		fflush(_PrintedFile);
	}
}

//--- diskread_get_record_printed ---------------------------
int diskread_get_record_printed(int recNo)
{
	if (_PrintedFile!=NULL)
	{
		char stamp[64];
		int pos=_PrintedFile_StartPos +(recNo-1)*TIMESTAMP_LEN;
		fseek(_PrintedFile, pos, SEEK_SET);
		fgets(stamp, sizeof(stamp), _PrintedFile);
		return (atoi(&stamp[10])>0);
	}

	return FALSE;
}

//--- _get_datetime_str -------------------------
static char* _get_datetime_str(char *fmt, char *add, int *pskip, char *buf)
{
	time_t now;
	struct tm* now_tm;
	int val=0;
	int addTime = 0;

	while(sscanf(add, "+%d", &val)==1)
	{
		*pskip = 1;
		while (TRUE)
		{
			if (*add=='D')
			{
				addTime += (24*60*60)*val;
				break;
			}
			else if (*add=='H')
			{
				addTime += (60*60)*val;
				break;
			}
			else 
			{
				add++; 
				(*pskip)++;
			}
		}
		val=0;
	}

	now = _Now + addTime;
	now_tm = localtime(&now);

	//see formats https://www.tutorialspoint.com/c_standard_library/c_function_strftime.htm

	if		(!strcmp(fmt, "HH"))			strftime(buf, 100, "%H",		now_tm);
	else if	(!strcmp(fmt, "mm"))			strftime(buf, 100, "%M",		now_tm);
	else if	(!strcmp(fmt, "HH:mm"))			strftime(buf, 100, "%H:%M",		now_tm);
	else if (!strcmp(fmt, "HH:mm:ss"))		strftime(buf, 100, "%H:%M:%S",	now_tm);
	else if (!strcmp(fmt, "DD.MM.YYYY"))	strftime(buf, 100, "%d.%m.%Y",	now_tm);
	else if (!strcmp(fmt, "DDD"))			strftime(buf, 100, "%j",		now_tm);
	else if (!strcmp(fmt, "DDD.YY"))		strftime(buf, 100, "%j.%y",		now_tm);
	else if (!strcmp(fmt, "DD.MM.YY"))		strftime(buf, 100, "%d.%m.%y",	now_tm);
	else if (!strcmp(fmt, "MM.YYYY"))		strftime(buf, 100, "%m.%Y",		now_tm);
	else if (!strcmp(fmt, "MM.YY"))			strftime(buf, 100, "%m.%y",		now_tm);
	else if (!strcmp(fmt, "Y"))				sprintf(buf, "%d",				now_tm->tm_year%10);
	else if (!strcmp(fmt, "Y"))				strftime(buf, 100, "%y",		now_tm);
	else if (!strcmp(fmt, "YYYY"))			strftime(buf, 100, "%Y",		now_tm);
	else if (!strcmp(fmt, "shift(6-6)"))			
	{
		if (now_tm->tm_hour>=6 && now_tm->tm_hour<18) strcpy(buf, "4");
		else strcpy(buf, "5");
	}
	else sprintf(buf, "TM:%s", fmt);
	return buf;
}

//--- _get_recno_str -------------------------------
static char* _get_recno_str(char *buf)
{
	sprintf(buf, "%d", _RecordNo);
	return buf;
}

//--- _get_mmin_str -------------------------------
static char* _get_mmin_str(char *buf)
{
	sprintf(buf, "%d", _MMin);
	return buf;
}

//--- _get_cph_str -------------------------------
static char* _get_cph_str(char *buf)
{
	sprintf(buf, "%d", _CpH);
	return buf;
}

//--- _get_dpi_str -------------------------------
static char* _get_dpi_str(char *buf)
{
	sprintf(buf, "%dx%d", _DpiX, _DpiY);
	return buf;
}

//--- _get_name_data -----------------------------
static char* _get_name_data(char *fieldname, char *next, int *pskip, char *buf)
{
	/*
	static int first=TRUE;
	if (first) TrInit("Diskread", 0, "D:\\Temp\\diskread.txt");
	first=FALSE;
	*/

	int fmt;
	int i;
	*pskip=0;
	*buf=0;
	if (fieldname!=NULL)
	{
	//	TrPrintf(-1, "Fieldname >>%s<<", fieldname);
		if (     fmt=strstart(fieldname, "TM:")) return _get_datetime_str(&fieldname[fmt], next, pskip, buf);
		else if (fmt=strstart(fieldname, "DT:")) return _get_datetime_str(&fieldname[fmt], next, pskip, buf);
		else if (!strcmp(fieldname, "RecNo"))	 return _get_recno_str(buf);
		else if (!strcmp(fieldname, "m/min"))	 return _get_mmin_str(buf);
		else if (!strcmp(fieldname, "p/h"))		 return _get_cph_str(buf);
		else if (!strcmp(fieldname, "dpi"))		 return _get_dpi_str(buf);
		else
		{			
			for (i=0; i<MAX_FIELDS && *_FieldName[i]; i++)
			{
				if (!stricmp(fieldname, _FieldName[i]))
				{
				//	TrPrintf(-1, "data[%d] >>%s<<", i, _DataFields[i]);
					strcpy(buf, _DataFields[i]);
					return buf;
				}
			}
			if (_FieldErrCheck)
			{
			//	TrPrintf(-1, "field[%d] >>%s<< not found, _FieldErrCnt=%d", i, fieldname, _FieldErrCnt);
				int n;
				for (n=0; n<_FieldErrCnt; n++)
				{
					if (!strcmp(fieldname, _FieldNotFound[n])) break;
					if (!*_FieldNotFound[n])
					{
						strncpy(_FieldNotFound[_FieldErrCnt++], fieldname, sizeof(_FieldNotFound[0])-1);
						break;
					}
				}
				return buf;
			}
		}
	}
	return buf;
}

//--- diskread_get_no_data -----------------------------
char* diskread_get_name_data(char *fieldname, char *buf, int bufsize)
{
	*buf=0;
	for (int i=0; i<MAX_FIELDS; i++)
	{
		if (!stricmp(fieldname, _FieldName[i]))
		{
			strncpy(buf, _DataFields[i], bufsize-1);
			buf[bufsize-1]=0;
			return buf;
		}
	}
	return buf;
}

//--- diskread_get_no_data -----------------------------
void diskread_get_no_data(int fieldNo, char *buf, int bufsize)
{	
	if (fieldNo>=0 && fieldNo<_FieldCnt)
	{
		strncpy(buf, _DataFields[fieldNo], bufsize-1);
		buf[bufsize-1]=0;
	}
}