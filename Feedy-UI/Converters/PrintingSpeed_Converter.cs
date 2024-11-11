using System;
using System.Windows.Data;

namespace Feedy.Converters
{
    public class PrintingSpeed_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value!=null)
            {
                if (targetType==typeof(double)) return (double)value/1000;
                if (targetType==typeof(string)) 
                {
                    double val=(double)value;
                    if (val<1000) return string.Format("{0:#.#}", value);
                    else return string.Format("{0}'{1:D3}", (int)(val/1000), (int)(val%1000));
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
