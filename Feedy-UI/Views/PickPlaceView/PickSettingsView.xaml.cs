using Feedy.Models;
using System.Windows.Controls;

namespace Feedy.Views.PickPlaceView
{
    /// <summary>
    /// Interaction logic for PickSettingsView.xaml
    /// </summary>
    public partial class PickSettingsView : UserControl
    {
        public PickSettingsView()
        {
            InitializeComponent();

            DataContext = FdGolbals.JobSettings.PickPlace.PickSettings;
        }
    }
}
