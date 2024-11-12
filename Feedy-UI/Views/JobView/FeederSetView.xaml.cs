using Feedy.Models;
using Feedy.Models.Enums;
using System.Windows.Controls;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class FeederSetView : UserControl
    {
        public FeederSetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;

            DataContext = FdGolbals.JobSettings.Feeder;

            CB_Position.ItemsSource         = new EN_Int(1,4);
            CB_Turns.ItemsSource            = new EN_Int(1,4);
            CB_MaxTurns.ItemsSource         = new EN_Int(1,20);
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

    }
}
