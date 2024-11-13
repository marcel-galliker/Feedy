using System;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class FdArgs
    {
        public FdArgs()
        {
            _parse_command_line();
        }

        //--- _parse_command_line ------------------------------
        private void _parse_command_line()
        {
            int i;
            string[] args = Environment.GetCommandLineArgs();
            for (i=1; i<args.Length; i++) 
            {
                if (args[i].ToLower().Equals("-ipaddr"))
                {
                    i++;
                    if (i<args.Length) IpAddr = args[i];
                }
            }
        }

        //--- Property IpAddr ---------------------------------------
        private string _IpAddr;
        public string IpAddr
        {
            get { return _IpAddr; }
            set { _IpAddr = value; }
        }
    }
}
