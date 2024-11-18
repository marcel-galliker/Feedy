using Feedy.Models;
using Feedy.Models.Enums;
using System.Windows.Controls;

namespace Feedy.Views.PickPlaceView
{
    /// <summary>
    /// Interaction logic for PlaceSettingsView.xaml
    /// </summary>
    public partial class PlaceSettingsView : UserControl
    {
        public PlaceSettingsView()
        {
            InitializeComponent();

            CB_PlaceMode.ItemsSource     = new EN_PlaceMode();

            DataContext = FdGolbals.JobSettings.PickPlace.PlaceSettings;
        }
    }
}
