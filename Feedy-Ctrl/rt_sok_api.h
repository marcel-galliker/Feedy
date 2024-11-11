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

#include <winsock.h>

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

typedef struct  {
	int		type;
	int		errCode;
	int		errLine;
	SOCKET	socket;
	struct	sockaddr_in ipAddr;
} MIC_SOCKET;

typedef int socklen_t;


/* --- Prototypes ------------------------------------------------------------- */

int		sok_open_client			(SOCKET *psocket, const char *addr, int port, int type, int timeout);
int		sok_sockaddr			(struct sockaddr_in *ipAddr, const char *addr, int port);

void 	ShowData				(char *data, int len);
int		rt_sok_init				(int client, char *host, int port, SOCKET *socki);
int		rt_sok_init_udp			(char *host, int port, SOCKET *socki);
int 	rt_sok_set_no_delay		(SOCKET socket);
void	rt_sok_close			(SOCKET *socki);
int		rt_sok_msg_send			(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_msg_recv			(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_send_size		(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_receive_size		(SOCKET *socki, void *msg_rec,  INT32 rec_size);
int		rt_sok_msg_send_nb		(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive_nb	(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_set_nb			(SOCKET *socki);
int		rt_sok_send_nb			(SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_receive_nb		(SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int 	rt_sok_msg_send_udp		(SOCKET *socki, void *msg_send, INT32 send_size, struct sockaddr_in *send_udp);
int		rt_sok_msg_receive_udp	(SOCKET *socki, void *msg_rec,  INT32 max_rec_size, struct sockaddr_in *si_other);
char*	sok_get_socket_name		(SOCKET socket, char *namestr, char *ipstr, UINT32 *pport);
int		rt_sok_error			(SOCKET *pSocket, char *file, int line);
int		rt_sok_recv_timeout		(SOCKET socket, int ms);
int		rt_sok_send_timeout		(SOCKET socket, int ms);

// with MIC_SOCKET
void	rt_sok_set_non_blocking	(MIC_SOCKET *socki);
int		rt_sok_init_gj			(int client, int type, char *host, int port, int rcvPort, MIC_SOCKET *socki);
void	rt_sok_close_gj			(MIC_SOCKET *socki);
void	rt_sok_end_gj			(void);
int		rt_sok_msg_send_gj		(MIC_SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive_gj	(MIC_SOCKET *socki, void *msg_rec,  INT32 max_rec_size);
int		rt_sok_msg_send_feig	(MIC_SOCKET *socki, void *msg_send, INT32 send_size);
int		rt_sok_msg_receive_feig	(MIC_SOCKET *socki, void *msg_send, INT32 send_size);
int		recv_feig_nb			(MIC_SOCKET *s, void *buf, INT32 len, int flags);
int		get_adapter_ip_addr		(char *adapter, char *ipAddr, INT32 len);
int		rt_sok_ping				(int objectType, int objectNo, char *ipAddress);
int		recv_nb					(MIC_SOCKET *s, void *buf, INT32 len, int flags);
int		send_nb					(MIC_SOCKET *s, const void *buf, INT32 len, int flags);
int		sok_error				(MIC_SOCKET *pSocket, int line);
int		sok_ping				(const char *addr);

/* ----------------------------------------------------------------------------*/

#endif

