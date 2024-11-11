using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GE_Utilities
{
    //--- Folder_Converter ----------------------------------------
    public class TextVisibility_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value==null || (string)value=="") return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class GeSeparator : UserControl
    {
        //--- constructor ------------------------------------
        public GeSeparator()
        {
            InitializeComponent();
            DataContext = this;
        }

        //--- Property Icon ---------------------------------------
        public PackIconMaterialKind Icon
        {
            get 
            { 
                return (PackIconMaterialKind)GetValue(IconProperty); 
            }
            set 
            { 
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(PackIconMaterialKind), typeof(GeSeparator));

        //--- Property Text ---------------------------------------
        public string Text
        {
            get 
            { 
                return (string)GetValue(TextProperty); 
            }
            set 
            { 
                SetValue(TextProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GeSeparator));

    }
}
