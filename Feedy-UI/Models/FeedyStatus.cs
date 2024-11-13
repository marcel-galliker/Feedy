using Feedy.Services;
using GE_Utilities;
using System.Collections.ObjectModel;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class FeedyStatus : GeBindable
    {
        //--- constructor ----------------------------------
        public FeedyStatus()
        {       
            if (_MotorPos==null)
            {
                _MotorPos = new ObservableCollection<double> { 0, 0, 0, 0 };
            }
            TestSpeed = 20;
        }

        //--- Set() ----------------------------------------------
        public void Set(SFeedyStatus msg)
        {
            for (int i=0; i< _MotorPos.Count; i++) 
            {
                MotorPos[i] = msg.motorPos[i];
            }
            TestSpeed = msg.testSpeed;
        }

        //--- Property JobState ---------------------------------------
        private EnJobState _JobState;
        public EnJobState JobState
        {
            get { return _JobState; }
            set { SetProperty(ref _JobState, value);}
        }

        //--- Property IsRunning ---------------------------------------
        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set { SetProperty(ref _IsRunning, value); }
        }

        //--- Property TestSpeed ---------------------------------------
        private int _TestSpeed;
        public int TestSpeed
        {
            get { return _TestSpeed; }
            set { SetProperty(ref _TestSpeed, value); }
        }

        //--- Property MotorPos ---------------------------------------
        private ObservableCollection<double> _MotorPos;
        public ObservableCollection<double> MotorPos
        {
            get { return _MotorPos; }   
            set { SetProperty(ref _MotorPos, value); }
        }

    }
}
