using Feedy.Services;
using GE_Utilities;
using System.Collections.ObjectModel;

namespace Feedy.Models
{
    public class PickPlace : GeBindable
    {
        public PickPlace()
        {
			if (_Place==null)
			{
				_Place = new ObservableCollection<Position>();
                for (int i = 0; i < feedy_def.PlacePosCnt; i++) 
                    _Place.Add(new Position(i,i,i,i));
			}
        }

        //--- Property Park ---------------------------------------
        private Position _Park = new Position(1,1,1,1);
		public Position Park	
		{
			get { return _Park; }
			set { SetProperty(ref _Park, value); }
		}

		//--- Property Pick ---------------------------------------
		private Position _Pick = new Position(2,2,2,2);
		public Position Pick
		{
			get { return _Pick; }
			set { SetProperty(ref _Pick, value); }
		}

		//--- Property Test ---------------------------------------
		private Position _Test = new Position(3,3,3,3);
		public Position Test
		{
			get { return _Test; }
			set { SetProperty(ref _Test, value); }
		}

		//--- Property Place ---------------------------------------
		private ObservableCollection<Position> _Place;
		public ObservableCollection<Position> Place
		{
			get { return _Place; }
			set { SetProperty(ref _Place, value); }
		}
	}
}
