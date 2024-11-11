using Feedy.Services;
using GE_Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feedy.Models
{
    public class FeedyStatus : GeBindable
    {
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

    }
}
