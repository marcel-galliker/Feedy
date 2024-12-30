#ifndef RT_SOK_API
#define RT_SOK_API

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

int		sok_open_client			(SOCKET *psocket, const char *addr, int port, int type, int timeout);

int		rt_sok_init_server		(char *host, int port, SOCKET *socki);
int 	rt_sok_set_no_delay		(SOCKET socket);
void	rt_sok_close			(SOCKET *socki);
int		rt_sok_msg_send			(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_msg_recv			(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_send_size		(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_receive_size		(SOCKET *socki, void *msg_rec,  INT32 rec_size);
int		rt_sok_msg_send_nb		(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive_nb	(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_set_blocking		(SOCKET *socki, int blocking);
int		rt_sok_receive_nb		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);

char*	sok_get_socket_name		(SOCKET socket, char *namestr, char *ipstr, UINT32 *pport);
int		rt_sok_error			(SOCKET *pSocket, char *file, int line);
int		rt_sok_recv_timeout		(SOCKET socket, int ms);
int		rt_sok_send_timeout		(SOCKET socket, int ms);

/* ----------------------------------------------------------------------------*/

#endif

