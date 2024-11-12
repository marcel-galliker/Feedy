using Feedy.Models;
using Feedy.Models.Enums;
using System.Windows.Controls;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class CamSetView : UserControl
    {
        public CamSetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;

            DataContext = FdGolbals.JobSettings.Cam;

            CB_TriggerMode.ItemsSource  = new EN_CamTrigger();
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

    }
}
