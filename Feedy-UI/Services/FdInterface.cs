
using Feedy.Models;
using GE_Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Runtime.CompilerServices;

namespace Feedy.Services
{
    public class FdInterface : GeBindable
    {
        //--- Creator -----------------------------------------
        #region Creator
        
        static TcpClient        _Client;
        private Boolean         _Running;

        private static int cnt = 0;
        public FdInterface()
        {
        //    GeTrace.WriteLine("FdInterface.Creator start");
      //    System.Windows.MessageBox.Show("Start");    // REMOTE DEBUGGING

       //     Console.WriteLine("*** RxInterface {0}", DateTime.Now);

            if (_task == null)
            {
                Address  = "Localhost";//FdGolbals.PrinterProperties.IP_Address;
                _Running = true;
                _task = new Thread(TcpIpThread);
                _task.Start();
            }
            cnt++;
            if (cnt > 1) 
                throw new Exception("Must not be initialzed more than once");
        //    GeTrace.WriteLine("FdInterface.Creator end");
        }
        #endregion

        //--- ShutDown ---------------------------------------------------
        public void ShutDown()
        {
            _Running = false;
            if (_Client!=null) _Client.Close();
        }

        //--- destructor ---
        ~FdInterface()
        {
        }

        #region Properties
        //--- Property Running ---------------------------------------
        public bool Running
        {
            get { return _Running; }
            set { SetProperty(ref _Running, value); }
        }

        //---Property Connected ---------------------------------
        private Boolean _GuiConnected = false;
        public Boolean GuiConnected
        {
            get { return _GuiConnected; }
            private set
            {
                if (_Running) SetProperty(ref _GuiConnected, value);
            }
        }

        //--- Property Enabled ---------------------------------------
        private bool _Enabled=true;
        public bool Enabled
        {
            get { return _Enabled; }
            set { 
                    if (SetProperty(ref _Enabled, value))
                    {
                        // Connected = !_Enabled;
                    }
                }
        }

        //---Property Connected ---------------------------------
        private Boolean _Connected = false;
        public Boolean Connected
        {
            get { return _Connected; }
            set
            {
                if (_Running)
                {
                    if (SetProperty(ref _Connected, value))
                    {
                        if (_Connected) GuiConnected=true;
                        else
                        {
                            Thread t=new Thread(()=>
                            {
                                Thread.Sleep(1000);
                                if (!_Connected) GuiConnected=false;
                            });
                            t.Start();
                        }
                    }
                }
            }
        }

        //--- Property Address ----------------------------------------

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set 
            { 
                if (SetProperty(ref _Address, value))
                {
                    if (_Client!=null && _Client.Client.Connected)
                    { 
                        _Client.Close();
                    }
                }
            }
        }

        //--- Property LocalAddress ---------------------------------------
        private string _LocalAddress;
        public string LocalAddress
        {
            get { return _LocalAddress; }
            private set 
            { 
                string str=value;
                int pos = str.IndexOf(":");
                if (pos>0) SetProperty(ref _LocalAddress, str.Substring(0,pos)); 
                else SetProperty(ref _LocalAddress, value);
            }
        }        

       #endregion


        private List<string> _LocalIpAddresses()
        {
            List<string> addresses = new List<string>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                foreach(var addr in adapterProperties.UnicastAddresses)
                {
                    addresses.Add(addr.Address.ToString());
                }
            }

            return addresses;
        }

        //--- _Start_PrintEngine -----------------------
        private void _Start_PrintEngine()
        {
            return;
            try
            {
                List<string> addresses=_LocalIpAddresses();
                if (addresses.Contains(Address))
                {
                    string bin_dir=AppDomain.CurrentDomain.BaseDirectory;
                //  string bin_dir="D:\\EZ-Editor\\bin\\EZ-Editor";   // for debugging

                    string name="EZ-PrintEngine";
                    int idx=bin_dir.IndexOf("bin");
                    if (idx<0) return; // started from debugger

                    bin_dir = bin_dir.Remove(idx+4);

                    Process[] list=Process.GetProcesses();
                    foreach(Process p in list) 
                    {
                    //  Console.WriteLine(p.Id+":  "+p.ProcessName);
                        if (p.ProcessName==name)
                        {
                            Console.WriteLine("EZ-PrintEngine alerady running");
                            return;
                        }
                    }

                    //--- starting process -----------------------------
                    ProcessStartInfo info=new ProcessStartInfo();
                    info.FileName=bin_dir+name+Path.DirectorySeparatorChar+name+".exe";
                    info.WindowStyle = ProcessWindowStyle.Minimized;
                    Process.Start(info);
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

        //--- MAIN Worker Thread --------------------------------------------------
        static Thread _task = null;
     //   static LogList _log = new LogList();
        static NetworkStream _Stream;

        private void TcpIpThread()
        {
            byte[] buffer = new Byte[6144];

            Int32 read, len;
            int size;
            GuiMsgHandler msgHandler = new GuiMsgHandler();
            
            while (GeBindable.Dispatcher==null)
                Thread.Sleep(100);

           _Start_PrintEngine();

            while (_Running)// && !FdGolbals.Settings.Demo)
            {
                //--- connecting -------------------------------------------
    
             //   Console.WriteLine("*** TcpIpThread started {0}", DateTime.Now);

                while (_Running && !_Connected)
                {
                    _Client = new TcpClient();
                    try 
                    {
                        _Client.Connect(Address, EzGuiMsg.PORT_GUI); 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    };

            //        Console.WriteLine("*** Connected {0}", DateTime.Now);

                    Connected = true;
            //        log.AddItem(new LogItem() { Error = 1010, Message = "Connected to Server" });
                }
                if ( Connected && _Client.Client!=null)
                {
                    LocalAddress = _Client.Client.LocalEndPoint.ToString();

                    while(true)
                    { 
                        try
                        {
                            _Stream = _Client.GetStream();
                            break;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Exception {0}", e.Message);
                        }
                    }
                    msgHandler.Reset();

                    FdGolbals.Job.LoadList();

                    /*
                    if (!RxNetUse.ShareConnected(RxGlobals.PrinterProperties.RippedDataPath))
                    {
                        RxNetUse.DeleteShare(RxGlobals.PrinterProperties.RippedDataPath);
                        RxNetUse.OpenShare(RxGlobals.PrinterProperties.RippedDataPath, "radex", "radex");
                    }
                    */

                    //--- handling messages --------------------------------------------
                   // bool msg_len=false;
                   // bool msg_split=false;
                   // TcpIp.SInkSupplyStatMsg msg=new TcpIp.SInkSupplyStatMsg();
                    SMsgHdr hdr;
                    while (_Running && Connected)
                    {
                        try
                        {
                            size = 4;
                            read = _Stream.Read(buffer, 0, size);
                            if (read <= 0) break;
                            len = BitConverter.ToInt32(buffer, 0);
                            
                            if (len>buffer.Length)
                            {
                                GeStructConvert.ToStruct(out hdr, buffer);
                             //   GeGlobals.Events.AddItem(new LogItem(string.Format("Receive Buffer Overflow. Last good msg(id:{0:X} len:{1})", msg.hdr.msgId, msg.hdr.msgLen, msg.no)));
                                Console.WriteLine("Receive Buffer Overflow. Last good msg(id:{0:X} len:{1})", hdr.msgId, hdr.msgLen);
                                break;
                            }
                            else
                            {
                                while(read>0 && size<len)
                                {
                                    read = _Stream.Read(buffer, size, len-size);
                                    size += read;
                                }
                                if (read <= 0) break;
                                GeStructConvert.ToStruct(out hdr, buffer);
                                GeBindable.Invoke(()=>msgHandler.handle_message(buffer));  
                            }
                        }
                        catch (Exception e)
                        {
                        //    if (_Running) _log.AddItem(new LogItem() { Error = 1010, Message = e.Message });
                            if (_Running) Console.WriteLine("Exception {0}", e.Message);
                            break;
                        }
                    }
                //  Console.WriteLine("Main Disconnected");
                }
                if (_Running) Connected = false;
                _Client.Close();
            };
        }

        //--- SendCommand ------------------------------------------------------------
        public bool SendCommand(UInt32 cmd)
        {
            if (Connected && _Stream!=null)
            {
                SMsgHdr hdr=new SMsgHdr();
                hdr.msgLen = Marshal.SizeOf(hdr);
                hdr.msgId = cmd;
                byte[] buffer;
                GeStructConvert.ToBuffer<SMsgHdr>(out buffer, hdr);
                try
                {
                    if (_Stream!=null && _Stream.CanWrite)
                        _Stream.Write(buffer, 0, hdr.msgLen);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("TCP/IP Send Exception: >>{0}<<", e.Message);
                    if (_Running) Connected = false;
                    _Client.Close();
                }
            }
            return false;
        }

        //--- SendMsg ----------------------------------------
        public bool SendMsg<type>(UInt32 msgId, ref type msg)
        {
            if (/*Connected && */ _Stream!=null)
            {
                byte[] buffer;
                GeStructConvert.ToBuffer<type>(out buffer, msg);
                SMsgHdr hdr= new SMsgHdr() { msgId = msgId, msgLen = buffer.Count() };
                GeStructConvert.OverwriteBuffer(buffer, new SMsgHdr(){msgId=msgId, msgLen=buffer.Count()});

                try
                {
                    _Stream.Write(buffer, 0, buffer.Count());
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("TCP/IP Send Exception: >>{0}<<", e.Message);
                    Connected = false;
                    _Client.Close();
                }
            }
            return false;
        }

        //--- SendMsgBuf ----------------------------------------
        public void SendMsgBuf(UInt32 msgId, string str)
        {
            if (Connected)// && !FdGolbals.Settings.Demo)
            {
                char[] msg = new char[str.Length];
                str.CopyTo(0, msg, 0, str.Length);
                int hdrsize  = Marshal.SizeOf(typeof(SMsgHdr));
                byte[] buffer = new byte[hdrsize+msg.Length+1];
                for (int i=0; i<msg.Length; i++)
                    buffer[hdrsize+i] = (byte)msg[i];
                GeStructConvert.OverwriteBuffer(buffer, new SMsgHdr(){msgId=msgId, msgLen=buffer.Count()});
                try
                {
                    _Stream.Write(buffer, 0, buffer.Count());
                }
                catch (Exception e)
                {
                    Console.WriteLine("TCP/IP Send Exception: >>{0}<<", e.Message);
                    Connected = false;
                    _Client.Close();
                }
            }
        }
    }
}
