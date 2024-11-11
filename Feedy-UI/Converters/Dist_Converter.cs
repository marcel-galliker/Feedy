using Feedy.Models;
using GE_Utilities;
using System;
using System.Windows.Data;

namespace Feedy.Converters
{
    public class Dist_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (FdGolbals.Settings.Units == Settings.EUnits.inches) return (double)value/25.4;
                else return value;
            }
            catch(Exception)
            { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (FdGolbals.Settings.Units == Settings.EUnits.inches) return Ge.StrToDouble(value as string)*25.4;
                else return value;
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.ToString());
                return null; 
            }
        }
    }
}
