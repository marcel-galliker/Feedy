using Feedy.Services;
using GE_Utilities;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows;
using static Feedy.Services.feedy_def;

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

        //--- LoadList -----------------------------------------------
        public void LoadList()
        {
            FdGolbals.FdInterface.SendCommand(GuiMsg.LIST_JOB_START);
        //    SFileMsg msg = new SFileMsg() { filename = "" };
        //   FdGolbals.FdInterface.SendMsg(GuiMsg.LOAD_JOB, ref msg);
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
            if (filename!=null && filename!="") FdGolbals.JobSettings.Name = filename;
            SFileMsg msg = new SFileMsg() { filename = FdGolbals.JobSettings.Name };
            FdGolbals.FdInterface.SendMsg(GuiMsg.LOAD_JOB, ref msg);
            _Loaded=true;
        }

        //--- Save --------------------------------------------
        public void Save(string filename = null)
        {
            if (!_Loaded) return;

            if (filename != null) FdGolbals.JobSettings.Name = filename;

            SJobMsg msg = new SJobMsg();
            msg.job= FdGolbals.JobSettings.Get();  
            FdGolbals.FdInterface.SendMsg(GuiMsg.SAVE_JOB, ref msg);
        }

        //--- SaveNew ---------------------------------------
        public void SaveNew(string filename)
        {
            string dir=Path.GetDirectoryName(FdGolbals.Job.Dir)+Path.DirectorySeparatorChar+filename;
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            FdGolbals.JobSettings.Name = filename;

            SJob    job = FdGolbals.JobSettings.Get();
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
            if (filename==FdGolbals.JobSettings.Name)
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

        //--- Property Dir ---------------------------------------
        private string _Dir;
        public string Dir
        {
            get { 
                    PingReply pr = new Ping().Send(FdGolbals.Settings.IpAddress, 100);
                    if (pr.Status==IPStatus.Success)
                    { 
                        string dir=FdGolbals.Dir.Jobs+FdGolbals.JobSettings.Name;
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
            if (FdGolbals.JobSettings.Name==null)
            {
                FdGolbals.Events.AddError(0, "No job loaded!");
                return;
            }
            FdGolbals.FdInterface.SendCommand(GuiMsg.RUN_JOB);
        }

        //--- Stop ----------------------------------
        public void Stop()
        {
            PrintingSpeed = 0;
            FdGolbals.FdInterface.SendCommand(GuiMsg.STOP_JOB);
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
    }
}
