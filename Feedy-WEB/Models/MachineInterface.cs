using System.Net.Sockets;
using System.Text;

namespace Feedy_WEB.Models
{
    public class MachineInterface
    {
        static Thread _thread = null;
        private  static TcpClient _Client;
        static NetworkStream _Stream;
        
        public MachineInterface() 
        { 
            if (_thread==null)
            {
                _thread = new Thread(TcpIpThread);
                _thread.Start();
            }
        }

        //--- Send -------------------------------------
        public static void Send(string msg)
        {
            if (_Stream!=null)
            {
                byte[] buffer=Encoding.ASCII.GetBytes(msg);
                _Stream.Write(buffer, 0, buffer.Length);
            }
        }

        //--- Recv -----------------------------------------------------
        public static string Recv(int size)
        {
            if (_Stream==null) return null;

            byte[] buffer = new Byte[256];
            int ret= _Stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, ret);
        }

        //--- TcpIpThread --------------------------------------------------
        private void TcpIpThread()
        {
            while(true)
            {
                _Client = new TcpClient();
                byte[] buffer = new Byte[1024];
                try
                {
                    _Client.Connect("localhost", 1234);
                    _Stream = _Client.GetStream();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(1000);
                    continue;
                };

                Send("START");
                while (_Client.Client!=null)
                {
                    int ret= _Stream.Read(buffer, 0, buffer.Length);
                    string str = Encoding.ASCII.GetString(buffer).Remove(ret);
                    Console.WriteLine(str);
                }
                _Stream.Close();
            }
        }
    }
}