using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Feedy.Models
{
    public class FdTrace
    {
        private static int _Tick0 = Environment.TickCount;
        private static TextWriter _File;

        private static bool _Trace=true;


        //--- SetTraceDir ----------------------
        public static void SetTraceDir(string dir)
        {

        }

        //--- WriteLine -----------------------------------
        public static void WriteLine(string str,
                    [CallerMemberName] string name="",
                    [CallerLineNumber] int line=-1,
                    [CallerFilePath] string path="")
        {
            if (_Trace)
            {
                try
                {
                    if (_File == null) 
                    {
                        string dir;
                        if (Directory.Exists("D:")) dir = "D:";
                        else dir = "C:";
                        dir += "\\temp";
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        dir += "\\Feedy";
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        dir += "\\Feedy.txt";
                        _File = File.CreateText(dir);
                    }
                }
                catch(Exception) 
                { 
                    _File=null; 
                }
            }
            if (_Trace)
            { 
                _File.WriteLine("{0:000.000}: {1}", (Environment.TickCount-_Tick0)/1000.0, str);
                _File.Flush();
            }
        }
    }
}

