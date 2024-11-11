using System;
using System.Runtime.InteropServices;
using TECIT.TBarCode;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Feedy.Services
{
    public class FdDef
    {

        //--- SJob ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SJob
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string description;
        };        

        //--- EStatType -------------------------------------
        public enum EStatType : Int32
        {
            ST_UNDEF,   // 0
            ST_START,   // 1    
            ST_STOP,    // 2 
            ST_SPEED,   // 3
        };

    }
}
