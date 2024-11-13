using GE_Utilities;
using System.Collections.ObjectModel;

namespace Feedy.Models
{
    public class JogInfo : GeBindable
    {
		//--- constructor ----------------------------------
        public JogInfo()
        {
			Pos = new ObservableCollection<double>();
            for (int i=0; i<FdGolbals.MotorSettings.Motors.Length; i++) 
			{
				Pos.Add(i);
			}
        }

        //--- Property Speed ---------------------------------------
        private int _Speed = 25;
		public int Speed
		{
			get { return _Speed; }
			set { SetProperty(ref _Speed, value); }
		}

		//--- Property Pos ---------------------------------------
		private ObservableCollection<double> _Pos;
		public ObservableCollection<double> Pos
		{
			get { return _Pos; }
			set { SetProperty(ref _Pos, value); }
		}

	}
}
