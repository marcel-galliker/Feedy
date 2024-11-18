using Feedy.Models.Enums;
using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class TestSettings : GeBindable
    {
        //--- Set ------------------------------
        public void Set(STestSettings set)
        {
            TestMode  = set.testMode;
        }

        //--- Get --------------------------------------
        public STestSettings Get()
        {
            STestSettings set=new STestSettings();
            set.testMode  = TestMode;
            return set;
        }

        //--- Property TestMode ---------------------------------------
        private EnTestMode _TestMode;
        public EnTestMode TestMode
        {
            get { return _TestMode; }
            set { SetProperty(ref _TestMode, value); }
        }


    }
}
