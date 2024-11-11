using Feedy.Models;
using System.Windows.Controls;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
            DataContext = FdGolbals.Job;
        }
    }
}
