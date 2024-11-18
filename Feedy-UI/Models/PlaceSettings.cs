using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class PlaceSettings : GeBindable
    {
        //--- Set ------------------------------
        public void Set(SPlaceSettings set)
        {
            PlaceMode       = set.placeMode;
            ApproachHeight  = set.approachHeight;
            ApproachSpeed   = set.approachSpeed;
            OpenTime        = set.openTime;
        }

        //--- Get --------------------------------------
        public SPlaceSettings Get()
        {
            SPlaceSettings set=new SPlaceSettings();
            set.placeMode       = PlaceMode;
            set.approachHeight  = ApproachHeight;
            set.approachSpeed   = ApproachSpeed;
            set.openTime        = OpenTime;
            return set;
        }

        //--- Property PlaceMode ---------------------------------------
        private EnPlaceMode _PlaceMode;
        public EnPlaceMode PlaceMode
        {
            get { return _PlaceMode; }
            set { SetProperty(ref _PlaceMode, value); }
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

        //--- Property OpenTime ---------------------------------------
        private int _OpenTime;
        public int OpenTime
        {
            get { return _OpenTime; }
            set { SetProperty(ref _OpenTime, value); }
        }
    }
}
