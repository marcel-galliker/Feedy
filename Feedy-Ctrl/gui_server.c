//--- includes --------------------------------------------------
#ifdef linux
	#include <stdlib.h>
	#include <stdio.h>
	#include <unistd.h>
	#include <netdb.h>
	#include <errno.h>
	#include <sys/ioctl.h>
	#include <sys/select.h>
	#include <sys/socket.h>
	#include <sys/time.h>
	#include <sys/types.h>
	#include <net/if.h>
	#include <netinet/in.h>
	#include <netinet/in_systm.h>
	#include <netinet/ether.h>
	#include <netinet/tcp.h>
	#include <netinet/udp.h>
	#include <linux/ip.h>
	#include <linux/if_packet.h>
	#include <arpa/inet.h>
	#include <string.h>
	#include <stdio.h>
#endif

#include <stdio.h>
#include "ge_sockets.h"
#include "FdTcpIp.h"
#include "FdGlobals.h"
#include "ge_trace.h"
#include "ge_threads.h"
#include "error.h"
#include "gui_msg.h"
#include "gui_server.h"


//--- defines ---------------------------------------------------
#define MAX_GUI_CLIENTS		5
#define MAX_BUFFER_LEN		1000
#define MAX_NAME_LEN		256


//--- global variables -------------------------------
static	SOCKET		_GuiServerSok;	// main socket 							
static	SOCKET		_GuiClientSok[MAX_GUI_CLIENTS];			// channel per client					
static  HANDLE		_GuiClientThreadHandle[MAX_GUI_CLIENTS];

//--- prototypes ---------------------------------------------

//static void _GuiClient_thread (SOCKET *socket);
static void *_GuiClient_thread (void *arg);
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
	ge_sok_init_server(NULL, GUI_PORT, &_GuiServerSok);
//	ge_sok_set_blocking (&_GuiServerSok, FALSE);
	listen (_GuiServerSok, MAX_GUI_CLIENTS);
	TrPrintf( 0, "GUI: Server Socket %d (%s:%d)  Waiting for clients", _GuiServerSok, GUI_HOST, GUI_PORT);
	while (TRUE) {
		ge_thread_sleep(1000);
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
					INT32 bNoDelay = TRUE;
					#ifdef linux
						TrPrintf(-1, "GUI Connected, socket=%d", _GuiClientSok[i]);
						setsockopt(_GuiClientSok[i], IPPROTO_TCP, TCP_NODELAY, &bNoDelay, sizeof(INT32));
						_GuiClientThreadHandle[i] = ge_thread_start((thread_main)&_GuiClient_thread, &_GuiClientSok[i]);
					#else
						setsockopt(_GuiClientSok[i], IPPROTO_TCP, TCP_NODELAY, (LPSTR) &bNoDelay, sizeof(INT32));
						/*--- create a receiver thread for that client ---*/
						_GuiClientThreadHandle[i] = CreateThread( 
							NULL,									/* no security attributes */
							0,										/* default stack size */
							(LPTHREAD_START_ROUTINE) &_GuiClient_thread,	/* function to call */
							&_GuiClientSok[i],						/* parameter for function */
							0,										/* 0=thread runs immediately after being called */
							NULL									/* returns thread identifier */
						);
					#endif
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
static void *_GuiClient_thread (void *arg)
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

	SOCKET *socket = (SOCKET*)arg;
	TrPrintf(-1, "_GuiClient_thread (socket=%d)", *socket);
	#ifdef linux
	#else
		SetThreadPriority (GetCurrentThread(), THREAD_PRIORITY_BELOW_NORMAL);
	#endif
	
	feedy_reset_error();

	ge_sok_set_no_delay(*socket);

	int len, recv;
	SGuiMsg msg;
	while(TRUE) 
	{
		len = sizeof(msg.hdr);

		recv=ge_sok_receive_nb (socket, &msg.hdr, len);
		if (recv!=len) 
		{
			TrPrintf(1, "ge_sok_receive_nb header: len=%d(%d)", recv, len);
			break;
		}

		if (msg.hdr.msgLen>len)
		{
			len = msg.hdr.msgLen-len;
			recv=ge_sok_receive_nb (socket, &msg.data, len);
			if (recv!=len) 
			{
				TrPrintf(1, "ge_sok_receive_nb data: len=%d(%d)", recv, len);
				break;
			}
		}
		gui_handle_msg(*socket, &msg);
	}
	TrPrintf( 0, "GUI_Client: Socket[%d]=>>%s<< closed", *socket, ge_sok_get_socket_name(*socket, str, NULL, NULL));
	gui_disconnected(*socket);
	ge_sok_close(socket);
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
		int i;
		for(i=0; i<MAX_GUI_CLIENTS; i++)
		{
			if (_GuiClientSok[i] && _GuiClientSok[i]!=INVALID_SOCKET) ge_sok_msg_send_nb(&_GuiClientSok[i], pmsg, pmsgHdr->msgLen);
		}
	}
	else ge_sok_msg_send_nb(&socket, pmsg, pmsgHdr->msgLen);
}

//--- gui_send_status --------------------------------------------
void gui_send_status(SOCKET socket)
{
	SFeedyStatusMsg msg;
	memcpy(&msg.status, &FeedyStatus, sizeof(FeedyStatus));
	msg.hdr.msgId = REP_STATUS;
	msg.hdr.msgLen = sizeof(msg);
	gui_send(socket, &msg);
}
