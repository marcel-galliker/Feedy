#pragma once


//--- defines ---------------------------------------------------
#define WS_SERVER_HOST	"localhost"
#define WS_SERVER_PORT 3000

void ws_init(void);
void ws_send(SOCKET socket, void *pmsg, int len);