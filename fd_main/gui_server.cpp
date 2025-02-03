
#include <string.h>
#include "ge_utilities.h"
#include "..\Externals\\CryptLib\sha1.h"
#include "..\Externals\\CryptLib/base64.h"
#include "gui_server.h"


static void _GuiServer_thread (void *ppar);
static void _GuiClient_thread (void *ppar);
static SOCKET _GuiServerSok=INVALID_SOCKET;
static SOCKET _GuiClientSok=INVALID_SOCKET;
static int    _LoginDone;
static void _login(SOCKET socket, char *msg, int len);

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
	_LoginDone=FALSE;
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

//--- _login -----------------------------------------------------
static void _login(SOCKET socket, char *msg, int len)
{
	char reply[1024];
	char keyAccept[128];
	memset(reply, 0, sizeof(reply));
	memset(keyAccept, 0, sizeof(keyAccept));
	int repLen=0;

	char *line=(char*)msg;
	for (int i=0; i<len; i++)
	{
		if (msg[i]=='\r')
		{
			msg[i]=0;
			printf("%s\n", line);
			int len=strlen("sec-websocket-key: ");
			if (!strnicmp(line, "sec-websocket-key: ", len))
			{
				//	const char *test_key="Sec-WebSocket-Key: jRuBF0LDp3Q9OJirluIZJG==";
				//	const char *test_accept="Sec-WebSocket-Accept: 2U1wM4jKbGHrsow5ubzxLFHULPY=";
				//	const char *test_key="Sec-WebSocket-Key: dGhlIHNhbXBsZSBub25jZQ==";
				//  const char *test_accept="Sec-WebSocket-Accept: s3pPLMBiTxaQ9kYGzzhZRbK+xOo=";
					const char *test_key="Sec-WebSocket-Key: 0KL2VEa4pM4XAGndahwriw==";
				//	const char *test_accept="Sec-WebSocket-Accept: jiCWDwQsaaD3h2jlEGq4RgrBE/E=";
				//  line = (char*)test_key;
			//	printf("KEY1=>>%s<<\n", &line[len]);
				strcpy(keyAccept, &line[len]);
				if (TRUE)
				{
					char key[128];
				//	line=(char*)test_key;
					printf("KEY: >>%s<<\n", &line[len]);
					sprintf(key, "%s%s", &line[len], "258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
					int l=strlen(key);
				//	printf("KEY2=>>%s<<\n", key);
					{
						SHA1_CTX ctxt;
						UINT8 code[SHA1_DIGEST_SIZE];
						memset(code, 0, sizeof(code));
						SHA1_Init  (&ctxt);
						SHA1_Update(&ctxt, (const UINT8*)key, strlen(key));
						SHA1_Final (&ctxt, code);

						int   c64l=32;
						int ret= base64_encode(code, SHA1_DIGEST_SIZE, keyAccept, &c64l);
						printf("Sec-WebSocket-Accept: %s\n", keyAccept);
					}
				}
			}
			line=(char*)&msg[i+2];
		}
	}
	if (*keyAccept)
	{
		repLen += sprintf(&reply[repLen], "HTTP/1.1 101 Switching Protocols\r\n");
		repLen += sprintf(&reply[repLen], "Connection: Upgrade\r\n");
		repLen += sprintf(&reply[repLen], "Upgrade: websocket\r\n");
		repLen += sprintf(&reply[repLen], "Sec-WebSocket-Accept: %s\r\n\r\n", keyAccept);
		int ret=send(socket, (char*)reply, repLen, 0);
		printf("LOGIN Reply:\n");
		printf(reply);
		printf("SENT LOGIN %d/%d bytes\n", repLen, ret);
		_LoginDone = TRUE;
	}
}

//--- _GuiClient_thread ----------------------------------------
static void _GuiClient_thread (void *ppar)
{
// https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_servers

	SOCKET clientSok = (SOCKET) ppar;
	while (clientSok!=INVALID_SOCKET)
	{
		UCHAR msg[1024];
		memset(msg, 0, sizeof(msg));
		int len=recv(clientSok, (char*)msg, sizeof(msg), 0);
		if (len>0)
		{
			printf("recv %d bytes >>%s<<\n", len, msg);
			_login(clientSok,(char*)msg, len);
		}
		else
		{
			printf("recv return=%d\n", len);
			closesocket(clientSok);
			_LoginDone = FALSE;
			clientSok = INVALID_SOCKET;
		}
	}
}
