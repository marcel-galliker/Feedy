using System;
using System.Windows.Data;
using System.Windows.Media;
using Feedy.Models;
using Feedy.Models.Enums;

namespace Feedy.Converters
{
    public class Progress_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /*
            if (FdGolbals.Job.RecordCnt!=0)
            {
                return string.Format("{0}%", (int)(100*(int)value/FdGolbals.Job.RecordCnt));
            }
            */
            return string.Format("{0}%", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
