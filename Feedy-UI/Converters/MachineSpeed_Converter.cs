using Feedy.Models;
using System;
using System.Windows.Data;

namespace Feedy.Converters
{
    public class MachineSpeed_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value!=null)
            {
                if (targetType==typeof(string)) 
                {
                    double val=(double)value;
                    if (FdGolbals.Settings.Units==Settings.EUnits.inches) val *= 3.28084;
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
