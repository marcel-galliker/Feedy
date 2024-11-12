using Feedy.Models;
using Feedy.Models.Enums;
using System.Windows.Controls;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class GripperSetView : UserControl
    {
        public GripperSetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;

            DataContext = FdGolbals.JobSettings.Gripper;
        
            CB_GripperType.ItemsSource      = new EN_GripperType();
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

    }
}
