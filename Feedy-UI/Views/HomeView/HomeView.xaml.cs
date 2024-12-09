using Feedy.Models;
using System.Windows.Controls;

namespace Feedy.Views.HomeView
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            DataContext = FdGolbals.JobSettings;
        }
    }
}
