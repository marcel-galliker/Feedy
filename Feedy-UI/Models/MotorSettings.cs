using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class MotorSettingsList : GeBindable
    {
        //--- Property Motors-----------------------------------
        private MotorSettings[] _Motors;
        public MotorSettings[] Motors
        {
            get { return _Motors; }
            set { SetProperty(ref _Motors, value); }
        }

        public MotorSettingsList(int cnt)
        {
            Motors = new MotorSettings[cnt];
            for (int i=0; i<cnt; i++) 
            {
                Motors[i] = new MotorSettings();
                Motors[i].PowerHold = 10*i;
            }
        }
    }

    public class MotorSettings : GeBindable
    { 
        //--- Set ---------------------------------
        public void Set(SMotorSettings set)
        {
            PowerHold   = set.powerHold;
            PowerMove   = set.powerMove;
            Acc         = set.acc;
            Speed       = set.speed;
            TestPosStart= set.testPosStart;
            TestPosEnd  = set.testPosEnd;
            TestSpeed   = set.testSpeed;
        }

        //--- Get --------------------------------
        public SMotorSettings Get() 
        {
            SMotorSettings set = new SMotorSettings();

            set.powerHold   = PowerHold;   
            set.powerMove   = PowerMove;   
            set.acc         = Acc;         
            set.speed       = Speed;
            set.testPosStart= TestPosStart;
            set.testPosEnd  = TestPosEnd; 
            set.testSpeed   = TestSpeed; 
            return set;
        }

        //--- Property powerHold ---------------------------------------
        private int _powerHold;
        public int PowerHold
        {
            get { return _powerHold; }
            set { SetProperty(ref _powerHold, value); }
        }

        //--- Property powerMove ---------------------------------------
        private int _powerMove;
        public int PowerMove
        {
            get { return _powerMove; }
            set { SetProperty(ref _powerMove, value); }
        }

        //--- Property acc ---------------------------------------
        private int _acc;
        public int Acc
        {
            get { return _acc; }
            set { SetProperty(ref _acc, value); }
        }

        //--- Property speed ---------------------------------------
        private int _speed;
        public int Speed
        {
            get { return _speed; }
            set { SetProperty(ref _speed, value); }
        }

        //--- Property testPosStart ---------------------------------------
        private int _testPosStart;
        public int TestPosStart
        {
            get { return _testPosStart; }
            set { SetProperty(ref _testPosStart, value); }
        }

        //--- Property testPosEnd ---------------------------------------
        private int _testPosEnd;
        public int TestPosEnd
        {
            get { return _testPosEnd; }
            set { SetProperty(ref _testPosEnd, value); }
        }

        //--- Property testSpeed ---------------------------------------
        private int _testSpeed;
        public int TestSpeed
        {
            get { return _testSpeed; }
            set { SetProperty(ref _testSpeed, value); }
        }
    }
}
