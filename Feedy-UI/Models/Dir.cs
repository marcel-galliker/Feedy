using GE_Utilities;
using System.IO;

namespace Feedy.Models
{
    public class Dir : GeBindable
    {
		//--- Property Base ---------------------------------------
		public static string _Base = "D:"+Path.DirectorySeparatorChar+"Feedy"+Path.DirectorySeparatorChar;
		public string Base
		{
			get { return _Base; }
			set { SetProperty(ref _Base, value); }
		}

		//--- Property Jobs ---------------------------------------
		public string Jobs
		{
			get { return Base+"Jobs"+Path.DirectorySeparatorChar; }
		}

		//--- Property Logs ---------------------------------------
		public string Logs
		{
			get { return Base+"Logs"+Path.DirectorySeparatorChar; }
		}


    }
}
