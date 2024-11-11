using Feedy.Services;
using GE_Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feedy.Models
{
    public class CVersion : GeBindable
    {
        //--- Update -------------------------------
        public void Update(string msg)
        {
            string[] list=msg.Split('\n');
            if (list.Length>=4)
            {
                Date    = list[0];
                Version = list[1];
                Branch  = list[2];
                Commit  = list[3];
            }
        }

        //--- Property Version ---------------------------------------
        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { SetProperty(ref _Version, value); }
        }

        //--- Property Date ---------------------------------------
        private string _Date="Date";
        public string Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }

        //--- Property Branch ---------------------------------------
        private string _Branch="Branch";
        public string Branch
        {
            get { return _Branch; }
            set { SetProperty(ref _Branch, value); }
        }

        //--- Property Commit ---------------------------------------
        private string _Commit="Commit";
        public string Commit
        {
            get { return _Commit; }
            set { SetProperty(ref _Commit, value); }
        }

    }
}
