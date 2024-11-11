using System.Windows.Media.Imaging;

namespace GE_Utilities
{
    public class GeEnum<type> : GeBindable
    {
        public GeEnum()
        {
        }

        public GeEnum(type value, string display)
        {
            Value = value;
            Display = display;
        }

        public GeEnum(type value, string display, BitmapImage image)
        {
            Value = value;
            Display = display;
            Image = image;
        }

        private type _Value;
        public type Value
        {
            get { return _Value; }
            set { SetProperty<type>(ref _Value, value); }
        }

        private string _Display;
        public string Display
        {
            get { return _Display; }
            set { SetProperty<string>(ref _Display, value); }
        }

        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set { SetProperty<BitmapImage>(ref _Image, value); }
        }
    }
}
