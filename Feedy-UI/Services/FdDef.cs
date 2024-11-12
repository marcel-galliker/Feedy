using System;
using System.Runtime.InteropServices;

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

        //--- SFeeder ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SFeeder
        {
            public Int32 used;
            public Int32 speed;
            public Int32 slope;
            public Int32 turns;
            public Int32 maxTurns;
        };

        //--- EnGripperType ------------------
        public enum EnGripperType : Int32
        {
            GRP_Vacc,
            GRP_Std,
            GRP_Inverse
        };

        //--- SGripper ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SGripper
        {
            public Int32            no;
            public EnGripperType    type;
            public Int32            time1;
            public Int32            time2;
        };
                        
        //--- EnJobState ------------------
        public enum EnCamTrigger : Int32
        {
            CAM_early,
            CAM_late
        };
 
        //--- SCam ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SCam
        {
            public Int32 intensity;
            public Int32 sideUsed;
            public EnCamTrigger trigger;
        };

                //--- STray ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct STray
        {
            public Int32 no;
            public Int32 time1;
            public Int32 pause;
            public Int32 time2;
            public Int32 repeats;
            public Int32 waitShake;
            public Int32 waitFill;
            public Int32 tries;
            public Int32 startFeed;
            public Int32 stopFeed;
        };

        //--- SJob ----------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)] 
        public struct SJob
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string   name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string   description;
            public Int32    camPrgNo;
            public Int32    speed;
            public Int32    count;
            public SFeeder  feeder;
            public SCam     cam;
            public STray    tray;
            public SGripper gripper;
        };

        //--- EnJobState ------------------
        public enum EnJobState : Int32
        {
	        JS_undef,
	        JS_off,
	        JS_starting,
	        JS_running,
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
