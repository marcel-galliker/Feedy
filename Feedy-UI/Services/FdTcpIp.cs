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
        //		GUI_.... message sent from GUI to printer
        //		EP_...   message sent drom printer to GUI

    //    public const Int32 GUI_GET_EVT			= 0x01000001;
    //    public const Int32 EP_EVENT_RESET		= 0x02000001;

        public const Int32 GET_STATUS           = 0x01000001;
        public const Int32 REP_PRINTER_STATUS   = 0x02000002;
        public const Int32 REP_HEAD_STATUS      = 0x02000003;
        public const Int32 SET_POWER			= 0x01000004;
        public const Int32 GET_HYPER_STATUS	    = 0x01000005;
        public const Int32 GET_HYPER_IO_STATUS	= 0x01000006;
        public const Int32 GET_HYPER_TEMPS	    = 0x01000007;
        public const Int32 GET_CAM_RESULT		= 0x01000008;
        public const Int32 GET_HYPER_INKSYS_STATUS = 0x01000009;
        public const Int32 GET_FERAG_STATUS     = 0x0100000a;
        public const Int32 GET_BX_INKSYS_STATUS = 0x01000011;
        public const Int32 GET_BX_HEAD_CONFIG   = 0x01000012;

        public const Int32 REP_PC22_INK_DATA	= 0x02000008;

        public const Int32 LIST_JOB_START		= 0x01001001;
        public const Int32 LIST_JOB_FILE		= 0x01001002;
        public const Int32 LIST_JOB_END			= 0x01001003;

        public const Int32 LIST_DATAFILE_START	= 0x01001004;
        public const Int32 LIST_DATAFILE_FILE	= 0x01001005;
        public const Int32 LIST_DATAFILE_END	= 0x01001006;

        public const Int32 LOAD_JOB				= 0x01001011;
        public const Int32 LOAD_JOB_LABEL		= 0x01001012;
        public const Int32 LOAD_JOB_TEXTBOX		= 0x01001013;
        public const Int32 LOAD_JOB_LOADED		= 0x01001014;

        public const Int32 SAVE_JOB				= 0x01001015;
        public const Int32 SAVE_JOB_LABEL		= 0x01001016;
        public const Int32 SAVE_JOB_TEXTBOX		= 0x01001017;

        public const Int32 RUN_JOB				= 0x01001021;
        public const Int32 STOP_JOB				= 0x01001022;
        public const Int32 ABORT_JOB			= 0x01001023;
        public const Int32 CMD_PURGE            = 0x01001024;
        public const Int32 CMD_SPIT             = 0x01001025;
        public const Int32 CMD_PRINTGO_UNUSED   = 0x01001026;
        public const Int32 CMD_OUT_TEST         = 0x01001027;
        public const Int32 CMD_RESET_MENISCUS   = 0x01001028;

        public const Int32 RECORD_STATUS		= 0x01001101;

        public const Int32 GET_LICENSE			= 0x01002001;
        public const Int32 REP_LICENSE			= 0x02002001;
        public const Int32 REP_VERSION			= 0x03002001;
        public const Int32 GET_COM_PORTS		= 0x01002002;
        public const Int32 REP_COM_PORTS		= 0x02002002;

        public const Int32 LOAD_PRINTER_CFG		= 0x01002011;
        public const Int32 SAVE_PRINTER_CFG		= 0x01002012;
        public const Int32 LOAD_PC22_PEN_CFG	= 0x01002013;
        public const Int32 SAVE_PC22_PEN_CFG	= 0x01002014;

        public const Int32 LOAD_DATAFILE		= 0x01003001;

        public const Int32 DEL_LAYOUT_START		= 0x01004031;

        public const Int32 ERROR_RESET			= 0x01005001;
        public const Int32 ERROR_EVENT			= 0x01005002;
        public const Int32 ERROR_MARQUEE		= 0x01005003;

        public const Int32 PROD_STAT_EVENT		= 0x01005101;

        public const Int32 CMD_BX_RESET		    = 0x01006002;
        public const Int32 CMD_BX_JETS_ON		= 0x01006003;
        public const Int32 CMD_BX_JETS_OFF		= 0x01006004;
        public const Int32 CMD_BX_DO_PHASE		= 0x01006005;
        public const Int32 CMD_BX_GET_PHASE		= 0x01006006;
        public const Int32 CMD_BX_BACK_FLUSH	= 0x01006007;
        public const Int32 CMD_BX_TEST_SPEED	= 0x01006008;
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

    //==== JOB ====================================================
    //--- SJobMsg -------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SJobMsg
    {
	    public SMsgHdr  hdr;			// LOAD_JOB / SAVE_JOB
	    public SJob	    job;
    }
}