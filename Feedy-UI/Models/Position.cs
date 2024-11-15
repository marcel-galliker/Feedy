using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class Position : GeBindable
    {
        public Position()
        {
        }

        public Position(double x, double cy, double z, double c)
		{
			Set(x, cy, z, c);
		}

		//--- Set ----------------------------------------
        public void Set(double x, double cy, double z, double c)
		{
			X  = x;
			CY = cy;
			Z  = z;
			C  = c;
		}

		//--- Set ----------------------------
		public void Set(SPosition pos)
		{
			X  = pos.x;
			CY = pos.cy;
			Z  = pos.z;
			C  = pos.c;
		}

		//--- Get ----------------------------------
		public SPosition Get()
		{
			SPosition pos=new SPosition();
			pos.z  = (int)Z;
			pos.x  = (int)X;
			pos.cy = (int)CY;
			pos.c  = (int)C;

			return pos;
		}

		//--- Property Z ---------------------------------------
		private double _Z;
		public double Z
		{
			get { return _Z; }
			set { SetProperty(ref _Z, value); }
		}

		//--- Property X ---------------------------------------
		private double _X;
		public double X
		{
			get { return _X; }
			set { SetProperty(ref _X, value); }
		}

		//--- Property CY ---------------------------------------
		private double _CY;
		public double CY
		{
			get { return _CY; }
			set { SetProperty(ref _CY, value); }
		}

		//--- Property C ---------------------------------------
		private double _C;
		public double C
		{
			get { return _C; }
			set { SetProperty(ref _C, value); }
		}

	}
}
