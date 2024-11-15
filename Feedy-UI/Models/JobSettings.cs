using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class JobSettings : GeBindable
    {
        public JobSettings()
        {
            if (PickPlace==null) 
                PickPlace=new PickPlace();
        }

        //--- Set ----------------------------------------
        public void Set(SJob job)
        {
            Name        = job.name;   
            Description = job.description;
            CamPrgNo    = job.camPrgNo;
            Speed       = job.speed;
            Count       = job.count;
            Feeder.Set(job.feeder);
            Cam.Set(job.cam);
            Tray.Set(job.tray);
            Gripper.Set(job.gripper);
            PickPlace.Set(job.pickPlace);
        }

        //--- Get -----------------------------------
        public SJob Get()
        {
            SJob job        = new SJob();
            job.name        = Name; 
            job.description = Description;
            job.camPrgNo    = CamPrgNo;
            job.speed       = Speed;
            job.count       = Count;
            job.feeder      = Feeder.Get();
            job.cam         = Cam.Get();
            job.tray        = Tray.Get();
            job.gripper     = Gripper.Get();
            job.pickPlace   = PickPlace.Get();
            return job;
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

        //--- Property Bin ---------------------------------------
        private FeederSettings _Feeder = new FeederSettings();
        public FeederSettings Feeder
        {
            get { return _Feeder; }
            set { SetProperty(ref _Feeder, value); }
        }

        //--- Property Cam ---------------------------------------
        private CamSettings _Cam = new CamSettings();
        public CamSettings Cam
        {
            get { return _Cam; }
            set { SetProperty(ref _Cam, value); }
        }

        //--- Property Tray ---------------------------------------
        private TraySettings _Tray = new TraySettings();
        public TraySettings Tray
        {
            get { return _Tray; }
            set { SetProperty(ref _Tray, value); }
        }

        //--- Property Gripper ---------------------------------------
        private GripperSettings _Gripper = new GripperSettings();
        public GripperSettings Gripper
        {
            get { return _Gripper; }
            set { SetProperty(ref _Gripper, value); }
        }

        //--- Property Name ---------------------------------------
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { IsChanged |= SetProperty(ref _Name, value);}
        }

        //--- Property Description ---------------------------------------
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { IsChanged |= SetProperty(ref _Description, value); }
        }

        //--- Property camPrgNo ---------------------------------------
        private int _camPrgNo;
        public int CamPrgNo
        {
            get { return _camPrgNo; }
            set { SetProperty(ref _camPrgNo, value); }
        }

        //--- Property Speed ---------------------------------------
        private int _Speed;
        public int Speed
        {
            get { return _Speed; }
            set { SetProperty(ref _Speed, value); }
        }

        //--- Property Count ---------------------------------------
        private int _Count;
        public int Count
        {
            get { return _Count; }
            set { SetProperty(ref _Count, value); }
        }

        //--- Property PickPlace ---------------------------------------
        private PickPlace _PickPlace;
        public PickPlace PickPlace
        {
            get { return _PickPlace; }
            set { SetProperty(ref _PickPlace, value); }
        }
    }
}
