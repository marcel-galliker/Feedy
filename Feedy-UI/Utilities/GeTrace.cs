using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace GE_Utilities
{
    public partial class Ge
    {
        private static int          _Tick0=Environment.TickCount;

        private static  StreamWriter _TraceFile;
        private static  string       _TraceDir;
        private static  string       _ProjectDir;

        private static bool          _PrintSourcePos = false;

        //--- Ticks -------------------
        static public string Ticks()
        {
            int ticks= Environment.TickCount-_Tick0;
            return string.Format("{0:0.000}", ticks/1000.0);
        }

        //--- Trace ---------------------------
        static public void Trace(string message="",
                    [CallerMemberName] string name="",
                    [CallerLineNumber] int line=-1,
                    [CallerFilePath] string path="")
        {
            if (_TraceFile==null)
            {
                string appname=Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
                if (_ProjectDir==null)
                {
                    _ProjectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."))
                                + Path.DirectorySeparatorChar
                                + appname
                                + Path.DirectorySeparatorChar;
                }
                if (_TraceDir==null)
                {
                    _TraceDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    _TraceDir = Path.Combine(_TraceDir, appname);
                    _TraceDir = "d:\\Temp\\"+appname;
                    Directory.CreateDirectory(_TraceDir);
                }
                string tracepath=Path.Combine(_TraceDir, appname+".txt");
                try { if (File.Exists(tracepath)) File.Delete(tracepath); }
                catch (Exception){ };

                try { _TraceFile = new StreamWriter(tracepath);}
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                };
            }

            try
            {
                string filename=path.Remove(0, _ProjectDir.Length);
                string srcPos="";
                if (_PrintSourcePos)
                {
                    srcPos = String.Format("({0}:{1}){2}: ", filename, line, name);
                }
                if (message.Length>0)
                    _TraceFile.WriteLine("{0}: {1}{2}", Ticks(), srcPos, message);
                else
                    _TraceFile.WriteLine("{0}: {1}", Ticks(), srcPos);
                _TraceFile.Flush();
            }
            catch (Exception) { };
        }

        //--- TraceEnd -------------------------------
        static public void TraceEnd()
        {
            if (_TraceFile!=null)
            {
                _TraceFile.Close();
                _TraceFile=null;
            }
        }
    }
}
