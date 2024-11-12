using GE_Utilities;
using static Feedy.Services.FdDef;

namespace Feedy.Models
{
    public class TraySettings : GeBindable
    {
        //--- Set ----------------------------------------
        public void Set(STray tray)
        {
            No          = tray.no;
            Time1       = tray.time1;
            Pause       = tray.pause;
            Time2       = tray.time2;
            Repeats     = tray.repeats;
            WaitShake   = tray.waitShake;
            WaitFill    = tray.waitFill;
            Tries       = tray.tries;
            StartFeed   = tray.startFeed;
            StopFeed    = tray.stopFeed;
        }

        //--- Get -----------------------------------
        public STray Get()
        {
            STray tray      = new STray();
            tray.no         = No;
            tray.time1      = Time1;
            tray.pause      = Pause;
            tray.time2      = Time2;
            tray.repeats    = Repeats;
            tray.waitShake  = WaitShake;
            tray.waitFill   = WaitFill;
            tray.tries      = Tries;
            tray.startFeed  = StartFeed;
            tray.stopFeed   = StopFeed;
            return tray;
        }

        //--- Property No ---------------------------------------
        private int _No;
        public int No
        {
            get { return _No; }
            set { SetProperty(ref _No, value); }
        }

        //--- Property Time1 ---------------------------------------
        private int _Time1;
        public int Time1
        {
            get { return _Time1; }
            set { SetProperty(ref _Time1, value); }
        }

        //--- Property Pause ---------------------------------------
        private int _Pause;
        public int Pause
        {
            get { return _Pause; }
            set { SetProperty(ref _Pause, value); }
        }

        //--- Property Time2 ---------------------------------------
        private int _Time2;
        public int Time2
        {
            get { return _Time2; }
            set { SetProperty(ref _Time2, value); }
        }

        //--- Property Repeats ---------------------------------------
        private int _Repeats;
        public int Repeats
        {
            get { return _Repeats; }
            set { SetProperty(ref _Repeats, value); }
        }

        //--- Property WaitShake ---------------------------------------
        private int _WaitShake;
        public int WaitShake
        {
            get { return _WaitShake; }
            set { SetProperty(ref _WaitShake, value); }
        }

        //--- Property WaitFill ---------------------------------------
        private int _WaitFill;
        public int WaitFill
        {
            get { return _WaitFill; }
            set { SetProperty(ref _WaitFill, value); }
        }

        //--- Property Tries ---------------------------------------
        private int _Tries;
        public int Tries
        {
            get { return _Tries; }
            set { SetProperty(ref _Tries, value); }
        }

        //--- Property StartFeed ---------------------------------------
        private int _StartFeed;
        public int StartFeed
        {
            get { return _StartFeed; }
            set { SetProperty(ref _StartFeed, value); }
        }

        //--- Property StopFeed ---------------------------------------
        private int _StopFeed;
        public int StopFeed
        {
            get { return _StopFeed; }
            set { SetProperty(ref _StopFeed, value); }
        }






    }
}
