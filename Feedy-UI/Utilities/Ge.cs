using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace GE_Utilities
{
    public partial class Ge
    {
        //--- MonthName --------------------
        public static string[] MonthName = {"", "JAN", "FEB", "MAR", "APR", "MAI", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};

        /*
        //--- Resource --------------------------------
        /// <summary>
        /// Finds the resource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public object Resource(string id)
        {
            return App.Current.FindResource(id);
        }
        */

        //--- CalledFrom --------------------------------------------
        /// <summary>
        /// Get name of calling function
        /// </summary>
        /// <param name="steps">number of steps back in calls</param>
        /// <returns>[File:line]</returns>
        public static string CalledFrom(int steps)
        {
            var stack = new StackTrace(true);
            var frame = stack.GetFrame(steps);
            return Path.GetFileName(frame.GetFileName())+":"+frame.GetFileLineNumber();
		}

        //--- StrToDouble ---------------------------------------
        /// <summary>
        /// Converts string to Double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double StrToDouble(string str)
        {
            double val=0;
            double weight=1;
            double sign=1.0;

            if (str==null) return 0;
            foreach(char c in str)
            {
                if (c=='-') sign=-1.0;
                if (c=='.' || c==',') weight=0.1;
                if (c>='0' && c<='9')
                {
                    if (weight==1) val=10*val+c-'0';
                    else 
                    {
                        val += weight*(c-'0');
                        weight/=10;
                    }
                }
               // else if (c>'9') 
               //     return double.NaN;
            }
                
            return val*sign; 
        }        

        //--- StrToUInt32 ------------------------------
        /// <summary>
        /// Converts string to UInt32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UInt32 StrToUInt32(string str)
        {
            UInt32 val=0;

            if (str==null) return 0;
            foreach(char c in str)
            {
                if (c=='.' || c==',') return val;
                if (c>='0' && c<='9') val=10*val+c-'0';
            }                
            return val; 
        }

        //--- HexToUInt32 ---------------------------------------------
        public static UInt32 HexToUInt32(string str)
        {
            UInt32 val=0;

            if (str==null) return 0;
            foreach(char c in str)
            {
                if (c=='.' || c==',') return val;
                else if (c>='0' && c<='9') val=16*val+c-'0';
                else if (c>='a' && c<='f') val=16*val+10+(byte)c-'a';  
                else if (c>='A' && c<='F') val=16*val+10+(byte)c-'A';  
            }                
            return val; 
        }

        //--- StrToInt32 ------------------------------
        /// <summary>
        /// Converts string to Int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Int32 StrToInt32(string str)
        {
            Int32 val=0;
            Int32 sign=1;

            if (str==null) return 0;
            foreach(char c in str)
            {
                if (c=='-') sign=-1;
                if (c=='.' || c==',') return val*sign;
                if (c>='0' && c<='9') val=10*val+c-'0';
            }                
            return val*sign; 
        }

        //--- StrToBool ------------------------------
        /// <summary>
        /// Converts string to bool
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StrToBool(string str)
        {
            if (str!=null && str.ToLower()=="true") return true;
            return false;
        }

        //--- StrToAscii --------------------
        static public string StrToAscii(string str)
        {
            Byte[] bytes=Encoding.ASCII.GetBytes(str);
            return Encoding.ASCII.GetString(bytes).Replace("?", "_");
        }

        //--- HideWndClose ----------------------------
        /// <summary>
        /// Hides the close button of a window. Must be called int the "loaded" callback.
        /// </summary>
        /// <param name="wnd">Window object</param>

                // Imports        
                [DllImport("user32.dll", SetLastError = true)]
                private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
                [DllImport("user32.dll")]
                private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        static public void HideWndClose(Window wnd)
        {
            const int GWL_STYLE = -16;
            const int WS_SYSMENU = 0x80000;
            var hwnd = new WindowInteropHelper(wnd).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        //--- BitInSet ---------------------------------
        static public bool BitInSet<type>(int set, type bit)
        {
            return (set & System.Convert.ToInt32(bit))!=0;
        }

        //--- SetBit ---------------------------------
        static public Int32 SetBit<type>(ref int set, type bit)
        {
            set |= System.Convert.ToInt32(bit);
            return set;
        }

        //--- ClrBit ---------------------------------
        static public Int32 ClrBit<type>(ref int set, type bit)
        {
            set &= ~System.Convert.ToInt32(bit);
            return set;
        }

        //--- ColorFromRgb --------------------------------
        static public System.Windows.Media.Brush BrushFromRgb(string str)
        {
            Color color =new Color();
            color.R = (byte)Ge.HexToUInt32(str.Substring(1,2));
            color.G = (byte)Ge.HexToUInt32(str.Substring(3,2));
            color.B = (byte)Ge.HexToUInt32(str.Substring(5,2));
            color.A = 0xff;
            return new SolidColorBrush(color);
        }
    }
}
