using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Feedy.Converters
{
    public class HeaterColor_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((bool)value) return Brushes.Red;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
