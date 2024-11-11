using System;
using System.Windows.Data;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Feedy.Converters
{
    public class Filetime2String_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FILETIME ft = (FILETIME)value;
            Int64 ft64 = ft.dwHighDateTime<<32 | ft.dwLowDateTime; 
            if (ft64==0) return null;
            DateTime dt = new DateTime(ft64);
            return dt.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
