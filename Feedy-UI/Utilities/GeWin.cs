using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Feedy.Utilities
{
    public class GeWin
    {
        private struct SearchData
        {
            public string exename;
            public IntPtr hWnd;
            public TextWriter file;
            public int cnt;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RealGetWindowClass(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);
          
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int OpenProcess(int dwDesiredAccess, bool  bInheritHandle, int dwProcessId);

        [DllImport("Psapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetProcessImageFileNameW(int hProcess, StringBuilder lpFilename, int size);

        [DllImport("USER32.DLL", EntryPoint="SetForegroundWindow")]
        private static extern bool _SetForegroundWindow(IntPtr hWnd);

//        private static StreamWriter _file;
//        private static int _windowCnt;

        //--- SetForegroundWindow --------------------------------
        public static bool SetForegroundWindow(IntPtr hwnd)
        {
            return _SetForegroundWindow(hwnd);
        }

        //--- FindWindow --------------------
        public static IntPtr FindWindow(string exeName)
        {
            SearchData search=new SearchData();
            search.exename = exeName;
        //    search.file = File.CreateText("D:\\temp\\trace.txt");
        //    search.file?.WriteLine("Search for: "+exeName);
            EnumWindows(_enumWindows, ref search);
            search.file?.Close();
            return search.hWnd;
        }

        //--- _enumWindows ----------------------------------------------
        private static bool _enumWindows(IntPtr hWnd, ref SearchData search)
        {
            StringBuilder title = new StringBuilder(1024);
            GetWindowText(hWnd, title, title.Capacity);
            try
            {
                search.file?.WriteLine("--- "+search.cnt++);
                search.file?.WriteLine("title=>>"+title.ToString()+"<<");
                int id=0;
                GetWindowThreadProcessId(hWnd, ref id);
                //PROCESS_QUERY_INFORMATION=0x0400 | PROCESSVM_READ=0x0010
                int hprocess=OpenProcess(0x0400 | 0x0010, false, id);
                GetProcessImageFileNameW(hprocess, title, title.Capacity);
                search.file?.WriteLine("ModuleName=>>"+title.ToString()+"<<");
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            if (search.file!=null && title.ToString()!="")
            {
            }
            if (Path.GetFileName(title.ToString())==search.exename)
            {
                search.hWnd = hWnd;
            }
            return true;
        }

    }
}
