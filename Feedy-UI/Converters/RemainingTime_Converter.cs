using System;
using System.Windows.Data;
using System.Windows.Media;
using Feedy.Models;
using Feedy.Models.Enums;

namespace Feedy.Converters
{
    public class RemainingTime_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int sec=(int)value;
            int hr=sec/3600;
            sec -= 3600*hr;
            int min=sec/60;
            sec-= 60*min;
            if (hr>0)       return string.Format("{0} h {1} min", hr, min, sec);
            else if (min>0) return string.Format("{0} min {1} sec", min, sec);
            else if (sec>0) return string.Format("{0} sec", sec);
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
