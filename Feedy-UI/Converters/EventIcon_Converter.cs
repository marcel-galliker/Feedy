using Feedy.Models;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Feedy.Converters
{
    //--- EventIconType_Converter ----------------------------------------
    public class EventIconType_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ENEventType)
            {
                switch((ENEventType)value)
                {
                    case ENEventType.err:  return MahApps.Metro.IconPacks.PackIconMaterialKind.AlertCircle;
                    case ENEventType.warn: return MahApps.Metro.IconPacks.PackIconMaterialKind.AlertCircleOutline;
                    default:               return MahApps.Metro.IconPacks.PackIconMaterialKind.None;
                }
            }
            return MahApps.Metro.IconPacks.PackIconMaterialKind.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //--- EventIconColor_Converter ----------------------------------------
    public class EventIconColor_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ENEventType)
            {
                switch((ENEventType)value)
                {
                    case ENEventType.warn: return Brushes.Orange;
                    case ENEventType.err:  return Brushes.Red;
                    default: break;
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
