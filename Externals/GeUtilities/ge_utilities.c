// *************************************************************************************************
//																				
//	ge_utilities: 
//																				
// *************************************************************************************************
//
//  This is a program that receives commands over tcp/ip and drives a eZ-Inkjet
//
//
//
// *************************************************************************************************
//
//    Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.		
//    Written by Marcel Galliker.												
//																				
//
// *************************************************************************************************

#include "ge_utilities.h"
#include "ge_trace.h"

//--- char_to_wchar --------------------------------------------------------------
void char_to_wchar(wchar_t *puni, const char *pansi, int len)
{
	while (len>=0)
	{
		*puni = (unsigned char)*pansi;
		puni++;
		len--;
		if (*pansi==0) break;
		*pansi++;
	}
}

//--- TimetToFileTime -----------------------------------------------------
#ifdef WIN32
void TimetToFileTime( time_t t, LPFILETIME pft )
{
	LONGLONG ll = Int32x32To64(t, 10000000) + 116444736000000000;
	pft->dwLowDateTime = (DWORD) ll;
	pft->dwHighDateTime = ll >>32;
}
#endif

//--- FiletimeToTimet --------------------------------------------------
#ifdef WIN32
time_t FiletimeToTimet(const LPFILETIME pft)
{
	ULARGE_INTEGER ull;
	ull.LowPart = pft->dwLowDateTime;
	ull.HighPart = pft->dwHighDateTime;

	return ull.QuadPart / 10000000ULL - 11644473600ULL;
}
#else
time_t FiletimeToTimet(const UINT64 fileTime)
{
	return fileTime / 10000000ULL - 11644473600ULL;
}
#endif

//--- char_to_lower -------------------------------------------------
char* char_to_lower(const char *str, char *out)
{
	while (*str)
	{
		if (*str>='A' && *str<='Z') *out++=(*str++)+'a'-'A';
		else *out++=*str++;
	}

	*out=0;
	return out;
}

//--- strstart -------------------------------------
int strstart(char *str, char *start)
{
	if (str==NULL) return 0;
	int len=(int)strlen(start);
	if (strncmp(str, start, len)==0) 
		return len;
	return 0;
}

//--- strend --------------------------------------
int strend(char *str, char *end)
{
	int len=(int)strlen(str);
	int endlen=(int)strlen(end);
	if (endlen>len) return FALSE;
#ifdef linux
	if (strcmp(&str[len - endlen], end) == 0) return TRUE;
#else
	if (stricmp(&str[len-endlen], end)==0) return TRUE;
#endif
	return FALSE;
}

#ifdef linux
int stricmp(const char *str1, char *str2)
{
	while (*str1 && *str2)
	{
		char a = (*str1++)&0x3F;
		char b = (*str1++)&0x3F;	
		if (a<b) return -1;
		if (a>b) return 1;
	}
	return 0;
}
#endif
//--- wtoi ----------------------------
int  wtoi(UINT16 *wstr)
{
	int value=0;	
	UINT16 *ch;
	for (ch=wstr; *ch; ch++)
	{
		if (*ch==L'\0') break;
		if (*ch==L' ' && value==0) continue;
		if (*ch>=L'0' && *ch<=L'9') value=10*value+*ch-L'0';
		else break;
	}
	return value;
}

//--- ge_strncpy -------------------------------
char *ge_strncpy(char *dst, char* src, int maxDstLen)
{
	strncpy(dst, src, maxDstLen);
	dst[maxDstLen-1]=0;
	return dst;
}

//--- ge_getline ---------------
char *ge_getline(char *str)
{
	char *start;
	for(start=str; *str; str++)
	{
		if (*str=='\n')
		{
			*str=0;
			return start;
		}
	}
	return start;
}

//--- win_error ----------------
char * win_error(int err, char *str, int size)
{
#ifdef linux
	strncpy(str, strerror(err), size);
	return str;
#else
	int len = sprintf(str, "%d: ", err);
	len += FormatMessageA(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, err, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), &str[len], size-len, NULL);
	str[len-2]=0; // remove cr/lf
	return str;	
#endif
}

//--- swap -----------------------------------
UINT32 swap(UINT32 *value)
{
	BYTE *b=(BYTE*)value;
	BYTE n;
	n=b[0]; 
	b[0]=b[3];
	b[3]=n;

	n=b[1]; 
	b[1]=b[2];
	b[2]=n;

	return *b;
}

//--- bin2hex ------------------------------
char *bin2hex(char *str, void *data, int len)
{
	UINT8 *src=(UINT8*)data;
	char *dst = str;
	dst+= sprintf(dst, "%03d ", len);
	int i;
	for(i=0; i<len; i++, src++)
	{
		dst+=sprintf(dst, "%02x ", *src);
	}
	return str;
}

//--- hex2bin ------------------------------
void *hex2bin(char *str, void *data, int len)
{
	UINT8 *dst=(UINT8*)data;
	int val;
	if ((int)strlen(str)<4+3*len) 
	{
	//	TrPrintf(-1, "hex2bin Error 1: strlen=%d, size=%d, len=%d", strlen(str), len, 4+3*len);
		return NULL;
	}
	if (atoi(str)!=len) 
	{
	//	TrPrintf(-1, "hex2bin Error 2");
		return NULL;
	}

	str += 4;
	int i;
	for(i=0; i<len; i++, str+=3)
	{
		sscanf(str, "%02x", &val);
		*dst++ = val;
	}
	return data;
}

