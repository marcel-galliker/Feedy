using GE_Utilities;
using static Feedy.Services.FdDef;

namespace Feedy.Models
{
    public class CamSettings : GeBindable
    {
        //--- Set ----------------------------------------
        public void Set(SCam cam)
        {
            Intensity = cam.intensity;
            SideUsed  = cam.sideUsed!=0;
            Trigger   = cam.trigger;
        }

        //--- Get -----------------------------------
        public SCam Get()
        {
            SCam cam = new SCam();
            cam.intensity = Intensity;
            cam.sideUsed  = SideUsed? 1:0;
            cam.trigger   = Trigger;
            return cam;
        }
        
        //--- Property IsChanged ---------------------------------------
        private bool _IsChanged=false;
        public bool IsChanged
        {
            get { return _IsChanged; }
            set { 
                   // if(IsLoading) SetProperty(ref _IsChanged, false);
                   // else 
                    {
                        SetProperty(ref _IsChanged, value);
                    }
                }
        }

        //--- Property Intensity ---------------------------------------
        private int _Intensity;
        public int Intensity
        {
            get { return _Intensity; }
            set { SetProperty(ref _Intensity, value); }
        }

        //--- Property SideUsed ---------------------------------------
        private bool _SideUsed;
        public bool SideUsed
        {
            get { return _SideUsed; }
            set { SetProperty(ref _SideUsed, value); }
        }

        //--- Property Trigger ---------------------------------------
        private EnCamTrigger _Trigger;
        public EnCamTrigger Trigger
        {
            get { return _Trigger; }
            set { SetProperty(ref _Trigger, value); }
        }
    }
}
