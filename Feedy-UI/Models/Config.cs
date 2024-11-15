using Feedy.Services;
using GE_Utilities;
using System.Collections.ObjectModel;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class Config : GeBindable
    {
        private bool _Loaded=false;

        //--- constructor --------------------------------
        public Config()
        {
            if (_Motor==null)
            {
                _Motor = new ObservableCollection<MotorSettings>();
                for (int i = 0; i < feedy_def.MotorCnt; i++) 
                    _Motor.Add(new MotorSettings());
            }
        }

        //--- Save --------------------------------------------
        public void Save(string filename = null)
        {
            if (!_Loaded) return;

            SCfgMsg msg = new SCfgMsg();
            msg.cfg = Get();  
            FdGolbals.FdInterface.SendMsg(GuiMsg.SAVE_CFG, ref msg);
        }

        //--- Set --------------------------------
        public void Set(SConfig msg)
        {
            for (int i = 0; i < feedy_def.MotorCnt; i++)
            {
                Motor[i].Set(msg.motor[i]);
            }
            _Loaded = true;
        }

        //--- Get --------------------------------
        public SConfig Get()
        {
            SConfig cfg=new SConfig();
            cfg.motor = new SMotorSettings[feedy_def.MotorCnt];
            for (int i = 0; i < feedy_def.MotorCnt; i++)
            {
                cfg.motor[i] = Motor[i].Get();
            }
            return cfg;
        }

        //--- Property Motor ---------------------------------------
        private ObservableCollection<MotorSettings> _Motor;
        public ObservableCollection<MotorSettings> Motor
        {
            get { return _Motor; }   
            set { SetProperty(ref _Motor, value); }
        }

    }
}
