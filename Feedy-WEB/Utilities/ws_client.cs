using Feedy_WEB.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


// https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API/Writing_WebSocket_server
// https://stackoverflow.com/questions/8125507/how-can-i-send-and-receive-websocket-messages-on-the-server-side

namespace Feedy_WEB.Source
{
    public class ws_client
    {

        private NetworkStream _Stream;
        
        public ws_client()
        {
        }
        
        ~ws_client()
        {
            Console.WriteLine("Destruct");
        }

        void END()
        {
            Console.Write("END");
        }

        //--- Start --------------------------------------
        public void Start(TcpClient client)
        {
            Thread thread = new Thread(_ws_clientThread);

            thread.Start(client);
        }
 
        //--- Send ---------------------------------------------
        public void Send(string msg)
        {
            if (_Stream!=null)
            {
                try
                {
                    byte[] data = _EncodeMessageToSend(msg);
                    _Stream.Write(data, 0, data.Length);
                }
                catch(Exception ex) 
                {
                    _Stream.Close();
                    _Stream = null;
                }
            }
        }

        //--- _EncodeMessageToSend -------------------------
        private static Byte[] _EncodeMessageToSend(String message)
        {
            Byte[] response;
            Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            Byte[] frame = new Byte[10];

            Int32 indexStartRawData = -1;
            Int32 length = bytesRaw.Length;

            frame[0] = (Byte)129;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (Byte)126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (Byte)127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int32 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

        //--- _ws_clientThread ---------------------------
        public void _ws_clientThread(object par)
        {
            TcpClient client = par as TcpClient;
            if (client!=null)
            {
                _Stream = client.GetStream();

                try
                {
                    while (_Stream!=null) 
                    {
                     //   while (!_Stream.DataAvailable);
                    //    byte[] bytes = new byte[client.Available];
                        byte[] bytes = new byte[1024];
                        int len=_Stream.Read(bytes, 0, bytes.Length);
                        Console.WriteLine("--- WebSocket from port={0} received {1} bytes", client.Client.RemoteEndPoint.ToString(), len);

                        String data = Encoding.UTF8.GetString(bytes);
                        if (Regex.IsMatch(data, "^GET")) 
                        {
                            Console.WriteLine("--- WebSocket from port={0} -----------------------------", client.Client.RemoteEndPoint.ToString());
                            Console.WriteLine(data);
                            Console.WriteLine("--------------------------------------------------------");

                            const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                                + "Connection: Upgrade" + eol
                                + "Upgrade: websocket" + eol
                                + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                                    System.Security.Cryptography.SHA1.Create().ComputeHash(
                                        Encoding.UTF8.GetBytes(
                                            new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                        )
                                    )
                                ) + eol
                                + eol);

                            _Stream.Write(response, 0, response.Length);
                        }
                        else
                        {
                            bool fin = (bytes[0] & 0b10000000) != 0,
                            mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
                            int opcode = bytes[0] & 0b00001111; // expecting 1 - text message
                            ulong offset = 2,
                            msglen = bytes[1] & (ulong)0b01111111;

                            if (msglen == 126) {
                                // bytes are reversed because websocket will print them in Big-Endian, whereas
                                // BitConverter will want them arranged in little-endian on windows
                                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                                offset = 4;
                            } else if (msglen == 127) {
                                // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                                // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                                // websocket frame available through client.Available).
                                msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] },0);
                                offset = 10;
                            }

                            if (msglen == 0) {
                                Console.WriteLine("msglen == 0");
                            } else if (mask) {
                                byte[] decoded = new byte[msglen];
                                byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                                offset += 4;

                                for (ulong i = 0; i < msglen; ++i)
                                    decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                                if (decoded.Length==2 && decoded[0] == 3 && decoded[1]==233) {
                                    _Stream.Close();
                                    _Stream = null;
                                    break;
                                }

                                string text = Encoding.UTF8.GetString(decoded);
                                Console.WriteLine("--- WebSocket from port={0} -----------------------------", client.Client.RemoteEndPoint.ToString());
                                Console.WriteLine(text);
                                Console.WriteLine("--------------------------------------------------------");
                                if (text.Length==2)
                                {
                                    Console.WriteLine("TEST");
                                }
                                if (FDGlobals.WebSocket.OnReceive!=null) FDGlobals.WebSocket.OnReceive(text);
                            } else
                                Console.WriteLine("mask bit not set");
                        }                    
                    }
                }
                catch(Exception e) {
                }
            }
         
            FDGlobals.WebSocket.ClientClosed(this);
        }
    }
}
