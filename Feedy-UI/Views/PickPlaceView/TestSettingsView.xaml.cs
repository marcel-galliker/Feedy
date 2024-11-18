using Feedy.Models;
using Feedy.Models.Enums;
using System.Windows.Controls;

namespace Feedy.Views.PickPlaceView
{
    /// <summary>
    /// Interaction logic for TestSettingsView.xaml
    /// </summary>
    public partial class TestSettingsView : UserControl
    {
        public TestSettingsView()
        {
            InitializeComponent();

            DataContext = FdGolbals.JobSettings.PickPlace.TestSettings;
            CB_TestMode.ItemsSource      = new EN_TestMode();
        }
    }
}
