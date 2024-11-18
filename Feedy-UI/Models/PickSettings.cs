using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class PickSettings : GeBindable
    {
        //--- Set ------------------------------
        public void Set(SPickSettings set)
        {
            ApproachHeight  = set.approachHeight;
            ApproachSpeed   = set.approachSpeed;
            CloseTime       = set.closeTime;
        }

        //--- Get --------------------------------------
        public SPickSettings Get()
        {
            SPickSettings set=new SPickSettings();
            set.approachHeight  = ApproachHeight;
            set.approachSpeed   = ApproachSpeed;
            set.closeTime       = CloseTime;
            return set;
        }

        //--- Property ApproachHeight ---------------------------------------
        private int _ApproachHeight;
        public int ApproachHeight
        {
            get { return _ApproachHeight; }
            set { SetProperty(ref _ApproachHeight, value); }
        }

        //--- Property ApproachSpeed ---------------------------------------
        private int _ApproachSpeed;
        public int ApproachSpeed
        {
            get { return _ApproachSpeed; }
            set { SetProperty(ref _ApproachSpeed, value); }
        }


        //--- Property CloseTime ---------------------------------------
        private int _CloseTime;
        public int CloseTime
        {
            get { return _CloseTime; }
            set { SetProperty(ref _CloseTime, value); }
        }
    }
}
