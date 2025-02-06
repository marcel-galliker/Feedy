
/* **************************************************************************** */
/*																				*/
/*    Real Time API																*/
/*						C O M M U N I C A T I O N								*/
/*						-------------------------								*/
/* ****************************************************************************	*/
/*																				*/
/*    Copyright 1998 Graph-Tech. All rights reserved.							*/
/*    Written by Markus Portmann.												*/
/*																				*/
/*	  rt_sok: InterProcessCommunication 										*/
/*			  communication between process in the same or different			*/
/*			  computer based on sockets. NON-BLOCKING messages.					*/
/*			  Does not work in RTX (no sockets there yet)						*/
/*																				*/
/*			  The protocol is GT-specific: 										*/
/*					4 bytes: length of message in bytes							*/
/*					rest: message itself										*/
/*																				*/
/*			  ge_sok_init														*/
/*			  ge_sok_close														*/
/*			  ge_sok_send														*/
/*			  ge_sok_receive													*/
/*																				*/
/* ****************************************************************************	*/


/* --- Version ---------------------------------------------------------------- */


/* --- Includes ---------------------------------------------------------------	*/
#ifdef linux
	#include <stdlib.h>
	#include <stdio.h>
	#include <unistd.h>
	#include <netdb.h>
	#include <errno.h>
	#include <fcntl.h>
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
#else
	#include <winsock.h>
	#include <stdio.h>	
	#include <stdlib.h>
	#include <string.h>
	#include <fcntl.h>
#endif

#include "ge_common.h"
#include "ge_time.h"
#include "ge_trace.h"
#include "ge_sockets.h"

/* --- Makros ----------------------------------------------------------------- */



/* --- Defines ---------------------------------------------------------------- */
 

/* --- Structures -------------------------------------------------------------	*/
int traciSOK=0;
int	UdpRec, UdpSent;


/* --- Prototypes ------------------------------------------------------------- */
static int _ge_sok_sockaddr(struct sockaddr_in *ipAddr, const char *addr, int port);
static int _sok_error(SOCKET* pSocket, int line);
/* ---------------------------------------------------------------------------- */



/* **************************************************************************** */
/*    G E N E R A L   F U N C T I O N S											*/
/* **************************************************************************** */


//--- sok_sockaddr --------------------------------------------------------------------------------
static int _ge_sok_sockaddr(struct sockaddr_in *ipAddr, const char *addr, int port)
{

	ipAddr->sin_family = AF_INET;
	ipAddr->sin_port = htons((short)port);
	if (addr)
	{
		//--- Resolve host name ---
#ifdef linux
		if (inet_pton(ipAddr->sin_family, addr, &ipAddr->sin_addr)) return REPLY_OK;
#else
		int a[4];
		if (sscanf(addr, "%d.%d.%d.%d", &a[0], &a[1], &a[2], &a[3])==4) 
		{
			UCHAR *sin_addr=(UCHAR*)&ipAddr->sin_addr;
			sin_addr[0] = a[0];
			sin_addr[1] = a[1];
			sin_addr[2] = a[2];
			sin_addr[3] = a[3];
			return 0;
		}
#endif

		struct hostent *hostInfo;
		hostInfo = gethostbyname(addr);				// (gethostbyname allocates one hostent structure per thread)
		if (hostInfo == NULL) return ge_sok_error(NULL, __FILE__, __LINE__);
		memcpy((char *)&(ipAddr->sin_addr), (char *)hostInfo->h_addr_list[0], hostInfo->h_length);
	}
	else ipAddr->sin_addr.s_addr = INADDR_ANY;

	return 0;
}

/*******************************************************************************
ge_sok_init: initializes sockets, either for a client or a server 

	client		TRUE: it is a client that tries to connect to an existing end-point
				FALSE: it is a server that creates an end-point
	host		name of host. default "localhost" 
	port		number of port. default "10051" 

  return:		0=OK
	
	socki		pointer to socket handle

*******************************************************************************/
int	ge_sok_init_server(char *addr, int port, SOCKET *socki)
{
	SOCKET sok;
	int ret=0;
	struct	sockaddr_in ipAddr;
	INT32 init=TRUE;

	*socki = INVALID_SOCKET;
	
#ifndef linux
	/* --- Startup Winsockets --- */
	UINT16	version;						
	WSADATA	data;

	version = MAKEWORD(2, 0);
	ret = WSAStartup(version, &data);
	if (ret) {
		version = MAKEWORD(1, 1);
		ret = WSAStartup(version, &data);
	}
	if (ret)
		return (6);
#endif

//printf("client=%d   host [%s]  port [%d]\n", client, host, port);

	/* --- Get socket --- */
	sok = socket(AF_INET, SOCK_STREAM, 0);	
	if (sok == INVALID_SOCKET)
		return(7);
	
	_ge_sok_sockaddr(&ipAddr, addr, port);
	/* --- Socket options --- */
	
	ret = setsockopt(sok, IPPROTO_TCP, TCP_NODELAY, (char*) &init, sizeof(INT32));
	if (ret) return(8);
	ret = bind(sok, (struct sockaddr *) &ipAddr, sizeof(ipAddr));
	if (ret != 0)
		return(9);
	
	*socki = sok;
	return REPLY_OK;
}

//--- ge_sok_open_client --------------------------------------------------------------
int ge_sok_open_client(SOCKET *psocket, const char *addr, int port, int type, int timeout)
{
	//---opens TCP/IP (stream) Socket -------------

	//	struct hostent *hostInfo;
	SOCKET sok;
	struct sockaddr_in ipAddr;
	int ret;

	*psocket = INVALID_SOCKET;

	if (type != SOCK_STREAM) return 1;

	//	if (type==SOCK_STREAM && sok_ping(addr)) return REPLY_ERROR;

#ifdef WIN32
	// if (!_wsa_started)
	{
	//	_wsa_started = TRUE;

		WSADATA	data;
		WORD version;
		//--- Startup Winsockets ---
		version = MAKEWORD(2, 2);
		if (WSAStartup(version, &data))
		{
			version = MAKEWORD(1, 1);
			if (WSAStartup(version, &data)) return ge_sok_error(NULL, __FILE__, __LINE__);
		}
	}
#endif // WIN32

	//--- Get socket ---
	sok = socket(AF_INET, type, 0);
	if (sok == INVALID_SOCKET) 
		return ge_sok_error(NULL, __FILE__, __LINE__);

	_ge_sok_sockaddr(&ipAddr, addr, port);
	
	int value = TRUE;
	if (setsockopt(sok, IPPROTO_TCP, TCP_NODELAY, (char*)&value, sizeof (value))) 
		return ge_sok_error(NULL, __FILE__, __LINE__);


	//	if( (flags = fcntl(sok, F_GETFL, 0)) < 0) return ge_sok_error(NULL, __FILE__, __LINE__);
	//	fcntl(sok, F_SETFL, flags | O_NONBLOCK);
	if (timeout)
	{
		ret=connect(sok, (struct sockaddr *)&ipAddr, sizeof(ipAddr));
		ret=ge_sok_send_timeout(sok, timeout);
		ge_sok_set_blocking(&sok, TRUE);
		if (ret==0)
		{
			TrPrintf(-1, "sok_open_client(client, %s:%d) timeout %d ms\n", addr, port, timeout);
			return WSAETIMEDOUT;
		}
	}
	else if (connect(sok, (struct sockaddr *)&ipAddr, sizeof(ipAddr))) 
	{
		return ge_sok_error(NULL, __FILE__, __LINE__);
	}

	*psocket = sok;

	return 0;
}

/*******************************************************************************
ge_sok_close: close sockets, either for a client or a server 

	client		TRUE: it is a client
				FALSE: it is a server 

*******************************************************************************/
void	ge_sok_close		( SOCKET *socki)
{
	if (*socki)
	{
	#ifdef linux
		close(*socki);
	#else
		closesocket( *socki);
	#endif
	//	WSACleanup ();
		*socki = INVALID_SOCKET;
	}
}

//--- ge_sok_send_timeout ------------------------------------
// return>0: ready to send
// return=0: not ready to send
// return<0: error
int ge_sok_send_timeout(SOCKET socket, int ms)
{
	if (ms==0) return 1;

	fd_set fdSet;

	struct timeval timeout;
	// Set the Timeout. 
	timeout.tv_sec  = ms / 1000;                // Extract the seconds
	timeout.tv_usec = (ms % 1000) * 1000;       // and microseconds of the timeout

	// Clear file descriptor set.
	FD_ZERO(&fdSet);

	// Set the bit for network socket in the descriptor set.
	FD_SET(socket, &fdSet);

	// Call select. No need to use FD_ISSET because select was called for one socket descriptor.
	int ret=select((int)socket + 1, NULL, &fdSet, NULL, &timeout);
	//	if (!ret)
	//		TrPrintf(-1, "Socket Receive Timeout ms=%d\n", ms);
	return ret;
}

//--- ge_sok_recv_timeout --------------------------------
// return>0: message received
// return=0: no message received
// return<0: error
int ge_sok_recv_timeout(SOCKET socket, int ms)
{
	fd_set fdSet;

	struct timeval timeout;
	// Set the Timeout. 
	timeout.tv_sec  = ms / 1000;                // Extract the seconds
	timeout.tv_usec = (ms % 1000) * 1000;       // and microseconds of the timeout

	// Clear file descriptor set.
	FD_ZERO(&fdSet);

	// Set the bit for network socket in the descriptor set.
	FD_SET(socket, &fdSet);

	// Call select. No need to use FD_ISSET because select was called for one socket descriptor.
	return select((int)socket + 1, &fdSet, NULL, NULL, &timeout);
}

//--- sok_ping ------------------------------------------------------
/*
static int sok_ping_old(const char *addr) 
{
	// obsolete version
	if (!*addr) return REPLY_ERROR;
	char cmd[32];
	sprintf(cmd, "ping -n 1 -w 10 %s", addr);
	FILE *pipe=_popen(cmd, "r");
	if (pipe==NULL) return REPLY_ERROR;
	return _pclose(pipe);
}

//--- sok_ping -------------------------
int sok_ping(const char *ipAddr)
{
	// https://github.com/davidgatti/How-to-Deconstruct-Ping-with-C-and-NodeJS/blob/master/C/pingWithBareMinimum.c
	
	int reply=REPLY_OK;
	WSADATA	data;


	int ret= WSAStartup (MAKEWORD(2, 0), &data);
	int s = (int)socket(PF_INET, SOCK_RAW, 1);
	if (s <= 0) reply=WSAGetLastError();
	else
	{
		typedef struct {
			UINT8 type;
			UINT8 code;
			UINT16 chksum;
			UINT32 data;
		} icmp_hdr_t;

		icmp_hdr_t pckt = {
			.type   = 8,       // The echo request is 8
			.code   = 0,       // No need
			.chksum = 0xfff7,  // Fixed checksum since the data is not changing
			.data   = 0        // We don't send anything.
		};

		struct sockaddr_in addr;
		addr.sin_family = AF_INET;
		addr.sin_port = 0;
		addr.sin_addr.s_addr = inet_addr(ipAddr);
		ret = sendto(s, (const char*)&pckt, sizeof(pckt), 0, (struct sockaddr*)&addr, sizeof(addr));
		if (ret<=0) reply=WSAGetLastError();
		else
		{
			int len=sizeof(struct sockaddr_in);
			unsigned char res[30];

			ret=ge_sok_recv_timeout(s, 50);
			if (ret>0)
			{
				ret = recvfrom(s, res, sizeof(res), 0, (struct sockaddr*)&addr, &len);
					
				if (ret<=0) reply=WSAGetLastError();
			}
			else reply=WSAETIMEDOUT; 
		}
		closesocket(s);
	}

	return reply;
}
*/

//--- ge_sok_set_blocking --------------------------
int ge_sok_set_blocking(SOCKET *socki, int blocking)
{
	if (*socki < 0) return REPLY_ERROR;

#ifdef _WIN32
	unsigned long mode = blocking ? 0 : 1;
	return (ioctlsocket(*socki, FIONBIO, &mode) == 0);
#else
	int status;
	if (blocking)
		status = fcntl(*socki, F_SETFL, fcntl(*socki, F_GETFL, 0) & ~O_NONBLOCK);
	else	
		status = fcntl(*socki, F_SETFL, fcntl(*socki, F_GETFL, 0) | O_NONBLOCK);
	return 0;
#endif
}

/*******************************************************************************
ge_sok_msg_send_nb: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int ge_sok_msg_send_nb(SOCKET *socki, void *msg_send, INT32 send_size)
{
	INT32 leni, sent, ret;
	fd_set writeSock;
	char *pmsg = (char*)msg_send;
	//struct timeval timeout;

	//timeout.tv_sec=0;
	//timeout.tv_usec=50000;
	FD_ZERO(&writeSock);
	FD_SET(*socki, &writeSock);

	/* --- send message length --- */
	/*
	sent=0;
	while (1) {
		leni=send (*socki, (char *) (&send_size)+sent, sizeof (send_size)-sent, 0);
		if (leni==SOCKET_ERROR) {
			ret=WSAGetLastError();
			if (ret==WSAEWOULDBLOCK) {
				ret=select (0, NULL, &writeSock, NULL, NULL);
				if (ret==SOCKET_ERROR) {
					ret=WSAGetLastError();
					printf("%s [%d]: Send 1 Return is %d\n", __FILE__, __LINE__, ret);
					return (-1);
				}
				continue;
			}
			else {
				printf("%s [%d]: Send 2 Return is %d  leni=%d\n", __FILE__, __LINE__, ret, leni);
				return (-2);
			}
		}
		else if (!leni) {
			printf("%s [%d]: Send 3 Return is %d\n", __FILE__, __LINE__, ret);
			return (-3);
		}
		else {
			sent+=leni;
			if (sent==sizeof (send_size))
				break;
		}
	}
	*/

	/* --- send message --- */
	sent = 0;
	while (1) {
		leni = send(*socki, (char *) pmsg + sent, send_size - sent, 0);
		if (leni<0) 
		{
			#ifdef linux
				if (errno == EWOULDBLOCK) {
					ret = select(0, NULL, &writeSock, NULL, NULL);
					if (ret<0) {
						printf("%s [%d]: Send Return is -4\n", __FILE__, __LINE__);
						return (-4);
					}
					continue;
				}
				else {
					printf("%s [%d]: Send Return is -5\n", __FILE__, __LINE__);
					return (-5);
				}
			#else
				ret = WSAGetLastError();
				if (ret == WSAEWOULDBLOCK) {
					ret = select(0, NULL, &writeSock, NULL, NULL);
					if (ret == SOCKET_ERROR) {
						ret = WSAGetLastError();
						printf("%s [%d]: Send Return is -4\n", __FILE__, __LINE__);
						return (-4);
					}
					continue;
				}
				else {
					printf("%s [%d]: Send Return is -5\n", __FILE__, __LINE__);
					return (-5);
				}
			#endif
		}
		else if (!leni) {
			printf("%s [%d]: Send Return is -6\n", __FILE__, __LINE__);
			return (-6);
		}
		else {
			sent += leni;
			if (sent == send_size)
				break;
		}
	}
	return (send_size);
}

/*******************************************************************************
ge_sok_receive_nb: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		ge_sok_receive_nb	( SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	INT32 leni, rec, ret;
	char *pMsg;
	fd_set readSock;
	struct timeval timeout;

	timeout.tv_sec=0;
	timeout.tv_usec=50000;
	FD_ZERO (&readSock);
	FD_SET (*socki, &readSock);

	/* --- get message --- */
	pMsg= (char*) msg_rec;
	rec=0;
	while (1) {
		leni=recv (*socki, (char *) msg_rec+rec, max_rec_size, 0);
		if (leni<0) 
		{
			#ifdef linux
			if (errno == EWOULDBLOCK) {
				ret = select(0, &readSock, NULL, NULL, NULL);
				if (ret<0) 
				{
					//					ret=WSAGetLastError();
					//					#ifndef _AFXDLL
					//					RtPrintf ("Rcv 3 Return is %d\n", ret);
					//					#endif
					return (-3);
				}
				continue;
			}
			else {
				//				#ifndef _AFXDLL
				//				RtPrintf ("Rcv 4 Return is %d\n", ret);
				//				#endif
				return (-4);
			}
#else
				ret=WSAGetLastError();
				if (ret==WSAEWOULDBLOCK) {
					ret=select (0, &readSock, NULL, NULL, NULL);
					if (ret==SOCKET_ERROR) {
	//					ret=WSAGetLastError();
	//					#ifndef _AFXDLL
	//					RtPrintf ("Rcv 3 Return is %d\n", ret);
	//					#endif
						return (-3);
					}
					continue;
				}
				else {
	//				#ifndef _AFXDLL
	//				RtPrintf ("Rcv 4 Return is %d\n", ret);
	//				#endif
					return (-4);
				}
			#endif
		}
		else if (!leni) {
			return (0);
		}
		else {
			rec+=leni;
//			if (rec>=*lenp)
				break;
		}
	}
//*(pMsg+leni)=0;
//printf("leni=%d [%s]\n", leni, msg_rec);
	return ( leni);
}


/*******************************************************************************
sok_error:

*******************************************************************************/

int ge_sok_error(SOCKET *pSocket, char *file, int line)
{
	int errCode;
	#ifdef linux 
		errCode  = errno;
	#else
		errCode  = WSAGetLastError();
	#endif
	TrPrintf(-1, "(%s-%d) Socket error %d\n", file, line, errCode);
	return errCode;
}

/*******************************************************************************

*******************************************************************************/
int ge_sok_set_no_delay(SOCKET socket)
{
	INT32 val=1;
	return setsockopt (socket, IPPROTO_TCP, TCP_NODELAY, (char*) &val, sizeof (INT32));
}

/*******************************************************************************/

//--- ge_sok_get_socket_name ---------------------------------------------------------------
char *ge_sok_get_socket_name(SOCKET socket, char *namestr, char *ipstr, UINT32 *pport)
{
	struct sockaddr sender;
	int  len = sizeof(sender);

	memset(&sender, 0, sizeof(sender));
	getsockname(socket, &sender, &len);
	if (ipstr!=NULL) sprintf(ipstr, "%d.%d.%d.%d", (UCHAR)sender.sa_data[2], (UCHAR)sender.sa_data[3], (UCHAR)sender.sa_data[4], (UCHAR)sender.sa_data[5]);
	if (pport!=NULL) *pport = (UCHAR)sender.sa_data[0]<<8|(UCHAR)sender.sa_data[1];
	if (namestr!=NULL) sprintf(namestr, "%d.%d.%d.%d:%d", (UCHAR)sender.sa_data[2], (UCHAR)sender.sa_data[3], (UCHAR)sender.sa_data[4], (UCHAR)sender.sa_data[5], (UCHAR)sender.sa_data[0]<<8|(UCHAR)sender.sa_data[1]);
	return namestr;
}
