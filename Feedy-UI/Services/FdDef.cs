using System;
using System.Runtime.InteropServices;
using TECIT.TBarCode;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Feedy.Services
{
    public class FdDef
    {
        //--- SConfig ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SConfig
        {
            public UInt32   cfg; 
        };        

        //--- SJob ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SJob
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string description;
        };        

        //--- EnJobState ------------------
        public enum EnJobState : Int32
        {
	        JS_undef,
	        JS_off,
	        JS_starting,
	        JS_printing,
	        JS_stopping,
            JS_done,
	        JS_aborted
        }

        //--- SStatusMsg ---------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SStatus
        {
	        public Int32		connected;
	        public EnJobState	jobState;	
        };

    }
}
