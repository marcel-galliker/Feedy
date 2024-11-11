﻿using Feedy.Models;
using System;
using System.Windows.Data;

namespace Feedy.Converters
{
    public class ValveStatus_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FdGolbals.Language.GetText(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
