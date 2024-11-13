#pragma once


//--- includes --------------------------------------------------
#include <stdio.h>
#include "rt_sok_api.h"
#include "FdTcpIp.h"
#include "FdGlobals.h"
#include "ge_trace.h"
#include "error.h"
#include "gui_msg.h"
#include "gui_server.h"

//--- defines ---------------------------------------------------
#define MAX_GUI_CLIENTS		5
#define MAX_BUFFER_LEN		1000
#define MAX_NAME_LEN		256


//--- global variables -------------------------------
static const int	NonBlk=TRUE;
static	SOCKET		_GuiServerSok;	// main socket 							
static	SOCKET		_GuiClientSok[MAX_GUI_CLIENTS];			// channel per client					
static  HANDLE		_GuiClientThreadHandle[MAX_GUI_CLIENTS];

//--- prototypes ---------------------------------------------

static void _GuiClient_thread (SOCKET *socket);

//--- gui_server -------------------------------
void gui_server(void)
{
	int	i;

	for (i=0; i<MAX_GUI_CLIENTS; i++)
	{
		_GuiClientSok[i]=INVALID_SOCKET;
		_GuiClientThreadHandle[i]=NULL;
	}

	// --- take the socket and listen ---
//	rt_sok_init (FALSE, GUI_HOST, GUI_PORT, &_GuiServerSok);
	rt_sok_init (FALSE, NULL, GUI_PORT, &_GuiServerSok);
	if ( NonBlk)
		rt_sok_set_nb (&_GuiServerSok);
	listen (_GuiServerSok, MAX_GUI_CLIENTS);
	TrPrintf( 0, "GUI: Server Socket %d (%s:%d)  Waiting for clients", _GuiServerSok, GUI_HOST, GUI_PORT);
	while (TRUE) {
		Sleep(1000);
		/*--- Get the next available connection ---*/
		for (i=0; i<MAX_GUI_CLIENTS; i++)
		{
			if (_GuiClientSok[i]==INVALID_SOCKET) 
			{
				/*--- wait for client connection ---*/
				_GuiClientSok[i]=accept (_GuiServerSok, NULL, NULL);
				if (_GuiClientSok[i]==INVALID_SOCKET) {
					/*
					if (ret=WSAGetLastError() ==10093) { 
						// socket has already been closed
						printf( "socket has already been closed");
						return;
					}
					*/
				}
				else {
					BOOL bNoDelay = TRUE;
					setsockopt (_GuiClientSok[i], IPPROTO_TCP, TCP_NODELAY, (LPSTR) &bNoDelay, sizeof (BOOL));
					/*--- create a receiver thread for that client ---*/
					_GuiClientThreadHandle[i]=CreateThread ( 
						NULL,									/* no security attributes */
						0,										/* default stack size */
						(LPTHREAD_START_ROUTINE) &_GuiClient_thread,	/* function to call */
						&_GuiClientSok[i],						/* parameter for function */
						0,										/* 0=thread runs immediately after being called */
						NULL									/* returns thread identifier */
					);
					if (_GuiClientThreadHandle[i]==NULL)
						printf("Could not start Thread");
				}
				break;
			}
		}
	}
	return;
}

//--- _GuiClient_thread ----------------------------
static void _GuiClient_thread (SOCKET *socket)
{
	char str[32];
//	TrPrintf( 0, "GUI_Client: Socket[%d]=>>%s<< connected", *socket, sok_get_socket_name(*socket, str, NULL, NULL));
//	Error(LOG, 10, "GUI_Client: Socket[%d]=>>%s<< connected", *socket, sok_get_socket_name(*socket, str, NULL, NULL));
	/*
	if (EzLicense.headType<Norwix_PC22)
	{
		Error(LOG, 20, "Hyper: Socket=%d", hyper_get_socket());
	}
	*/

	SetThreadPriority (GetCurrentThread(), THREAD_PRIORITY_BELOW_NORMAL);
	
	feedy_reset_error();

	rt_sok_set_no_delay(*socket);

	int len, recv;
	SGuiMsg msg;
	while(TRUE) 
	{
		len = sizeof(msg.hdr);

		recv=rt_sok_receive_nb (socket, &msg.hdr, len);
		if (recv!=len) 
		{
			TrPrintf(1, "rt_sok_receive_nb header: len=%d(%d)", recv, len);
			break;
		}

		if (msg.hdr.msgLen>len)
		{
			len = msg.hdr.msgLen-len;
			recv=rt_sok_receive_nb (socket, &msg.data, len);
			if (recv!=len) 
			{
				TrPrintf(1, "rt_sok_receive_nb data: len=%d(%d)", recv, len);
				break;
			}
		}
		gui_handle_msg(*socket, &msg);
	}
	TrPrintf( 0, "GUI_Client: Socket[%d]=>>%s<< closed", *socket, sok_get_socket_name(*socket, str, NULL, NULL));
	gui_disconnected(*socket);
	closesocket (*socket);
	*socket=INVALID_SOCKET;
} /* end main_thread */

//--- gui_send -----------------------------------------
void gui_send(SOCKET socket, void *pmsg)
{
	SMsgHdr *pmsgHdr = (SMsgHdr*)pmsg;

	if (pmsgHdr->msgLen==0) return;
	if (FALSE)
	{
		int i;
		char *ch;
		TrPrintf(-1, "Send: ");
		for ( i=0, ch=(char*)pmsg; i<pmsgHdr->msgLen; i++, ch++)
		{
			if (FALSE && *ch >= 0x20) TrPrintf( 0, "%c", *ch);
			else			 TrPrintf( 0, "<%02X>", *ch);
		}
		TrPrintf( 0, "\n");
	}
	if (socket==0 || socket==INVALID_SOCKET)
	{
		for(int i=0; i<MAX_GUI_CLIENTS; i++)
		{
			if (_GuiClientSok[i] && _GuiClientSok[i]!=INVALID_SOCKET) rt_sok_msg_send_nb(&_GuiClientSok[i], pmsg, pmsgHdr->msgLen);
		}
	}
	else rt_sok_msg_send_nb(&socket, pmsg, pmsgHdr->msgLen);
}

//--- gui_send_status --------------------------------------------
void gui_send_status(SOCKET socket)
{
	int time=ge_ticks();
	SFeedyStatusMsg msg;
	memcpy(&msg.status, &FeedyStatus, sizeof(FeedyStatus));
	msg.hdr.msgId = REP_STATUS;
	msg.hdr.msgLen = sizeof(msg);
	gui_send(socket, &msg);
}
