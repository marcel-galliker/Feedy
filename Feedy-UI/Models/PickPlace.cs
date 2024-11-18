using Feedy.Services;
using GE_Utilities;
using System.Collections.ObjectModel;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class PickPlace : GeBindable
    {
        public PickPlace()
        {
			if (_PlacePos==null)
			{
				_PlacePos = new ObservableCollection<Position>();
                for (int i = 0; i < feedy_def.PlacePosCnt; i++) 
                    _PlacePos.Add(new Position(i,i,i,i));
			}
        }

		//--- Set ----------------------------------
		public void Set(SPickPlace pp)
		{
			ParkPos.Set(pp.parkPos);
			PickSettings.Set(pp.pickSettings);
			PickPos.Set(pp.pickPos);
			TestSettings.Set(pp.testSettings);
			TestPos.Set(pp.testPos);
			PlaceSettings.Set(pp.placeSettings);
			for (int i = 0; i < feedy_def.PlacePosCnt; i++) PlacePos[i].Set(pp.placePos[i]);
		}

		//--- Get ---------------------------------
		public SPickPlace Get()
		{
			SPickPlace pp = new SPickPlace();
			pp.placePos = new SPosition[feedy_def.PlacePosCnt];
			pp.parkPos = ParkPos.Get();
			pp.pickSettings = PickSettings.Get();
			pp.pickPos = PickPos.Get();
			pp.testSettings = TestSettings.Get();
			pp.testPos = TestPos.Get();
			pp.placeSettings = PlaceSettings.Get();
			for (int i = 0; i < feedy_def.PlacePosCnt; i++) pp.placePos[i] = PlacePos[i].Get();
			return pp;
		}

        //--- Property ParkPos ---------------------------------------
        private Position _ParkPos = new Position(1,1,1,1);
		public Position ParkPos	
		{
			get { return _ParkPos; }
			set { SetProperty(ref _ParkPos, value); }
		}

		//--- Property PickSettings ---------------------------------------
		private PickSettings _PickSettings = new PickSettings();
		public PickSettings PickSettings
		{
			get { return _PickSettings; }
			set { SetProperty(ref _PickSettings, value); }
		}

		//--- Property PickPos ---------------------------------------
		private Position _PickPos = new Position(2,2,2,2);
		public Position PickPos
		{
			get { return _PickPos; }
			set { SetProperty(ref _PickPos, value); }
		}

		//--- Property TestSettings ---------------------------------------
		private TestSettings _TestSettings= new TestSettings();
		public TestSettings TestSettings
		{
			get { return _TestSettings; }
			set { SetProperty(ref _TestSettings, value); }
		}

		//--- Property TestPos ---------------------------------------
		private Position _TestPos = new Position(3,3,3,3);
		public Position TestPos
		{
			get { return _TestPos; }
			set { SetProperty(ref _TestPos, value); }
		}

		//--- Property PlaceSettings ---------------------------------------
		private PlaceSettings _PlaceSettings = new PlaceSettings();
		public PlaceSettings PlaceSettings
		{
			get { return _PlaceSettings; }
			set { SetProperty(ref _PlaceSettings, value); }
		}

		//--- Property PlacePos ---------------------------------------
		private ObservableCollection<Position> _PlacePos;
		public ObservableCollection<Position> PlacePos
		{
			get { return _PlacePos; }
			set { SetProperty(ref _PlacePos, value); }
		}
	}
}
