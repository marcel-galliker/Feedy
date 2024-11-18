using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class GripperSettings : GeBindable
    {
        public void Set(SGripper grp)
        {
            No    = grp.gripperNo;
            Type  = grp.type;
        }

        //--- Get -----------------------------------
        public SGripper Get()
        {
            SGripper grp = new SGripper();
            grp.gripperNo = No;
            grp.type  = Type;
            return grp;
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

        //--- Property No ---------------------------------------
        private int _No;
        public int No
        {
            get { return _No; }
            set { SetProperty(ref _No, value); }
        }

        //--- Property Type ---------------------------------------
        private EnGripperType _Type;
        public EnGripperType Type
        {
            get { return _Type; }
            set { SetProperty(ref _Type, value); }
        }
    }
}
