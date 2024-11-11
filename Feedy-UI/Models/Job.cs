using Feedy.Services;
using GE_Utilities;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using static Feedy.Services.FdDef;

namespace Feedy.Models
{
    public class Job : GeBindable
    {
        public FileList Files = new FileList();

        private bool   _Loaded=false;
        
        //--- Constructor --------------------------------------
        public Job()
        {
        }

        //--- LoadLastJob ------------------------------------
        public void LoadLastJob()
        {
        //  if (FdGolbals.FdInterface.Connected) return;
            Load("");
        }

        //--- Update ----------------------------------------
        public void Update(SJob job)
        {
            IsLoading       = true;

            FileName        = job.name;   
            Description     = job.description;

            IsLoading        = false;
            _Loaded          = true;    
        }

        //--- Property IsLoading ---------------------------------------
        private static int _IsLoadingCnt=0;
        public bool IsLoading
        {
            get { return _IsLoadingCnt>0; }
            set { 
                    if (value) _IsLoadingCnt++;
                    else       _IsLoadingCnt--;
                }
        }

        //--- Property IsChanged ---------------------------------------
        private bool _IsChanged=false;
        public bool IsChanged
        {
            get { return _IsChanged; }
            set { 
                    if(IsLoading) SetProperty(ref _IsChanged, false);
                    else 
                    {
                        SetProperty(ref _IsChanged, value);
                    }
                }
        }

        //--- LoadList -----------------------------------------------
        public void LoadList()
        {
            FdGolbals.FdInterface.SendCommand(EzGuiMsg.LIST_JOB_START);
        //    SFileMsg msg = new SFileMsg() { filename = "" };
        //   FdGolbals.FdInterface.SendMsg(EzGuiMsg.LOAD_JOB, ref msg);
        }

        //--- Property ListLoaded ---------------------------------------
        private int _ListLoaded;
        public int ListLoaded
        {
            get { return _ListLoaded; }
            set { SetProperty(ref _ListLoaded, value); }
        }

        //--- Load -----------------------------------------------
        public void Load(string filename=null)
        {
            IsLoading = true;

            if (filename!=null && filename!="") FileName = filename;
            SFileMsg msg = new SFileMsg() { filename = FileName };
        //  if(FdGolbals.License.Type==License.LicenseType.LIC_Printer)
            FdGolbals.FdInterface.SendMsg(EzGuiMsg.LOAD_JOB, ref msg);

            IsLoading = false;
        }

        //--- GetJob -----------------------------------
        public SJob GetJob()
        {
            SJob job = new SJob();
            job.name        = FileName; 
            job.description = Description;
            return job;
        }

        //--- Save --------------------------------------------
        public void Save(string filename = null)
        {
            if (!_Loaded) return;

            if (filename != null) FileName = filename;

            if (FdGolbals.User.Type < EN_UserType.USER_supervisor)
            {   // save changed distances ...
                SJobMsg msg = new SJobMsg();
                msg.job= GetJob();  
                FdGolbals.FdInterface.SendMsg(EzGuiMsg.SAVE_JOB, ref msg);
            }
            else
            {
            //    FdGolbals.PrinterCfg.Save();
                SJobMsg msg = new SJobMsg();
                msg.job= GetJob();
                FdGolbals.FdInterface.SendMsg(EzGuiMsg.SAVE_JOB, ref msg);

            //    FdGolbals.Layout.Save();
            }
            IsChanged = false;
        }

        //--- SaveNew ---------------------------------------
        public void SaveNew(string filename)
        {
            string dir=Path.GetDirectoryName(FdGolbals.Job.Dir)+Path.DirectorySeparatorChar+filename;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            FileName = filename;

            SJob    job     = GetJob();
        }

        //--- SaveAs --------------------------------------
        public void SaveAs(string filename)
        {
            string dir=Path.GetDirectoryName(FdGolbals.Job.Dir)+Path.DirectorySeparatorChar+filename;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            try
            {
                string srcDir = FdGolbals.Job.Dir;
                string[] files=Directory.GetFiles(srcDir);
                foreach (string file in files)
                {
                    if (Path.GetExtension(file)!=".xml")
                    {
                        string dest=dir+Path.DirectorySeparatorChar+Path.GetFileName(file);
                        File.Copy(file, dest, true);
                    }
                }
            }
            catch(Exception)
            { }

            Save(filename);
        }

        //--- Delete --------------------------------------
        public bool Delete(string filename)
        {
            if (filename==FileName)
            {
                string msg = FdGolbals.Language.GetError("EzEditor.Jobs.cs", "10", "Delete");
                MessageBox.Show(msg, "Error");
                return false;
            }
            else
            {
                string dir=FdGolbals.Dir.Jobs+filename;
                Directory.Delete(dir, true);
                return true;
            }
        }


        //--- Property FileName ---------------------------------------
        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { IsChanged |= SetProperty(ref _FileName, value);}
        }

        //--- Property Description ---------------------------------------
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { IsChanged |= SetProperty(ref _Description, value); }
        }

        //--- Property Dir ---------------------------------------
        private string _Dir;
        public string Dir
        {
            get { 
                    PingReply pr = new Ping().Send(FdGolbals.Settings.IpAddress, 100);
                    if (pr.Status==IPStatus.Success)
                    { 
                        string dir=FdGolbals.Dir.Jobs+FileName;
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        return dir; 
                    }
                    return null;
                }
            set { SetProperty(ref _Dir, value); }
        }

        //--- Start -----------------------------------------------
        public void Start()
        {
            if (FdGolbals.Job.FileName==null)
            {
                FdGolbals.Events.AddError(0, "No job loaded!");
                return;
            }
            FdGolbals.FdInterface.SendCommand(EzGuiMsg.RUN_JOB);
        }

        //--- Stop ----------------------------------
        public void Stop()
        {
            PrintingSpeed = 0;
            FdGolbals.FdInterface.SendCommand(EzGuiMsg.STOP_JOB);
        }

        //--- Property IsRunning ---------------------------------------
        private bool _IsRunning=false;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set { SetProperty(ref _IsRunning, value); }
        }

        //--- Property PrintingSpeed ---------------------------------------
        private double _PrintingSpeed;
        public double PrintingSpeed
        {
            get { return _PrintingSpeed; }
            set { 
                    SetProperty(ref _PrintingSpeed, value);
                }
        }

        //--- Property MaxRecNo ---------------------------------------
        private int _MaxRecNo;
        public int MaxRecNo
        {
            get { return _MaxRecNo; }
            set { SetProperty(ref _MaxRecNo, value); }
        }

    }
}
