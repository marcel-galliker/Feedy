

#include "gui_server.h"


static void _GuiServer_thread (void *ppar);
static void _GuiClient_thread (void *ppar);
static SOCKET _GuiServerSok=INVALID_SOCKET;
static SOCKET _GuiClientSok=INVALID_SOCKET;

HANDLE _GuiServerHdl;
HANDLE _GuiClientHdl;

//--- gui_server -------------------------------------------
gui_server::gui_server()
{
	_GuiServerHdl=CreateThread ( 
		NULL,									/* no security attributes */
		0,										/* default stack size */
		(LPTHREAD_START_ROUTINE) &_GuiServer_thread,	/* function to call */
		NULL,									/* parameter for function */
		0,										/* 0=thread runs immediately after being called */
		NULL									/* returns thread identifier */
	);
}

//--- _GuiServer_thread ---------------------------------
static void _GuiServer_thread (void *ppar)
{
	printf("_GuiServer_thread\n");

	/*
	int	i;
	for (i=0; i<MAX_GUI_CLIENTS; i++)
	{
		_GuiClientSok[i]=INVALID_SOCKET;
		_GuiClientThreadHandle[i]=NULL;
	}
	*/

	// --- take the socket and listen ---
	//	rt_sok_init (FALSE, GUI_HOST, GUI_PORT, &_GuiServerSok);
	/* --- Startup Winsockets --- */
	WSADATA	data;
	struct	sockaddr_in baseAddr;
	struct	hostent *hostInfo;

	int version= MAKEWORD (2,0);
	int ret= WSAStartup (version, &data);
	if (ret) 
	{
		version= MAKEWORD (1,1);
		ret= WSAStartup (version, &data);
	}

	_GuiServerSok = socket( AF_INET, SOCK_STREAM, 0);

	/* --- Get port Number --- */
	baseAddr.sin_addr.s_addr = INADDR_ANY;
	baseAddr.sin_family = AF_INET;       
	baseAddr.sin_port = htons( (short) GUI_SERVER_PORT);

	hostInfo = gethostbyname ((char*) GUI_SERVER_HOST);
	memcpy((char *) &(baseAddr.sin_addr), (char *) hostInfo->h_addr, hostInfo->h_length);
	int val=TRUE;
	ret=setsockopt (_GuiServerSok, IPPROTO_TCP, TCP_NODELAY, (char*) &val, sizeof (BOOL));
	ret=bind(_GuiServerSok, (struct sockaddr *) &baseAddr, sizeof (baseAddr));
	listen (_GuiServerSok, 10);
	while(TRUE)
	{
		_GuiClientSok=accept (_GuiServerSok, NULL, NULL);
		if (_GuiClientSok!=INVALID_SOCKET)
		{
			BOOL bNoDelay = TRUE;
			setsockopt (_GuiClientSok, IPPROTO_TCP, TCP_NODELAY, (LPSTR) &bNoDelay, sizeof (BOOL));

			printf("GUI connected\n");
			_GuiClientHdl=CreateThread ( 
				NULL,									/* no security attributes */
				0,										/* default stack size */
				(LPTHREAD_START_ROUTINE) &_GuiClient_thread,	/* function to call */
				(void*)_GuiClientSok,									/* parameter for function */
				0,										/* 0=thread runs immediately after being called */
				NULL									/* returns thread identifier */
			);

		}
	}
}

//--- _GuiClient_thread ----------------------------------------
static void _GuiClient_thread (void *ppar)
{
	SOCKET clientSok = (SOCKET) ppar;
	while (clientSok!=INVALID_SOCKET)
	{
		UCHAR msg[1024];
		memset(msg, 0, sizeof(msg));
		int len=recv(clientSok, (char*)msg, sizeof(msg), 0);
		if (len>0)
		{
			printf("recv %d bytes >>%s<<\n", len, msg);
			int ret=send(clientSok, (char*)msg, len, 0);
			printf("send %d bytes\n", len);
		}
		else
		{
			printf("recv return=%d\n", len);
			closesocket(clientSok);
			clientSok = INVALID_SOCKET;
		}


	}
}


