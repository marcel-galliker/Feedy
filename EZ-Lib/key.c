// *************************************************************************************************
//																				
//	key.c: 
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

// DOC: open MP https://learn.microsoft.com/en-us/archive/msdn-magazine/2005/october/openmp-and-c-reap-the-benefits-of-multithreading-without-all-the-work

#include "ge_common.h"
#include <omp.h>
#include "key.h"


//--- dfeines -------------------------------
#define SORT_TABLE_BLK 1024

//--- statics ------------------------------- 

typedef struct
{
	int recNo;
	int filePos;
	char *key;
} SSortItem;

static SSortItem *_Keys = NULL;
static int		  _KeysSize=0;
static int		  _KeysMax=0;

//--- prototypes -----------------------------
static int _expand_to(int cnt);
static int _key_find_idx(char *key, int *idx);

//--- key_clear ---------------
void key_clear(void)
{
	for(int i=0; i<_KeysSize; i++)
	{
		#pragma warning (suppress:6001)
		if (_Keys[i].key) free(_Keys[i].key);
	}
	_KeysMax = 0;
}

//--- key_trace ----------------------------------
void key_trace(void)
{
	printf("--- Trace %d keys: ---------------------------------\n", _KeysMax);
	int last=0;
	for (int i=0; i<_KeysMax; i++)
	{
		printf("Key[%d]=>>%s   diff=%d\n", i, _Keys[i].key, _Keys[i].filePos-last);
		last=_Keys[i].filePos;
	}
	printf("--------------------------------------------------\n");

}


//--- key_add ------------------------
int key_add(int recNo, int filePos, char *key)
{
	int idx;
	if (_key_find_idx(key, &idx)==REPLY_OK)  
		return REPLY_ERROR;
	if (_expand_to(_KeysMax+1)) 
		return REPLY_ERROR;

	memcpy(&_Keys[idx+1], &_Keys[idx], (_KeysMax-idx)*sizeof(_Keys[0]));
	_Keys[idx].recNo	= recNo;
	_Keys[idx].filePos  = filePos;
	_Keys[idx].key		= _strdup(key);
	_KeysMax++;
	return REPLY_OK;
}

//--- key_find -------------------------
int  key_find(char *key, int *pRecNo, int *pFilePos)
{
	int idx;
	if (_key_find_idx(key, &idx)==REPLY_OK)
	{
		*pRecNo   = _Keys[idx].recNo;
		*pFilePos = _Keys[idx].filePos;
		return REPLY_OK;
	}

	*pRecNo   = 0;
	*pFilePos = 0;
	return REPLY_ERROR;
}

//--- _key_find_idx -------------------------
static int  _key_find_idx_linear(char *key, int *idx)
{
	for (*idx=0; *idx<_KeysMax; (*idx)++)
	{
		int res=strcmp(_Keys[*idx].key, key);
		if (res==0) return REPLY_OK;
		else if (res>0) break;
	}
	return REPLY_ERROR;
}

//--- _key_find_idx -------------------------
static int  _key_find_idx(char *key, int *idx)
{
	int from = 0;
	int to   = _KeysMax;
	int cmp;
	*idx=(from+to)/2;
	while(from<to)
	{
		*idx=(from+to)/2;
		cmp=strcmp(_Keys[*idx].key, key);
		if (cmp==0) 
			return REPLY_OK;
		if (cmp<0) from=(*idx)+1;
		else       to  =(*idx);
	}
	*idx=from;
	return REPLY_ERROR;
}

/*
//--- _key_find_idx -------------------------
static int  _key_find_idx(char *key, int *idx)
{
	int found=FALSE;
	int threadCnt=omp_get_max_threads();
	int size=(_KeysMax+threadCnt-1)/threadCnt;
#pragma omp parallel 
	{
		int start=size*omp_get_thread_num();
		int end  =start+size;
		if (end>_KeysMax) end=_KeysMax;
		for(int i=start; i<end && !found; i++)
		{
			if (!strcmp(_Keys[i].key, key)) 
			{
				found=TRUE;
			}
		}
	}
	if (found) return REPLY_OK;
	return REPLY_ERROR;
}
*/
//--- _expand_to --------------------------
static int _expand_to(int cnt)
{
	if (cnt>_KeysSize)
	{
		int size = ((cnt+SORT_TABLE_BLK-1)/SORT_TABLE_BLK)*SORT_TABLE_BLK;
		printf("_expand_to %d\n", size);
		SSortItem *table = (SSortItem*)malloc(size*sizeof(SSortItem));
		if (table==NULL) 
		{
			perror("sort.c::_expand_to: Error malloc");
			return REPLY_ERROR;
		}
		memset(table, 0, size*sizeof(SSortItem));
		if (_Keys!=NULL)
		{
			memcpy(table, _Keys, _KeysSize*sizeof(SSortItem));
			free(_Keys);
		}
		_Keys	  = table;
		_KeysSize = size;
	}
	return REPLY_OK;
}