
// ****************************************************************************
//
//	ge_time.c
//
// ****************************************************************************
//
//	Copyright 2022 by Galliker-Engineering, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************


#include "ge_common.h"
#include "ge_time.h"

static const char *_MonthStr[12]= {"JAN", "FEB", "MAR", "APR", "MAI", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"}; 

//--- ge_get_date_time_str ----------------------------
int ge_get_date_time_str(time_t time, char *str)
{
	char separator='-';
	struct tm *lt;
#ifdef linux
	lt = localtime(&time);
#else
	lt=_localtime64(&time);
#endif
	if (lt==NULL) return sprintf(str, "????");
	return sprintf(str, "%02d%c%s%c%d - %02d-%02d-%02d", lt->tm_mday, separator, _MonthStr[lt->tm_mon], separator, 1900+lt->tm_year, lt->tm_hour, lt->tm_min, lt->tm_sec);
}

//--- ge_get_system_date_time_str ----------------------------
int ge_get_system_date_time_str(char *str)
{
#ifdef linux
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	char separator = '-';
	return sprintf(str, "%02d%c%s%c%d - %02d-%02d-%02d", tm.tm_mday, separator, _MonthStr[tm.tm_mon], separator, tm.tm_year + 1900, tm.tm_hour, tm.tm_min, tm.tm_sec);
#else
	SYSTEMTIME time;
	//	GetSystemTime(&time);
	GetLocalTime(&time);
	char separator='-';
	return sprintf(str, "%02d%c%s%c%d - %02d-%02d-%02d", time.wDay, separator, _MonthStr[time.wMonth-1], separator, time.wYear, time.wHour, time.wMinute, time.wSecond);
#endif
}

//--- ge_get_system_time_str ---------------------------
int ge_get_system_time_str(char *str)
{
#ifdef linux
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	return sprintf(str, "%d:%02d:%02d", tm.tm_hour, tm.tm_min, tm.tm_sec);
#else
	SYSTEMTIME time;
//	GetSystemTime(&time);
	GetLocalTime(&time);
	return sprintf(str, "%d:%02d:%02d", time.wHour, time.wMinute, time.wSecond);
#endif
}

//--- ge_get_system_numdate_str ---------------------------
int ge_get_system_numdate_str(char *str, char separator)
{
#ifdef linux
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	return sprintf(str, "%0d%c%d%c%d", tm.tm_mday, separator, tm.tm_mon+1, separator, tm.tm_year + 1900);
#else
	SYSTEMTIME time;
	//	GetSystemTime(&time);
	GetLocalTime(&time);
	return sprintf(str, "%0d%c%d%c%d", time.wDay, separator, time.wMonth, separator, time.wYear);
#endif
}

//--- ge_get_system_date_str ---------------------------
int ge_get_system_date_str(char *str, char separator)
{
#ifdef linux
	time_t t = time(NULL);
	struct tm tm = *localtime(&t);
	return sprintf(str, "%0d%c%d%c%d", tm.tm_mday, separator, _MonthStr[tm.tm_mon], separator, tm.tm_year + 1900);
#else
	SYSTEMTIME time;
//	GetSystemTime(&time);
	GetLocalTime(&time);
	return sprintf(str, "%02d%c%s%c%d", time.wDay, separator, _MonthStr[time.wMonth-1], separator, time.wYear);
#endif
}

//--- _get_day ------------------------
/*
static int _get_day(SYSTEMTIME *date)
{
	int daysPerMonth[]     = {31,28,31,30,31,30,31,31,30,31,30,31};
	int daysPerMonthLeap[] = {31,29,31,30,31,30,31,31,30,31,30,31};
	int doy=0;
	int m;
	int *dpm;
	if ((date->wYear%4)==0) dpm=daysPerMonthLeap;
	else dpm=daysPerMonth;
	for (m=1; m<date->wMonth; m++)
	{
		doy += dpm [m-1];
	}
	doy += date->wDay;
	return doy;
}
*/

//--- ge_get_days_between ------------------------------------
/*
int ge_get_days_between(SYSTEMTIME *from, SYSTEMTIME *to)
{
	int from_doy;
	int to_doy;
	int days = 0;

	from_doy = _get_day(from);
	to_doy   = _get_day(to);
	days     = to_doy-from_doy;
	if (from->wYear<=to->wYear)
	{
		for(int y=from->wYear; y<to->wYear; y++)
		{
			if (y%4) days+=365;
			else     days+=366;
		}
	}
	else 
	{
		days -= 1000;
	}
	return days;
}
*/

//--- ge_ticks ----------------------------------------------------------
#ifdef linux
	static UINT64 sTick0 = 0; // ms
	int ge_ticks()
	{
		if (sTick0 == 0)
		{
			struct timespec tp;
			clock_gettime(0, &tp);
			sTick0 = 1000*tp.tv_sec + tp.tv_nsec / 1000000;
		}
		struct timespec tp;

		clock_gettime(0, &tp);
		return (1000*tp.tv_sec + tp.tv_nsec / 1000000) - sTick0;
	}
#else
	static int _Tick0 = 0;
	int ge_ticks()
	{
		if (!_Tick0) _Tick0 = GetTickCount();
		return GetTickCount()-_Tick0;
	}
#endif