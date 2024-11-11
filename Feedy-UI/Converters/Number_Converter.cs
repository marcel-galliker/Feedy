using System;
using System.Windows.Data;

namespace Feedy.Converters
{
    public class Number_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int n=(int)value;
            int n0=n%1000; n=n/1000;
            int n1=n%1000; n=n/1000;
            int n2=n%1000;
            if (n2>0) return string.Format("{0}'{1:D03}'{2:D03}", n2, n1, n0);
            else if (n1>0) return string.Format("{0}'{1:D03}", n1, n0);
            else return string.Format("{0}", n0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
