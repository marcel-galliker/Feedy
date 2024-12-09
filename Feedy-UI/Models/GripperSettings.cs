using GE_Utilities;
using System.IO;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class GripperSettings : GeBindable
    {
        public void Set(SGripper grp)
        {
            No    = grp.gripperNo;
            Name  = grp.name;
            Type  = grp.type;
        }

        //--- Get -----------------------------------
        public SGripper Get()
        {
            SGripper grp = new SGripper();
            grp.gripperNo = No;
            grp.name      = Name;
            grp.type      = Type;
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

        //--- Property Name ---------------------------------------
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { if (SetProperty(ref _Name, value))
                    ImagePath=Path.Combine(FdGolbals.Job.Dir,   value); 
                }  
        }

        //--- Property ImagePath ---------------------------------------
        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set { SetProperty(ref _ImagePath, value); }
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
