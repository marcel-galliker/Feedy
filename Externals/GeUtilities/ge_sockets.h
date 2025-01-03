#pragma once

/* **************************************************************************** */
/*																				*/
/*    REAL TIME SOCKET COMMUNICATION API										*/
/*																				*/
/* ****************************************************************************	*/
/*																				*/
/*    Copyright 1998 Graph-Tech AG, Switzerland. All rights reserved.			*/
/*    Written by Markus Portmann.				               					*/
/*																				*/
/* ****************************************************************************	*/

/* --- Includes --------------------------------------------------------------- */
#include "ge_common.h"

#ifdef linux
	#include "errno.h"
	typedef int SOCKET;
	
	#define INVALID_SOCKET -1

	#define WSAETIMEDOUT ETIMEDOUT // 110

	
#else
	#include <winsock.h>
#endif

/* --- Macros ----------------------------------------------------------------- */


/* --- Defines ---------------------------------------------------------------- */


/* --- Structures ------------------------------------------------------------- */
										/* socket initialization */

//WORD	version;						
//WSADATA	data;
//u_long	para;  
//int		accessPoint;
//struct	sockaddr_in baseAddr;
//struct	hostent *hostInfo;
#ifdef unused
typedef struct  {
	int		type;
	int		errCode;
	int		errLine;
	SOCKET	socket;
	struct	sockaddr_in ipAddr;
} MIC_SOCKE

#endif
// typedef int socklen_t;


/* --- Prototypes ------------------------------------------------------------- */

int		ge_sok_open_client		(SOCKET *psocket, const char *addr, int port, int type, int timeout);

int		ge_sok_init_server		(char *host, int port, SOCKET *socki);
int 	ge_sok_set_no_delay		(SOCKET socket);
void	ge_sok_close			(SOCKET *socki);
int		ge_sok_msg_send			(SOCKET *socki, void *msg_send, INT32 send_size);
int		ge_sok_msg_receive		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		ge_sok_msg_recv			(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		ge_sok_send_size		(SOCKET *socki, void *msg_send, INT32 send_size);
int		ge_sok_receive_size		(SOCKET *socki, void *msg_rec,  INT32 rec_size);
int		ge_sok_msg_send_nb		(SOCKET *socki, void *msg_send, INT32 send_size);
int		ge_sok_msg_receive_nb	(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		ge_sok_set_blocking		(SOCKET *socki, int blocking);
int		ge_sok_receive_nb		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);

char*	ge_sok_get_socket_name		(SOCKET socket, char *namestr, char *ipstr, UINT32 *pport);
int		ge_sok_error			(SOCKET *pSocket, char *file, int line);
int		ge_sok_recv_timeout		(SOCKET socket, int ms);
int		ge_sok_send_timeout		(SOCKET socket, int ms);

/* ----------------------------------------------------------------------------*/

