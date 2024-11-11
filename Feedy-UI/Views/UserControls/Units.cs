using Feedy.Models;
using System.Windows.Controls;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Units.xaml
    /// </summary>
    public partial class Units : TextBlock
    {
        public Units()
        {
            FdGolbals.Settings.PropertyChanged += Settings_PropertyChanged;
            VerticalAlignment = System.Windows.VerticalAlignment.Center;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            FontSize = (double)FindResource("FontSizeSmall");
            _setText();
        }

        //--- Settings_PropertyChanged -------------------------------
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="Units") _setText();
        }

        //--- _setText ------------------------------------
        private void _setText()
        {
            switch(FdGolbals.Settings.Units)
            {
                case Settings.EUnits.inches: Text="inch"; break;
                default:                     Text="mm";   break;
            }
        }
    }
}
