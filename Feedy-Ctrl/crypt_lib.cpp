#ifndef linux

#include "crypt_lib.h"
#include "rijndael.h"
#include "ge_common.h"

#include <ctime>

#define BLOCK_SIZE 16
const char * KEY = "/141/168/252/193/31/48/236/158/202/171/77/74/155/255/58/6/26/168/16/77/41/11/115/194/47/7/146/104/118/206/172/17";  // must be 32!
const char * CHAIN = "/12/116/122/9/228/109/118/160/187/79/214/170/100/66/118/160/67/166/133/6/154/73/230/65/8/221/214/173/79/169/208/235";

static void generate_numbers()
{
	char code[256];
	int len=0;
	memset(code, 0, sizeof(code));
	srand ((UINT32)time(NULL));
	len = sprintf(&code[len], "\"");
	for (int i=0; i<32; i++)
	{
		len += sprintf(&code[len], "/%d", rand()%256);
	}
	len = sprintf(&code[len], "\"");
}

int decrypt(byte *crypt, int clen, byte *buffer, int blen)
{
	//	generate_numbers();
	CRijndael oRijndael;
	oRijndael.MakeKey(KEY, CHAIN, 32, BLOCK_SIZE);
	oRijndael.Decrypt((const char*)crypt, (char*)buffer, clen);
	return clen;
}

int encrypt(byte *buffer, int blen, byte *crypt, int clen)
{
	char in[1024];
	if (blen<sizeof(in))
	{
		memset(in, 0, sizeof(in));
		memset(crypt, 0, clen);
		memcpy(in, buffer, blen);
		blen = ((blen+BLOCK_SIZE-1)/BLOCK_SIZE) * BLOCK_SIZE;
		if (clen>=blen)
		{
			CRijndael oRijndael;
			oRijndael.MakeKey(KEY, CHAIN, 32, BLOCK_SIZE);
			oRijndael.Encrypt((const char*)in, (char*) crypt, blen, CRijndael::ECB);
			return blen;
		}
	}
	return 0;
}

#endif