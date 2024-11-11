
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
/*			  rt_sok_init														*/
/*			  rt_sok_close														*/
/*			  rt_sok_send														*/
/*			  rt_sok_receive													*/
/*																				*/
/* ****************************************************************************	*/


/* --- Version ---------------------------------------------------------------- */


/* --- Includes ---------------------------------------------------------------	*/
#include <winsock.h>
#include <stdio.h>	
#include <stdlib.h>
#include <string.h>
#include <fcntl.h>
#include "ge_time.h"
#include "ge_trace.h"

#include "rt_sok_api.h"

/* --- Makros ----------------------------------------------------------------- */



/* --- Defines ---------------------------------------------------------------- */
 

/* --- Structures -------------------------------------------------------------	*/
int traciSOK=0;
int	UdpRec, UdpSent;


/* --- Prototypes ------------------------------------------------------------- */

/* ---------------------------------------------------------------------------- */



/* **************************************************************************** */
/*    G E N E R A L   F U N C T I O N S											*/
/* **************************************************************************** */


/*******************************************************************************
rt_sok_init: initializes sockets, either for a client or a server 

	client		TRUE: it is a client that tries to connect to an existing end-point
				FALSE: it is a server that creates an end-point
	host		name of host. default "localhost" 
	port		number of port. default "10051" 

  return:		0=OK
	
	socki		pointer to socket handle

*******************************************************************************/
int	rt_sok_init			( int client, char *host, int port, SOCKET *socki)
{
	int ret=0;
	unsigned int addr;
	struct	sockaddr_in baseAddr;
	struct	hostent *hostInfo;
	int init=TRUE;
	WORD	version;						
	WSADATA	data;


//printf("client=%d   host [%s]  port [%d]\n", client, host, port);
	if ( client){
		/* --- Startup Winsockets --- */
		if ( traciSOK) printf("Startup Winsockets\n");
		version= MAKEWORD (2,0);
		ret= WSAStartup (version, &data);
		if (ret) {
			printf("<<<<<<<<<<<<<<<<< version 1\n");
			version= MAKEWORD (1,1);
			ret= WSAStartup (version, &data);
		}
		if ( ret)
			return( 1);

		/* --- Get socket --- */
		if ( traciSOK) printf("Get socket CLIENT\n");
		*socki = socket( AF_INET, SOCK_STREAM, 0);
		if ( *socki == INVALID_SOCKET){
			return(2);
		}
		/* --- Socket options --- */
		ret=setsockopt (*socki, IPPROTO_TCP, TCP_NODELAY, (char *) &init, sizeof (BOOL));
		if ( ret)
			return(3);

		if (isalpha(host[0])){
			/* --- Resolve host name --- */  
			hostInfo = gethostbyname ((char*) host);
			if ( hostInfo==NULL)
				return(41);
		}
		else{
			/* Convert nnn.nnn address to a usable one */
			addr = inet_addr( host);
			hostInfo = gethostbyaddr((char *)&addr,4,AF_INET);
			if ( hostInfo==NULL)
				return(42);
		}
		memcpy((char *) &(baseAddr.sin_addr), (char *) hostInfo->h_addr, hostInfo->h_length);

		/* --- Get port Number --- */
		if ( traciSOK) printf("get port for >>>%s<<< port=%d\n", host, port);
	    baseAddr.sin_family = AF_INET;       
		baseAddr.sin_port = htons( (short) port);

		/* --- Try to connect to socket --- */
		if ( traciSOK) printf("Try to connect socket\n");
		if ( connect( *socki, (struct sockaddr *) &baseAddr, sizeof (baseAddr) ) != 0) {
//			ret = WSAGetLastError();
//			RtPrintf ("Error is >>>>%d<<<<\n", ret);
			return(5);
		}
	}
	else{
		/* --- Startup Winsockets --- */
		if ( traciSOK) printf("Startup Winsockets\n");
		version= MAKEWORD (2,0);
		ret= WSAStartup (version, &data);
		if (ret) {
			version= MAKEWORD (1,1);
			ret= WSAStartup (version, &data);
		}
		if ( ret)
			return(6);

		/* --- Get socket --- */
		if ( traciSOK) printf("Get socket SERVER\n");
		*socki = socket( AF_INET, SOCK_STREAM, 0);	
		if ( *socki == INVALID_SOCKET)
			return(7);

		/* --- Get port Number --- */
		if ( traciSOK) printf("get port for >>>%s<<< port=%d\n", host, port);
	    baseAddr.sin_addr.s_addr = INADDR_ANY;
	    baseAddr.sin_family = AF_INET;       
		baseAddr.sin_port = htons( (short) port);

		if (host!=NULL && host[0])
		{
			if (isalpha(host[0])){
				/* --- Resolve host name --- */  
				hostInfo = gethostbyname ((char*) host);
				if ( hostInfo==NULL)
					return(41);
			}
			else{
				/* Convert nnn.nnn address to a usable one */
				addr = inet_addr( host);
				hostInfo = gethostbyaddr((char *)&addr,4,AF_INET);
				if ( hostInfo==NULL)
					return(42);
			}
			memcpy((char *) &(baseAddr.sin_addr), (char *) hostInfo->h_addr, hostInfo->h_length);
		}

		/* --- Socket options --- */
		ret=setsockopt (*socki, IPPROTO_TCP, TCP_NODELAY, (char*) &init, sizeof (BOOL));
		if (ret)
			return(8);

		/* --- Try to bind socket --- */
		if ( traciSOK) printf("Try to bind socket\n");
		if ( bind( *socki, (struct sockaddr *) &baseAddr, sizeof (baseAddr) ) != 0)
			return(9);
	}
	return (0);
}

//--- rt_sok_send_timeout ------------------------------------
// return>0: ready to send
// return=0: not ready to send
// return<0: error
int rt_sok_send_timeout(SOCKET socket, int ms)
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

//--- rt_sok_recv_timeout --------------------------------
// return>0: message received
// return=0: no message received
// return<0: error
int rt_sok_recv_timeout(SOCKET socket, int ms)
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

/*******************************************************************************
rt_sok_close: close sockets, either for a client or a server 

	client		TRUE: it is a client
				FALSE: it is a server 

*******************************************************************************/
void	rt_sok_close		( SOCKET *socki)
{
	if (*socki)
	{
		closesocket( *socki);
	//	WSACleanup ();
		*socki = INVALID_SOCKET;
	}
}

//--- sok_ping ------------------------------------------------------
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
	int s = socket(PF_INET, SOCK_RAW, 1);
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

			ret=rt_sok_recv_timeout(s, 50);
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

//--- sok_sockaddr --------------------------------------------------------------------------------
int sok_sockaddr(struct sockaddr_in *ipAddr, const char *addr, int port)
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
		if (hostInfo == NULL) return sok_error(NULL, __LINE__);
		memcpy((char *)&(ipAddr->sin_addr), (char *)hostInfo->h_addr_list[0], hostInfo->h_length);
	}
	else ipAddr->sin_addr.s_addr = INADDR_ANY;

	return 0;
}

//--- sok_open_client --------------------------------------------------------------
int sok_open_client(SOCKET *psocket, const char *addr, int port, int type, int timeout)
{
	//---opens TCP/IP (stream) Socket -------------

	//	struct hostent *hostInfo;
	struct sockaddr_in ipAddr;
	int ret;
	SOCKET sok;
	int time0, time;

	*psocket = INVALID_SOCKET;

	if (type != SOCK_STREAM && type != SOCK_DGRAM) return 1;

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
			if (WSAStartup(version, &data)) return sok_error(NULL, __LINE__);
		}
	}
#endif // WIN32

	//--- Get socket ---
	sok = socket(AF_INET, type, 0);
	if (sok == INVALID_SOCKET) 
		return sok_error(NULL, __LINE__);

	time0 = ge_ticks();
	sok_sockaddr(&ipAddr, addr, port);
	time = ge_ticks()-time0;

	if (type == SOCK_STREAM)
	{
		int value = TRUE;
		if (setsockopt(sok, IPPROTO_TCP, TCP_NODELAY, (char*)&value, sizeof (value))) 
			return sok_error(NULL, __LINE__);
	}
	//	if( (flags = fcntl(sok, F_GETFL, 0)) < 0) return sok_error(&sok, __LINE__);
	//	fcntl(sok, F_SETFL, flags | O_NONBLOCK);
	if (timeout)
	{
		ULONG ul=1;
		ioctlsocket (sok, FIONBIO, &ul);
		ret=connect(sok, (struct sockaddr *)&ipAddr, sizeof(ipAddr));
//		if (ret) return sok_error(NULL, __LINE__);
		ret=rt_sok_send_timeout(sok, timeout);
		ul=0;
		ioctlsocket (sok, FIONBIO, &ul);
		if (ret==0)
		{
			TrPrintf(-1, "sok_open_client(client, %s:%d) timeout %d ms\n", addr, port, timeout);
			return WSAETIMEDOUT;
		}
	}
	else if (connect(sok, (struct sockaddr *)&ipAddr, sizeof(ipAddr))) 
	{
		return sok_error(NULL, __LINE__);
	}

	*psocket = sok;

	return 0;
}

/*******************************************************************************
rt_sok_init_udp: initializes sockets, a receiver and a sender. 
				 Make sure the port number coincides 

	client		TRUE: it is a client that tries to connect to an existing end-point
				FALSE: it is a server that creates an end-point
	type		SOCK_STREAM or SOCK_DGRAM 
	host		name of host. default "localhost" 
	port		number of port. default "10071" 

  return:		0=OK
	
	socki		pointer to socket handle

*******************************************************************************/
int	rt_sok_init_udp ( char *host, int port, SOCKET *socki)
{
	
	struct sockaddr_in server;
    WSADATA wsa;
	
	//Initialize winsock
	if ( traciSOK) printf("%s [%d]: UDPPPPP Startup Winsockets ip[%s] port=%d\n", __FILE__, __LINE__, host, port);
	if (WSAStartup(MAKEWORD(2,2),&wsa) != 0)
	{
		printf("%s [%d]: Failed. Error Code : %d", __FILE__, __LINE__, WSAGetLastError());
		return 1;
	}
	if ( traciSOK) printf("%s [%d]: Initialized\n", __FILE__, __LINE__);

	UdpRec=0;
	//Create a socket
	*socki = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if(*socki == INVALID_SOCKET){
		printf("%s [%d]: Could not create socket : %d", __FILE__, __LINE__, WSAGetLastError());
		return 2;
	}
	if ( traciSOK) printf("%s [%d]: Receiver Socket created port=%d\n", __FILE__, __LINE__, port);
 
	//Prepare the sockaddr_in structure
	server.sin_family = AF_INET;
	if (host==NULL)
		server.sin_addr.S_un.S_addr = INADDR_ANY;    
	else
		server.sin_addr.S_un.S_addr = inet_addr( host);    
	server.sin_port = htons( (short) port );

	TrPrintf(1, "%s [%d]: Setting server udp IP %s:%d\n", __FILE__, __LINE__, inet_ntoa(server.sin_addr), ntohs(server.sin_port));
	
	//Bind
	if( bind( *socki ,(struct sockaddr *)&server , sizeof(server)) == SOCKET_ERROR){
		int err=WSAGetLastError();
		printf("%s [%d]: Bind failed with error code : %d" , __FILE__, __LINE__, err);
		return 3;
	}
	if ( traciSOK) printf("%s [%d]: Socket bind OK\n", __FILE__, __LINE__);

	return 0;
}

/*******************************************************************************
rt_sok_msg_receive_udp: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		rt_sok_msg_receive_udp	( SOCKET *socki, void *msg_rec, INT32 max_rec_size, struct sockaddr_in *si_other)
{
	int rec;
	int ret;
	INT32 leni;
	char buffi[32];
	INT32 *lenp;
	int slen = sizeof(struct sockaddr_in);
	
	/* --- get message length and number --- */
	rec = 0;
	if ( traciSOK) 
		printf("%s [%d]: start maxsize = %d  %ld\n", __FILE__, __LINE__, max_rec_size, sizeof(int) - rec);
	
	while (1) {
		int slen = sizeof(struct sockaddr_in);
		leni = recvfrom(*socki, buffi, 4, 0, (struct sockaddr *)si_other, (socklen_t *)&slen);
		ret = WSAGetLastError();

		if ( traciSOK) 
		{
			printf("%s [%d]: leni = %d from %s:%d\n", __FILE__, __LINE__, leni, inet_ntoa(si_other->sin_addr), ntohs(si_other->sin_port));
			ShowData(&buffi[rec], sizeof(int) - rec);
		}
		
		if (leni == SOCKET_ERROR) {

			printf("%s [%d]: Rcv 1 Return is %d\n", __FILE__, __LINE__, ret);
			return (-1);
		}
		else if (!leni) {
			return (0);
		}
		else {
			rec += leni;

			if (rec == sizeof(int))
				break;
		}
	}
	
	lenp = (int *)buffi;

	if ( traciSOK) 
		printf("%s [%d]: length = %d < %d  \n", __FILE__, __LINE__, *lenp, max_rec_size);
	
	if (*lenp > max_rec_size){
		printf("%s [%d]: Received length %d > %d MaxRecSize\n", __FILE__, __LINE__, *lenp, max_rec_size);
		return( -2);
	}

	/* --- get message --- */
	rec=0;
	while (1) {
		int slen = sizeof(struct sockaddr_in);
		leni = recvfrom( *socki, (char *) msg_rec+rec, *lenp - rec, 0, (struct sockaddr *) si_other, (socklen_t *)&slen);
		//if ( traciSOK) printf("Received packet from %s:%d\n", inet_ntoa(si_other.sin_addr), ntohs(si_other.sin_port));
		if ( traciSOK) 
		{
			printf("%s [%d]: Received packet from %s:%d\n", __FILE__, __LINE__, inet_ntoa(si_other->sin_addr), ntohs(si_other->sin_port));
			ShowData((char *)msg_rec + rec, *lenp - rec);
		}
		
		if (leni==SOCKET_ERROR) {
			printf("%s [%d]: Rcv 3 Return is %d\n", __FILE__, __LINE__, WSAGetLastError());
			return( -3);
		}
		else if (!leni) {
			return (0);
		}
		else {
			rec+=leni;
			if (rec >= *lenp)
				break;
		}
	}
	if (rec != *lenp)
		return (-5);
	UdpRec++;
	return (*lenp);
}

void ShowData(char *data, INT32 len)
{
	int i, j;

	for (i = 0; i < len; i++)
	{
		printf("0x%02hhX ", data[i]);
		if (((i + 1) % 16) == 0)
		{
			printf("   ");
			for ( j = i - 15; j <= i; j++)
			{
				if (data[j] > 0x1f && data[j] < 0x7f)
					printf("%c", data[j]);
				else
					printf(".");
			}
			printf("\n");
		}
	}

	if (i % 16)
	{
		int Spaces = ((5 * 16) - ((i % 16) * 5)) + 3;
		for ( j = 0; j < Spaces; j++)
			printf(" ");

		for ( j = i - (i % 16); j < i; j++)
		{
			if (data[j] > 0x1f && data[j] < 0x7f)
				printf("%c", data[j]);
			else
				printf(".");
		}
	}
	
	printf("\n");
}

/*******************************************************************************
rt_sok_msg_send_udp: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send
  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int rt_sok_msg_send_udp(SOCKET *socki, void *msg_send, INT32 send_size, struct sockaddr_in *send_udp)
{
	INT32 leni, sent, ret;
	INT32 slen = sizeof(struct sockaddr);

//	send_udp->sin_port = si_udp.sin_port;
//	printf("%s [%d]: Sending leni %d to %s:%d\n", __FILE__, __LINE__, send_size, inet_ntoa(send_udp->sin_addr), ntohs(send_udp->sin_port));

	/* --- send message length & number --- */
	sent=0;
	while (1) {
//2		memcpy( &buffi[0], (char *) &send_size, sizeof (send_size));
//2		memcpy( &buffi[4], (char *) &UdpSent, sizeof (UdpSent));
//2		leni=sendto( *socki, buffi+sent, 2*sizeof (send_size)-sent , 0 , (struct sockaddr *) &si_udp, slen);

//		printf("%s [%d]: Sending len %ld\n", __FILE__, __LINE__, sizeof(send_size));

	//	ShowData((char *)(&send_size) + sent, sizeof(send_size) - sent);
		
		leni = sendto( *socki, (char *) (&send_size) + sent, sizeof(send_size) - sent , 0 , (struct sockaddr *)send_udp, slen);
		if (leni==SOCKET_ERROR) {
			ret=WSAGetLastError();
			printf("%s [%d]: Send -1 Error is %d\n", __FILE__, __LINE__, ret);
			return (-1);
		}
		else if (!leni) {
			printf("%s [%d]: Send 3 Return is %d\n", __FILE__, __LINE__, leni);
			return (-2);
		}
		else {
			sent += leni;
//			if (sent == 2 * sizeof(send_size))
			if (sent == sizeof(send_size))
				break;
		}
	}
	/* --- send message --- */
	sent=0;
	while (1) {
	//	printf("%s [%d]: Sending len %d\n", __FILE__, __LINE__, send_size - sent);
	//	ShowData((char *)msg_send + sent, send_size - sent);

		leni = sendto( *socki, (char *) msg_send + sent, send_size - sent, 0 , (struct sockaddr *)send_udp, slen);
		
		if (leni==SOCKET_ERROR) {
			ret = WSAGetLastError();
			printf("%s [%d]: Send 4 Error is %d\n", __FILE__, __LINE__, ret);
			return (-4);
		}
		else if (!leni) {
			printf("%s [%d]: Send 6 Return is %d\n", __FILE__, __LINE__, leni);
			return (-5);
		}
		else {
			sent += leni;
			if (sent == send_size)
				break;
		}
	}
	UdpSent++;
	return (send_size);
}


/*******************************************************************************
rt_sok_set_nb: Sets the socket to none blocking mode

	socki		pointer to socket where msg_send is to be sent
	return:		0=OK, else error

*******************************************************************************/
int	rt_sok_set_nb (SOCKET *socki)
{
	unsigned long l=1;

	if (ioctlsocket (*socki, FIONBIO, &l))
		return (1);
	return (0);
}

/*******************************************************************************
rt_sok_msg_send_nb: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int rt_sok_msg_send_nb (SOCKET *socki, void *msg_send, INT32 send_size)
{
	INT32 leni, sent, ret;
	fd_set writeSock;
	char *pmsg = (char*)msg_send;
	//struct timeval timeout;

	//timeout.tv_sec=0;
	//timeout.tv_usec=50000;
	FD_ZERO (&writeSock);
	FD_SET (*socki, &writeSock);

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
	sent=0;
	while (1) {
		leni=send (*socki, (char *) pmsg+sent, send_size-sent, 0);
		if (leni==SOCKET_ERROR) {
			ret=WSAGetLastError();
			if (ret==WSAEWOULDBLOCK) {
				ret=select (0, NULL, &writeSock, NULL, NULL);
				if (ret==SOCKET_ERROR) {
					ret=WSAGetLastError();
					printf("%s [%d]: Send Return is -4\n", __FILE__, __LINE__);
					return (-4);
				}
				continue;
			}
			else {
				printf("%s [%d]: Send Return is -5\n", __FILE__, __LINE__);
				return (-5);
			}
		}
		else if (!leni) {
			printf("%s [%d]: Send Return is -6\n", __FILE__, __LINE__);
			return (-6);
		}
		else {
			sent+=leni;
			if (sent==send_size)
				break;
		}
	}
	return (send_size);
}

/*******************************************************************************
rt_sok_msg_receive: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		rt_sok_msg_receive_nb	( SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	INT32 leni, rec, ret;
	char buffi[ 8];
	INT32 *lenp;
	//static char *pMsg;
	fd_set readSock;
	//struct timeval timeout;

	//timeout.tv_sec=0;
	//timeout.tv_usec=50000;
	FD_ZERO (&readSock);
	FD_SET (*socki, &readSock);

	/* --- get message length --- */
	rec=0;
	if ( traciSOK) 
		printf("%s [%d]: start %d  %ld\n", __FILE__, __LINE__, max_rec_size, sizeof (lenp)-rec);

	while (1) 
	{
		leni=recv (*socki, &buffi [rec], sizeof (lenp)-rec, 0);
		if ( traciSOK) printf("%s [%d]: leni=%d\n", __FILE__, __LINE__, leni);
		if (leni==SOCKET_ERROR) 
		{
			ret=WSAGetLastError();
			if (ret==WSAEWOULDBLOCK) 
			{
				ret=select (0, &readSock, NULL, NULL, NULL);
				if (ret==SOCKET_ERROR) 
				{
//					ret=WSAGetLastError();
//					#ifndef _AFXDLL
//					RtPrintf ("Rcv 1 Return is %d\n", ret);
//					#endif
					return (-1);
				}
				continue;
			}
			else
			{
//				#ifndef _AFXDLL
//				RtPrintf ("Rcv 2 Return is %d\n", ret);
//				#endif
				return (-2);
			}
		}
		else if (!leni) 
		{
			return (0);
		}
		else 
		{
			rec+=leni;
			if (rec==sizeof (lenp))
				break;
		}
	}

	lenp=(int*) buffi;
	if ( traciSOK) printf("%s [%d]: length=%d < %d\n", __FILE__, __LINE__, *lenp, max_rec_size);
	if (*lenp>max_rec_size)
		return( -3);

	/* --- get message --- */
	rec=0;
	while (1) 
	{
		leni=recv (*socki, (char *) msg_rec+rec, *lenp-rec, 0);
		if (leni==SOCKET_ERROR) 
		{
			ret=WSAGetLastError();
			if (ret==WSAEWOULDBLOCK) 
			{
				ret=select (0, &readSock, NULL, NULL, NULL);
				if (ret==SOCKET_ERROR) 
				{
//					ret=WSAGetLastError();
//					#ifndef _AFXDLL
//					RtPrintf ("Rcv 3 Return is %d\n", ret);
//					#endif
					return (-3);
				}
				continue;
			}
			else 
			{
//				#ifndef _AFXDLL
//				RtPrintf ("Rcv 4 Return is %d\n", ret);
//				#endif
				return (-4);
			}
		}
		else if (!leni) 
		{
			return (0);
		}
		else 
		{
			rec+=leni;
			if (rec>=*lenp)
				break;
		}
	}
	if (rec!=*lenp)
		return (-5);
	return (*lenp);
}


/* ---------------------------------------------------------------------------- */



/*******************************************************************************
rt_sok_msg_send: sends a message though a socket. You stay blocked until the 
				 message is out.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		0=NOK
				n  = length

*******************************************************************************/
int		rt_sok_msg_send		( SOCKET *socki, void *msg_send, INT32 send_size)
{
	int ret;
	/* --- send message length --- */
	if ( ret=send( *socki, (char *) &send_size, sizeof( send_size), 0) <= 0){
		ret = WSAGetLastError();
		printf ("Send 11 Return is %d", ret);
		return( 0);
	}

	/* --- send message --- */
	if ( ret=send( *socki, (char*) msg_send, send_size, 0) <= 0){
		ret = WSAGetLastError();
		printf ("Send 22 Return is %d", ret);
		return( 0);
	}

	return (send_size);
}

/*******************************************************************************
rt_sok_msg_recv: receives a message from a socket. You stay blocked until a 
message	comes. Format from HYPER board

socki			pointer to socket where msg is goint to come_send is to be sent
msg_send		pointer to structure (buffer, struct, ..) to be sent
max_rec_size	maximum size of message to be received

return	0,<0  = NOK: socket closed
n     = length

*******************************************************************************/
int		rt_sok_msg_recv	( SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	int ret;
	int msglen;
	INT32 len;

	/* --- get message length --- */
	len = recv( *socki, (char*) &msglen, sizeof(msglen), 0);
	if (len==0) return len;

	if ( len <= 0)
	{
		ret = WSAGetLastError();
		printf ("rt_sok_msg_recv 1 Error is %d\n", ret);
		return( -1);
	}

	if ( msglen > max_rec_size) return( -2);

	/* --- get message --- */
	len = recv( *socki, msg_rec, msglen, 0);
	if (len==0) return len;
	if ( len <= 0)
	{
		ret = WSAGetLastError();
		printf ("rt_sok_msg_recv 3 Error is %d\n", ret);
		return -3;
	}
	if (len!=msglen) return -4;

	return msglen;
}


/*******************************************************************************
rt_sok_msg_receive: receives a message from a socket. You stay blocked until a 
					message	comes.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		rt_sok_msg_receive	( SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	int ret;
	int leni, len;
	INT32 msgLen=0;
	char* pMsg;

	/* --- get message length --- */
	leni = recv(*socki, (char*)&msgLen, sizeof(msgLen), 0);
	if (leni == 0)
	{
		return(0);
	}
	if (leni < 0)
	{
		ret = WSAGetLastError();
		//printf ("Rec 11 Error is %d\n", ret);
		return(-1);
	}
	if (leni!=sizeof(msgLen))
	{
		return (-4);
	}
	if (msgLen > max_rec_size)
	{
		return(-2);
	}

	/* --- get message --- */
	pMsg = (char*)msg_rec;
	for (len = msgLen; len>0; )
	{
		leni = recv(*socki, pMsg, len, 0);
		if (leni < 0)
		{
			ret = WSAGetLastError();
			printf ("Rec 22 Error is %d\n", ret);
			return(-5);
		}
		len -= leni;
		pMsg += leni;
	}
	if (len)
		return(-3);
	return msgLen;
}


/*******************************************************************************
rt_sok_send_nb: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int rt_sok_send_nb (SOCKET *socki, void *msg_send, INT32 send_size)
{
	INT32 leni, sent, ret;
	fd_set writeSock;
	struct timeval timeout;

	timeout.tv_sec=0;
	timeout.tv_usec=50000;
	FD_ZERO (&writeSock);
	FD_SET (*socki, &writeSock);

	/* --- send message --- */
	sent=0;
	while (1) {
		leni=send (*socki, (char *) msg_send+sent, send_size-sent, 0);
		if (leni==SOCKET_ERROR) {
			ret=WSAGetLastError();
			if (ret==WSAEWOULDBLOCK) {
				ret=select (0, NULL, &writeSock, NULL, NULL);
				if (ret==SOCKET_ERROR) {
					ret=WSAGetLastError();
					printf ("Send 4 Return is %d\n", ret);
					return (-4);
				}
				continue;
			}
			else {
				printf ("Send 5 Return is %d\n", ret);
				return (-5);
			}
		}
		else if (!leni) {
			printf ("Send 6 Return is %d\n", ret);
			return (-6);
		}
		else {
			sent+=leni;
			if (sent==send_size)
				break;
		}
	}
	return (send_size);
}



/*******************************************************************************
rt_sok_receive: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		rt_sok_receive_nb	( SOCKET *socki, void *msg_rec, INT32 max_rec_size)
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
		if (leni==SOCKET_ERROR) {
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
rt_sok_size_send: sends a message though a socket. You stay blocked until the 
				 message is out.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		0=NOK
				n  = length

*******************************************************************************/
int		rt_sok_send_size		( SOCKET *socki, void *msg_send, INT32 send_size)
{
	int ret;

	/* --- send message --- */
	if ( ret=send( *socki, (char*) msg_send, send_size, 0) <= 0){
		ret = WSAGetLastError();
		printf ("Send 22 Return is %d", ret);
		return( 0);
	}

	return (send_size);
}


/*******************************************************************************
rt_sok_size_receive: receives a message from a socket. You stay blocked until a 
					message	comes.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int		rt_sok_receive_size	( SOCKET *socki, void *msg_rec, INT32 rec_size)
{
	int ret;
	INT32 leni, pos;
	char *pMsg;

	/* --- get message --- */
	pMsg= (char*) msg_rec;
	pos = rec_size;
	for (;;) {
		leni = recv( *socki, pMsg, pos, 0);
		if ( leni < 0){
			ret = WSAGetLastError();
			printf ("Rec 22 Error is %d\n", ret);
			return( -5);
		}
		if ( leni > rec_size){
			printf ("Rec 22 Error is %d\n", -6);
			return( -6);
		}
		pos -= leni;
		if ( pos <= 0) break;
		pMsg += leni;
	}
	if ( pos )
		return( -3);
	return( rec_size);
}


/*******************************************************************************
rt_sok_init_gj: initializes sockets, either for a client or a server 

	client		TRUE: it is a client that tries to connect to an existing end-point
				FALSE: it is a server that creates an end-point
	type		SOCK_STREAM or SOCK_DGRAM 
	host		name of host. default "localhost" 
	port		number of port. default "10051" 

  return:		0=OK
	
	socki		pointer to socket handle

*******************************************************************************/
int	rt_sok_init_gj ( int client, int type, char *host, int port, int rcvPort, MIC_SOCKET *socki)
{
	WSADATA	data;
	WORD version;						
	struct hostent *hostInfo;
	int ret=0;
	int init=TRUE;

#ifdef __cplusplus
	ASSERT(type==SOCK_STREAM || type==SOCK_DGRAM);
#else 
	if (type!=SOCK_STREAM && type!=SOCK_DGRAM)
//		error(ERR, 10, "%s", "undefined socket type");
	return (99);
#endif
	socki->type  = type;
	socki->errCode = 0;

	/* --- Startup Winsockets --- */
	if ( traciSOK) printf("Startup Winsockets\n");
	version= MAKEWORD (2,2);
	ret= WSAStartup (version, &data);
	if (ret) {
		printf("<<<<<<<<<<<<<<<<< version 1\n");
		version= MAKEWORD (1,1);
		ret= WSAStartup (version, &data);
	}
	if ( ret)
	{
		return( 1);
	}

	/* --- Get socket --- */
	if ( traciSOK) printf("Get socket\n");
	socki->socket = socket( AF_INET, type, 0);
	if ( socki->socket == INVALID_SOCKET){
		return(2);
	}

	socki->ipAddr.sin_family	  = AF_INET; 
	socki->ipAddr.sin_port	      = htons( (short) port);

	if (host!=NULL && isalpha(host[0])){       
		/* --- Resolve host name --- */  
		hostInfo = gethostbyname ((char*) host);
		if ( hostInfo==NULL)
		{
			socki->socket=INVALID_SOCKET;
			return(3);
		}
		memcpy((char *) &(socki->ipAddr.sin_addr), (char *) hostInfo->h_addr, hostInfo->h_length);
	}
	else{
		/* Convert nnn.nnn address to a usable one */
		if (host==NULL || !strlen (host))
			socki->ipAddr.sin_addr.S_un.S_addr = INADDR_ANY;
		else
			socki->ipAddr.sin_addr.S_un.S_addr = inet_addr( host);
	}

	if ( client){
		if ( traciSOK) printf("client  host [%s]  port [%d]\n", host, port);

	
		if ( traciSOK) printf("get host address\n");

		if (socki->type==SOCK_DGRAM) {
			struct	sockaddr_in rcvIpAddr;

			/* --- Socket options --- */
			if ( traciSOK) printf("setsockopt\n");
			if (setsockopt (socki->socket, SOL_SOCKET, SO_DONTROUTE, (char *) &init, sizeof (BOOL)))
			{
				sok_error(socki, __LINE__);
				return(4);
			}

//			if (setsockopt (socki->socket, SOL_SOCKET, SO_DONTLINGER, (char *) &init, sizeof (BOOL)))
//			{
//				socki->socket=INVALID_SOCKET;
//				return(3);
//			}

			//--- Get port Number --- 
			if ( traciSOK) printf("get port for >>>%s<<< port=%d\n", host, port);
			rcvIpAddr.sin_family = AF_INET;       
			rcvIpAddr.sin_addr.s_addr = INADDR_ANY;
			rcvIpAddr.sin_port = htons( (short) rcvPort);

			//--- Try to bind socket ---
			if ( traciSOK) printf("Try to bind socket\n");
			if ( bind( socki->socket, (struct sockaddr *) &rcvIpAddr, sizeof (rcvIpAddr)) != 0)
			{
				sok_error(socki, __LINE__);
				return(5);
			}

			/* --- Try to connect to socket --- */
			if ( traciSOK) printf("Try to connect socket\n");
			if ( connect( socki->socket, (struct sockaddr *) &socki->ipAddr, sizeof (socki->ipAddr) ) != 0)
			{
				sok_error(socki, __LINE__);
				return(6);
			}
		}
		else {
			/* --- Socket options --- */
			if ( traciSOK) printf("setsockopt\n");
			if (setsockopt (socki->socket, IPPROTO_TCP, TCP_NODELAY, (char *) &init, sizeof (BOOL)))
			{
				sok_error(socki, __LINE__);
				return(7);
			}

			/* --- Try to connect to socket --- */
			if ( traciSOK) printf("Try to connect socket\n");
			if ( connect( socki->socket, (struct sockaddr *) &socki->ipAddr, sizeof (socki->ipAddr) ) != 0)
			{
				sok_error(socki, __LINE__);
				return(8);
			}
		}
	}
	else{		
		if ( traciSOK) printf("host  port [%d]\n", port);
	
		if (socki->type==SOCK_STREAM) {

			/* --- Socket options --- */
			if (setsockopt (socki->socket, IPPROTO_TCP, TCP_NODELAY, (char*) &init, sizeof (BOOL)))
			{
				socki->socket=INVALID_SOCKET;
				return(11);
			}
		}
		else {
			struct	sockaddr_in rcvIpAddr;

			if ( traciSOK) printf("setsockopt\n");
			if (setsockopt (socki->socket, SOL_SOCKET, SO_DONTROUTE, (char *) &init, sizeof (BOOL)))
			{
				socki->socket=INVALID_SOCKET;
				return(4);
			}

			//--- Get port Number --- 
			if ( traciSOK) printf("get port for >>>%s<<< port=%d\n", host, rcvPort);
			rcvIpAddr.sin_family = AF_INET;       
			rcvIpAddr.sin_addr.s_addr = socki->ipAddr.sin_addr.S_un.S_addr;
			rcvIpAddr.sin_port = htons( (short) rcvPort);
		}
		/* --- Try to bind socket --- */
		if ( traciSOK) printf("Try to bind socket\n");
		if ( bind( socki->socket, (struct sockaddr *) &socki->ipAddr, sizeof (socki->ipAddr) ) != 0)
		{
			printf("Socker error %d\n", WSAGetLastError());
			socki->socket=INVALID_SOCKET;
			return(12);
		}
	}
	return (0);
}

/*******************************************************************************
rt_sok_close_gj: close sockets, either for a client or a server 

	client		TRUE: it is a client
				FALSE: it is a server 

*******************************************************************************/
void rt_sok_close_gj (MIC_SOCKET *socki)
{
	if (socki->socket!=INVALID_SOCKET)
	{
		if ( traciSOK) printf("rt_sok_close\n");
		closesocket( socki->socket);
		WSACleanup ();
	}
	socki->socket=INVALID_SOCKET;
}

/*******************************************************************************
sok_error:

*******************************************************************************/

int sok_error(MIC_SOCKET* pSocket, int line)
{
	int err=WSAGetLastError();
	TrPrintf(-1, "(rt_sok_api.c-%d) Socket error %d\n", line, err);
	if (pSocket==NULL) return err;

	pSocket->errCode = err;
	pSocket->errLine = line;

	pSocket->socket = INVALID_SOCKET;

	return pSocket->errCode;
}

/*******************************************************************************
sok_error:

*******************************************************************************/

int rt_sok_error(SOCKET *pSocket, char *file, int line)
{
	int errCode  = WSAGetLastError();
	TrPrintf(-1, "(%s-%d) Socket error %d\n", file, line, errCode);
	return errCode;
}

/*******************************************************************************
rt_sok_end_gj: 
*******************************************************************************/
void rt_sok_end_gj(void)
{
	if ( traciSOK) printf("WSACleanup\n");
	WSACleanup ();
}

/*******************************************************************************

*******************************************************************************/
void rt_sok_set_non_blocking(MIC_SOCKET *socki)
{
	unsigned long ul;

	/* --- init socket: Non-blocking mode --- */
	if ( traciSOK) printf("init socket: Non-blocking mode\n");

	ul=1;
	ioctlsocket (socki->socket, FIONBIO, &ul);
}

/*******************************************************************************

*******************************************************************************/
int rt_sok_set_no_delay(SOCKET socket)
{
	int val=1;
	return setsockopt (socket, IPPROTO_TCP, TCP_NODELAY, (char*) &val, sizeof (BOOL));
}

/*******************************************************************************
rt_sok_msg_receive_gj: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int	rt_sok_msg_receive_gj (MIC_SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	INT32 l;

	l = recv_nb (socki, msg_rec, max_rec_size, 0);

	if (l==0) return (l);
	if (l<0)  return (l-3);

	return (l);
}



/*******************************************************************************
rt_sok_msg_send_gj: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int rt_sok_msg_send_gj(MIC_SOCKET *socki, void *msg_send, INT32 send_size)
{
	INT32 len;

	// --- send message ---
	if (send_size) len = send_nb (socki, msg_send, send_size, MSG_DONTROUTE);
	else return 0;

	if (len==0) return (len);
	if (len<0)  return (len-3);

	return (len);
}

/*******************************************************************************

*******************************************************************************/
int recv_nb(MIC_SOCKET *s, void *buf, INT32 len, int flags)
{
	INT32 l;
	INT32 rec;
	int err;
	fd_set readSock;

	FD_ZERO (&readSock);
	FD_SET (s->socket, &readSock);

	rec=0;
	while(1) {
		l=recv (s->socket, &((char*)buf)[rec], len-rec, flags);
printf("l=%d   rec=%d > %d\n", l, rec, len);
		if (l==SOCKET_ERROR) {
			err = WSAGetLastError();
			if (err==WSAEWOULDBLOCK) {
				if (select (0, &readSock, NULL, NULL, NULL)==SOCKET_ERROR) 
					return (-1);
				else {
					continue;
				}
			}
			else {
				s->errCode=err;
//				printf("Receive Error=%d\n", err);
				return (-2);
			}
		}
		else if (s->type==SOCK_DGRAM) return(l);
		else if (l<=0) return (l);
		else {
			rec += l;
			if (rec>=len) {
				return(rec);
			}
		}
	}
}


/*******************************************************************************

*******************************************************************************/
int send_nb(MIC_SOCKET *s, const void *buf, INT32 len, int flags)
{
	INT32 l, sent, err;
	fd_set writeSock;

	FD_ZERO (&writeSock);
	FD_SET (s->socket, &writeSock);

	/* --- send message length --- */
	sent=0;
	while (1) {
//		if (s->type==SOCK_DGRAM)
//			l = sendto (s->socket, &(((char*)buf)[sent]), len-sent, flags|MSG_DONTROUTE, (struct sockaddr *)&s->ipAddr, sizeof(s->ipAddr));
//		else
			l = send (s->socket, &(((char*)buf)[sent]), len-sent, flags);

		if (l==SOCKET_ERROR) {
			err = WSAGetLastError();
			if (err==WSAEWOULDBLOCK) {
				if (select (0, NULL, &writeSock, NULL, NULL)==SOCKET_ERROR) 
					return (-1);
				else continue;
			}
			else {
				s->errCode = err;
				return (-2);
			}
		}
		else if (l==0) return (-3);
		else {
			sent+=l;
			if (sent==len) {
				return(sent);
			}
		}
	}
}


/*******************************************************************************
rt_sok_msg_send_feig: sends a message though a socket. Non blocking mode.

	socki		pointer to socket where msg_send is to be sent
	msg_send	pointer to structure (buffer, struct, ..) to be sent
	send_size	size of msg_send

  
	return:		< 0=NOK
				n  = length

*******************************************************************************/
int rt_sok_msg_send_feig(MIC_SOCKET *socki, void *msg_send, INT32 send_size)
{
	INT32 len;

	// --- send message ---
	if (send_size) len = send_nb (socki, msg_send, send_size, MSG_DONTROUTE);
	else return 0;

	if (len==0) return (len);
	if (len<0)  return (len-3);

	return (len);
}


/*******************************************************************************
rt_sok_msg_receive_gj: receives a message from a socket. Non blocking mode.

	socki			pointer to socket where msg is goint to come_send is to be sent
	msg_send		pointer to structure (buffer, struct, ..) to be sent
	max_rec_size	maximum size of message to be received

  return	0,<0  = NOK: socket closed
			n     = length

*******************************************************************************/
int	rt_sok_msg_receive_feig (MIC_SOCKET *socki, void *msg_rec, INT32 max_rec_size)
{
	INT32 l;

	l = recv_feig_nb (socki, msg_rec, max_rec_size, 0);

	if (l==0) return (l);
	if (l<0)  return (l-3);

	return (l);
}

int recv_feig_nb(MIC_SOCKET *s, void *buf, INT32 len, int flags)
{
	INT32 l;
	int rec;
	int err;
	int feig_len;
	char *ch;
	fd_set readSock;

	FD_ZERO (&readSock);
	FD_SET (s->socket, &readSock);

	rec=0;
	ch = buf;
	while(1) {
		l=recv (s->socket, &((char*)buf)[rec], len-rec, flags);
		feig_len = 255*(*(ch+1)) + *(ch+2);
//printf("l=%d   rec=%d > %d   | feig_len=%d  0=%02X 1=%02X 2=%02X\n", l, rec, len, feig_len, *ch, *(ch+1), *(ch+2));
		if (l==SOCKET_ERROR) {
			err = WSAGetLastError();
			if (err==WSAEWOULDBLOCK) {
				if (select (0, &readSock, NULL, NULL, NULL)==SOCKET_ERROR) 
					return (-1);
				else {
					continue;
				}
			}
			else {
				s->errCode=err;
//				printf("Receive Error=%d\n", err);
				return (-2);
			}
		}
		else if (s->type==SOCK_DGRAM) return(l);
		else if (l<=0) return (l);
		else {
			rec += l;
			if (rec>=feig_len) {
				return(rec);
			}
		}
	}
}

//--- sok_get_socket_name ---------------------------------------------------------------
char *sok_get_socket_name(SOCKET socket, char *namestr, char *ipstr, UINT32 *pport)
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
