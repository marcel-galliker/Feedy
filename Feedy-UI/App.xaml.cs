using Feedy.Models;
using GE_Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Feedy
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //--- _bringToFront --------------------------
        private void _bringToFront(string title) 
        {
            IntPtr handle = FindWindow(null, title);

            if (handle == IntPtr.Zero) return;

            SetForegroundWindow(handle);
            ShowWindow(handle, 3); // SW_MAXIMIZE
        }

        //--- App -------------------------------------------------------------
        public App()
        {
            Ge.Trace("Feedy started");
            string name = Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);
            if (Process.GetProcessesByName(name).Length > 1)
            {
                _bringToFront(name);
                Application.Current.Shutdown();
            }

            Application.Current.DispatcherUnhandledException += _DispatcherUnhandledException;
        }

        //--- DispatcherUnhandledException ---------------------------------
        private void _DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            StringBuilder msg= new StringBuilder(1024);
            msg.AppendLine(e.Exception.Message);
            msg.AppendLine("");
            msg.AppendLine("StackTrace:");

         //   EzTrace.WriteLine("Exception: "+e.Exception.Message);
         
            try
            {
                string[] lines=e.Exception.StackTrace.Split('\n');
                for (int i = 0; i < 10 && i<lines.Length; i++)
                    msg.Append(lines[i]);
            }
            catch { }
            Ge.Trace("Exception: "+msg.ToString());
         //  MessageBox.Show(msg.ToString(), "Exception");
            e.Handled = true;
        }
    }
}
