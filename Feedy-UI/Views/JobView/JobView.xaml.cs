using Feedy.Converters;
using Feedy.Models;
using Feedy.Models.Enums;
using GE_Utilities;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Feedy.Views.JobView
{

    //--- Filename_Converter --------------------------------------
    public class Filename_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Path.GetFileName(value as String);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //--- Folder_Converter ----------------------------------------
    public class Folder_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Path.GetDirectoryName(value as String);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for JobView.xaml
    /// </summary>
    public partial class JobView : UserControl
    {
        //--- JobView ------------------------------------------------------------
        public JobView()
        {
            InitializeComponent();

            DataContext                     = FdGolbals.JobSettings;
            FdGolbals.Language.LanguageChanged += _Language;
            FdGolbals.User.UserChanged      += _UserChanged;

            _SetVisibility();
            _Language();
            _UserChanged();
        }
        
        //--- _Language ----------------------------------------------
        private void _Language()
        {
            TB_Job.Text         = FdGolbals.Language.GetText("TB_Job");
        }

        //--- _UserChanged ------------------------------------
        private void _UserChanged()
        {
            this.IsEnabled = FdGolbals.User.Enabled;
        }

        //--- UserControl_Loaded -----------------------------------------
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GeBindable.InvokeDelayed(1000, ()=>FdGolbals.Job.LoadLastJob());
        }

        //--- _SetVisibility -------------------------------------------------
        private void _SetVisibility()
        {
        }

        private void GeSeparator_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
