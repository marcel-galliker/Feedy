// ****************************************************************************
//
//	DIGITAL PRINTING - xml_file.cppp
//
// ****************************************************************************
//
//	Copyright 2013 by Radex AG, Switzerland. All rights reserved.
//	Written by Marcel Galliker
//
// ****************************************************************************

#include "tinyxml.h"
#include <stdlib.h> 
#include "ge_common.h"
#include "ge_file.h"
#include "ge_xml_file.h"


//--- defines -------------------
#define MAX_LEVELS 10

typedef struct
{
	char			ErrorStr[256];
	TiXmlDocument	*xml;
	int				level;
	TiXmlElement	*chapter[MAX_LEVELS];
	TiXmlElement	*actChapter;
} SSetupFile;

//--- xml_create -------------------------------------
HANDLE xml_create()
{
	SSetupFile *setup;
	setup = (SSetupFile*)malloc(sizeof(SSetupFile));
	if (setup)
	{
		memset(setup, 0, sizeof(SSetupFile));
		setup->xml = new TiXmlDocument();
		TiXmlDeclaration	*decl = new TiXmlDeclaration("1.0", "", "");
		setup->xml->LinkEndChild(decl);
		setup->level = -1;
	}
	return (HANDLE)setup;
}

//--- xml_destroy --------------------------------------
void xml_destroy(HANDLE hsetup)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	delete(setup->xml);
	free(hsetup);
}

//--- xml_get_error_str -------------------------------------------------
char* xml_get_error_str(HANDLE hsetup)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	return setup->ErrorStr;
}

//--- xml_load --------------------------------------------
int xml_load(HANDLE hsetup, const char *filename)
{	
	SSetupFile *setup=(SSetupFile*)hsetup;
	
	if (!ge_file_exists(filename)) 
	{
		return REPLY_ERROR;
	}

	if (!setup->xml->LoadFile(filename))
	{
		strncpy(setup->ErrorStr, setup->xml->ErrorDesc(), sizeof(setup->ErrorStr));
		return REPLY_ERROR;
	}
	return REPLY_OK;
}

//--- xml_save ----------------------------------------------
int xml_save(HANDLE hsetup, const char *filename)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	ge_mkdir_path(filename);
	return setup->xml->SaveFile(filename);
}

//--- xml_to_str --------------------------------------------
char* xml_to_str(HANDLE hsetup)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	TiXmlPrinter printer;
	printer.SetIndent( "\t" );
	setup->xml->Accept(&printer);
	char *buffer=(char*)malloc(printer.Size()+1);
	strcpy(buffer, printer.CStr());
	return buffer;
}

//--- xml_chapter ---------------------------------------------
int xml_chapter(HANDLE hsetup, const char *name, int no, EN_Xml_Action action)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (!strcmp(name, ".."))
	{
		if (setup->level>0) setup->level--;
		setup->actChapter = setup->chapter[setup->level];
	}
	else
	{
		TiXmlNode		*node;
		TiXmlElement	*e;
		int				n;
		if (setup->level < 0) node = setup->xml->FirstChild(name);
		else				  node = setup->chapter[setup->level]->FirstChild(name);
		while(1)
		{
			if (node == NULL)
			{
				if (action == READ)return REPLY_ERROR;
				
				//---  add new node ---
				setup->level++;
				setup->chapter[setup->level] = new TiXmlElement(name);
				if (setup->level == 0)	setup->xml->LinkEndChild(setup->chapter[setup->level]);
				else setup->chapter[setup->level - 1]->LinkEndChild(setup->chapter[setup->level]);
				setup->actChapter = setup->chapter[setup->level];
				if (no >= 0) xml_int32(hsetup, "no", action, (INT32*)&no, 0);
				return REPLY_OK;
			}

			e = node->ToElement();
			n = 1234;
			e->Attribute("no", &n);
			if ((no<0 && n==1234) || n == no)
			{
				//--- node found ---
				if (action == READ)
				{
					setup->level++;
					setup->chapter[setup->level] = e;
					setup->actChapter = e;
					return REPLY_OK;
				}

				//--- remove node and add new empty node at next iteration ---
				if (setup->level<0)	setup->xml->RemoveChild(node);
				else				setup->chapter[setup->level]->RemoveChild(node);
				node=NULL; 
			}
			if (node) node = node->NextSibling(name);
		};
	}
	return REPLY_OK;
}

//--- xml_chapter_next -------------------------------------------
int xml_chapter_next(HANDLE hsetup, EN_Xml_Action action, char *name, int size)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	*name=0;
	if (action==READ)
	{
		if (setup->actChapter==NULL) setup->actChapter = (TiXmlElement*)setup->xml->IterateChildren(NULL);
		if (setup->actChapter!=NULL) setup->actChapter = (TiXmlElement*)setup->actChapter->NextSibling();				
		if (setup->actChapter==NULL) return REPLY_ERROR;
		strncpy(name, setup->actChapter->Value(), size);
	}
	return REPLY_ERROR;
}

//--- xml_chapter_add ---------------------------------------
int  xml_chapter_add	 (HANDLE hsetup, char *name)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	TiXmlNode *pnode = setup->actChapter;

	setup->level++;
	setup->chapter[setup->level] = setup->actChapter = new TiXmlElement(name);
	if (setup->level == 0)	setup->xml->LinkEndChild(setup->chapter[setup->level]);
	else setup->chapter[setup->level - 1]->LinkEndChild(setup->chapter[setup->level]);

	return REPLY_OK;		
}

//--- xml_chapter_delete ------------------------
int xml_chapter_delete(HANDLE hsetup)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	
	if (setup->actChapter!=NULL) 
	{
		TiXmlNode *pnode = setup->actChapter;
		TiXmlNode *pparent = pnode->Parent();
		
		pparent->RemoveChild(pnode);
		setup->actChapter = NULL;
//		(TiXmlNode*)(setup->actChapter)->Clear();
	}
	return REPLY_OK;
}

//--- xml_text ---------------------------------------------------------------
void xml_text	(HANDLE hsetup, EN_Xml_Action action,  char  *val, int size, const char* def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
	}
	else
	{
		const char *str = setup->actChapter->GetText();
		if (str) strncpy(val, str, size);
		else strncpy(val, def, size);
	}
}

//--- xml_str -----------------------------------------------------
void xml_str(HANDLE hsetup, const char *name, EN_Xml_Action action, char  *val, int size, const char* def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, val);
	else
	{
		const char *str = setup->actChapter->Attribute(name);
		if (str) strncpy(val, str, size);
		else strncpy(val, def, size);
	}
}

//--- xml_str_next ----------------------------------------------------------
void xml_str_next	(HANDLE hsetup, HANDLE *attribute, char *name, int namesize, char  *val, int valsize)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	TiXmlAttribute *attr=(TiXmlAttribute*)*attribute;
	*name=0;
	*val=0;
	if (attr==NULL) attr = setup->actChapter->FirstAttribute();
	else            attr = attr->Next();
	*attribute = attr;
	if (attr==NULL) return;
	strncpy(name, attr->Name(), namesize);
	strncpy(val, attr->Value(), valsize);
}

//--- xml_uchar ---------------------------------------------
void xml_uchar(HANDLE hsetup, const char *name, EN_Xml_Action action, UCHAR *val, UCHAR def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, *val);
	else
	{
		int i=def;
		setup->actChapter->Attribute(name, &i);
		*val=i;
	}
}

//--- xml_uchar_arr ------------------------------------------------------------------
void xml_uchar_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, UCHAR  *val, int cnt, INT32 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
		int i, len;
		int strsize = 10*cnt;
		char *str = (char*)malloc(strsize);
		memset(str, 0, strsize);
		for (i=0, len=0; i<cnt; i++) len+=snprintf(&str[len], strsize-len, "%d ", val[i]);
		setup->actChapter->SetAttribute(name, str);
	}
	else
	{
		int i, pos;
		for (i=0; i<cnt; i++) val[i]=def;
		const char *str = setup->actChapter->Attribute(name);
		if (str==NULL) return;
		for (i=0, pos=0; i<cnt; i++)
		{
			if (!str[pos]) break;
			val[i] = atoi(&str[pos]);
			while (str[pos] && str[pos]!=' ') pos++;
			pos++;
		}
	}
}

//--- xml_int16 ---------------------------------------------
void xml_int16(HANDLE hsetup, const char *name, EN_Xml_Action action, INT16 *val, INT16 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, *val);
	else
	{
		int i = def;
		setup->actChapter->Attribute(name, &i);
		*val = i;
	}
}

//--- xml_uint16 ---------------------------------------------
void xml_uint16(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT16 *val, UINT16 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, *val);
	else
	{
		int i = def;
		setup->actChapter->Attribute(name, &i);
		*val = i;
	}
}

//--- xml_int16_arr ------------------------------------------------------------------
void xml_int16_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, INT16  *val, int cnt, INT16 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
		int i, len;
		int strsize = 10*cnt;
		char *str = (char*)malloc(strsize);
		memset(str, 0, strsize);
		for (i=0, len=0; i<cnt; i++) len+=snprintf(&str[len], strsize-len, "%d ", val[i]);
		setup->actChapter->SetAttribute(name, str);
	}
	else
	{
		int i, pos;
		for (i=0; i<cnt; i++) val[i]=def;
		const char *str = setup->actChapter->Attribute(name);
		if (str==NULL) return;
		for (i=0, pos=0; i<cnt; i++)
		{
			if (!str[pos]) break;
			val[i] = atoi(&str[pos]);
			while (str[pos] && str[pos]!=' ') pos++;
			pos++;
		}
	}
}

//--- xml_int32 ---------------------------------------------
void xml_int32(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32 *val, INT32 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, *val);
	else
	{
		int i = def;
		setup->actChapter->Attribute(name, &i);
		*val = i;
	}
}

//--- xml_int32_arr ------------------------------------------------------------------
void xml_int32_arr(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32  *val, int cnt, INT32 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
		int i, len;
		int strsize = 10*cnt;
		char *str = (char*)malloc(strsize);
		memset(str, 0, strsize);
		for (i=0, len=0; i<cnt; i++) len+=snprintf(&str[len], strsize-len, "%d ", val[i]);
		setup->actChapter->SetAttribute(name, str);
	}
	else
	{
		int i, pos;
		for (i=0; i<cnt; i++) val[i]=def;
		const char *str = setup->actChapter->Attribute(name);
		if (str==NULL) return;
		for (i=0, pos=0; i<cnt; i++)
		{
			if (!str[pos]) break;
			val[i] = atoi(&str[pos]);
			while (str[pos] && str[pos]!=' ') pos++;
			pos++;
		}
	}
}

//--- xml_uint32 ---------------------------------------------
void xml_uint32(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT32 *val, UINT32 def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetAttribute(name, *val);
	else
	{
		int i = def;
		setup->actChapter->Attribute(name, &i);
		*val = i;
	}
}


//--- xml_double -----------------------------------------------------------------------------
void xml_double	(HANDLE hsetup, const char *name, EN_Xml_Action action, double *val, double def)
{
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) setup->actChapter->SetDoubleAttribute(name, *val);
	else
	{
		double i = def;
		setup->actChapter->Attribute(name, &i);
		*val = i;
	}
}

//--- xml_enum -----------------------------------------------
void xml_enum(HANDLE hsetup, const char *name, EN_Xml_Action action, INT32  *val, char *enumstr)
{
	// enumstr: each string ended by '|'
	SSetupFile *setup=(SSetupFile*)hsetup;
	if(action == WRITE)
	{
		int i;
		char *ch, *pstr;
		char str[32];
		for(ch=enumstr,i=0,pstr=str; *ch; ch++)
		{
			if(*ch == '|')
			{
				if(i == *val)
				{
					*pstr = 0;
					setup->actChapter->SetAttribute(name,str);
					return;		
				}
				pstr = str;
				i++;
			}
			else *pstr++=*ch;
		}
		setup->actChapter->SetAttribute(name, "???");
		return;
	}
	else
	{
		*val=0;
		const char *value = setup->actChapter->Attribute(name);
		int i;
		char *ch, *pstr;
		char str[32];
		for(ch=enumstr,i=0,pstr=str; *ch; ch++)
		{
			if(*ch == '|')
			{
				*pstr = 0;
				if (!stricmp(value, str)) 
				{						
					*val=i;
					return;		
				}
				pstr = str;
				i++;
			}
			else *pstr++=*ch;
		}
	}
}

//--- xml_int64 ---------------------------------------------
void xml_int64(HANDLE hsetup, const char *name, EN_Xml_Action action, INT64 *val, INT64 def)
{
	char str[32];
	int l;
	INT64 v;

	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
		v=abs(*val);
		l = sizeof(str);
		str[--l]=0;
		while (v)
		{
			str[--l] = '0'+v%10;
			v=v/10;
		}
		if (*val<0) str[--l] = '-';
		setup->actChapter->SetAttribute(name, &str[l]);
	}
	else
	{
		const char *s = setup->actChapter->Attribute(name);
		if (s) strncpy(str, s, sizeof(str));
		v=0;
		l=0;
		if (str[0]=='-') l++;
		for (; str[l]; l++) v = 10*v + str[l]-'0';
		if (str[0] == '-') *val = -v;
		else               *val =  v;
	}
}

//--- xml_uint64 ---------------------------------------------
void xml_uint64(HANDLE hsetup, const char *name, EN_Xml_Action action, UINT64 *val, UINT64 def)
{
	char str[32];
	int l;
	UINT64 v;

	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE)
	{
		v = *val;
		l = sizeof(str);
		str[--l] = 0;
		while (v)
		{
			str[--l] = '0' + v % 10;
			v = v / 10;
		}
		setup->actChapter->SetAttribute(name, &str[l]);
	}
	else
	{
		const char *s = setup->actChapter->Attribute(name);
		if (s) strncpy(str, s, sizeof(str));
		v = 0;
		for (l = 0; str[l]; l++) v = 10*v + str[l]-'0';
		*val = v;
	}
}

//--- xml_filetime ------------------------------------------------------------
void xml_filetime(HANDLE hsetup, const char *name, EN_Xml_Action action, FILETIME *val)
{
	UINT64 val64;
	if (action == WRITE) 
	{
		memcpy(&val64, val, sizeof(val64));
		xml_uint64(hsetup, name, action, &val64, 0);
	}
	else
	{
		val64=0;
		xml_uint64(hsetup, name, action, &val64, 0);
		memcpy(val, &val64, sizeof(val64));
	}
}

//--- xml_mac_addr ------------------------------------------------------
void xml_mac_addr	(HANDLE hsetup, const char *name, EN_Xml_Action action, INT64  *val, INT64  def)
{
	char str[STR_64];
	UCHAR  *addr = (UCHAR*)val;
	
	SSetupFile *setup=(SSetupFile*)hsetup;
	if (action == WRITE) 
	{
		sprintf(str, "%02X-%02X-%02X-%02X-%02X-%02X", addr[0], addr[1], addr[2], addr[3], addr[4], addr[5]);
		setup->actChapter->SetAttribute(name, str);
	}
	else
	{
		const char *str = setup->actChapter->Attribute(name);
		*val=0;
		sscanf(str, "%02X-%02X-%02X-%02X-%02X-%02X", (UINT*)&addr[0], (UINT*)&addr[1], (UINT*)&addr[2], (UINT*)&addr[3], (UINT*)&addr[4], (UINT*)&addr[5]);
	}
}

//--- xml_binary ----------------------------------------------------
void xml_binary	(HANDLE hsetup, const char *name, EN_Xml_Action action, void *val, int size, int *plen)
{
	char *str = (char*)malloc(2*size+2);
	char *pch;
	int i, l;
	UCHAR *pdata = (UCHAR*)val;
	
	if (action==WRITE)
	{
		for (i=0, l=0; i<size; i++)
			l+=sprintf(&str[l], "%02X", *pdata++);
		xml_str(hsetup, "Data", action, str, sizeof(str), "");		
	}
	else
	{
		memset(val, 0, size);
		xml_str(hsetup, "Data", action, str, 2*size+2, "");
		for (pch=str, i=0; *pch && i<2*size; pch++, i++)
		{
			if (*pch>='0' && *pch<='9') *pdata += *pch-'0';
			if (*pch>='A' && *pch<='F') *pdata += *pch+10-'A';
			if (*pch>='a' && *pch<='f') *pdata += *pch+10-'a';
			if (i&1) pdata++;
			else	 (*pdata)*=0x10;
		}
		*plen = i/2;			
	}
	free(str);
}

