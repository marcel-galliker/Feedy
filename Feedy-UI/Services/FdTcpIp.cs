using System;
using System.Runtime.InteropServices;
using static Feedy.Services.FdDef;

namespace Feedy.Services
{
    //--- constants --------------------------------------------------
    public class EzGuiMsg
    {
        public const Int32 PORT_GUI = 10000; // TCP-IP Port

        //--- message IDs ------------------------------------------
        public const Int32 GET_STATUS           = 0x01000001;
        public const Int32 REP_STATUS           = 0x02000002;

        public const Int32 LIST_JOB_START		= 0x01001001;
        public const Int32 LIST_JOB_FILE		= 0x01001002;
        public const Int32 LIST_JOB_END			= 0x01001003;

        public const Int32 LOAD_CFG				= 0x01001011;
        public const Int32 SAVE_CFG				= 0x01001012;

        public const Int32 LOAD_JOB				= 0x01001021;
        public const Int32 LOAD_JOB_LOADED		= 0x01001022;

        public const Int32 SAVE_JOB				= 0x01001023;

        public const Int32 RUN_JOB				= 0x01001031;
        public const Int32 STOP_JOB				= 0x01001032;
        public const Int32 ABORT_JOB			= 0x01001033;

        public const Int32 REP_VERSION			= 0x03002001;

        public const Int32 ERROR_RESET			= 0x01003001;
        public const Int32 ERROR_EVENT			= 0x01003002;
    }

    //--- SGuiMsgHdr -----------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SMsgHdr
    {
        public Int32 msgLen;   // length of the entire message, inclusive header
        public UInt32 msgId;	// message identifier
    }

    //--- SGuiMsg ---------------------------
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SGuiMsg
    {
	    public SMsgHdr hdr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
	    public string	data;
    }

    //--- SFeedyStatusMsg ---------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFeedyStatusMsg
    {
        public SMsgHdr hdr;
        public Int32 connected;
    }

    //--- SFileMsg -------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SFileMsg
    {
        public SMsgHdr hdr;            // LOAD_LAYOUT_START / SAVE_LAYOUT_START / DEL_LAYOUT_START
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string filename;
    }

    //--- SCfgMsg -------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SCfgMsg
    {
	    public SMsgHdr  hdr;			
	    public SConfig	cfg;
    }
    
    //--- SJobMsg -------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SJobMsg
    {
	    public SMsgHdr  hdr;			// LOAD_JOB / SAVE_JOB
	    public SJob	    job;
    }

    //--- SStatusMsg -------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SStatusMsg
    {
	    public SMsgHdr  hdr;			
	    public SStatus	status;
    }
}