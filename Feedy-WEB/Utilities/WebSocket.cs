using System.Net.Sockets;
using System.Net;
using System;
using Microsoft.AspNetCore.Hosting.Server;

namespace Feedy_WEB.Source
{

    public class CWebSocket
    {
        private TcpListener _server;
        private static List<ws_client> _Clients = new List<ws_client>();
        
        //--- constructor --------------------------------------------
        public CWebSocket()
        {
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        }

        //--- Start ------------------------------
        public void Start()
        {
            Task _server_task = new Task(() =>
            {
                _server.Start();
                Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection…", Environment.NewLine);
                while (true) 
                {
                    TcpClient client = _server.AcceptTcpClient();
                    Console.WriteLine("A client connected.");
                    ws_client thread= new ws_client();
                    _Clients.Add(thread);
                    thread.Start(client);
                }
            });

            _server_task.Start();
        }

        //--- Property OnReceive ---------------------------------------
        private Action<string> _OnReceive;
        public Action<string> OnReceive
        {
            get { return _OnReceive; }
            set { 
                    int cnt=_Clients.Count;
                    _OnReceive = value;
                }
        }

        //--- ClientClosed ------------------
        public void ClientClosed(ws_client client)
        {
            _Clients.Remove(client);
        }

        //--- Send ----------------------------------------------
        public void Send(string msg)
        {
            Console.WriteLine("WebSocket.send. cnt={0}", _Clients.Count);
            foreach(ws_client client in _Clients) 
                client.Send(msg);
        }

        //--- _server_task ------------------------
    }
}
