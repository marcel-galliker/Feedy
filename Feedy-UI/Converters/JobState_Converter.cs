using Feedy.Models;
using Feedy.Services;
using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using static Feedy.Models.FeedyStatus;
using static Feedy.Services.FdDef;

namespace Feedy.Converters
{
    public class JobState_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.Name=="Boolean")
            {
                bool running=false;
                EnJobState st=(EnJobState)value;
                switch((EnJobState)value)
                {
                    case EnJobState.JS_starting: running=true; break;
                    case EnJobState.JS_printing: running=true; break;
                    case EnJobState.JS_stopping: running=true; break;
                    default: running=false; break;
                }
                if (parameter==null) 
                    return running;
                else return !running;
            }
            else if (targetType.Name=="Visibility")
            {
                switch((EnJobState)value)
                {
                    case EnJobState.JS_starting:    return Visibility.Visible;
                    case EnJobState.JS_printing:    return Visibility.Visible;
                    case EnJobState.JS_stopping:    return Visibility.Visible;
                    default: return Visibility.Collapsed;
                }
            }
            else if (targetType.Name=="PackIconMaterialKind")
            {
                switch((EnJobState)value)
                {
                    case EnJobState.JS_stopping:    return PackIconMaterialKind.Stop;
                    case EnJobState.JS_printing:    return PackIconMaterialKind.Stop;
                    default:                        return PackIconMaterialKind.Play;
                }
            }
            else if (targetType.Name=="Brush")
            {
                switch((EnJobState)value)
                {
                    case EnJobState.JS_printing: return Brushes.Red;
                    case EnJobState.JS_stopping: return Brushes.Red;
                    default: return Brushes.Green;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class JobStateBusy_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.Name=="Boolean")
            {
                switch((EnJobState)value)
                {
                    case EnJobState.JS_starting: return true;
                    case EnJobState.JS_stopping: return true;
                    default: return false;
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
