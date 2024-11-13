using Feedy.Models;
using Feedy.Models.Enums;
using Feedy.Services;
using System.Windows;
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

        //--- Test_Clicked ----------------------------------------
        private void Test_Clicked(object sender, RoutedEventArgs e)
        {
            Services.feedy_def.SFeeder set=FdGolbals.JobSettings.Feeder.Get();
            FdGolbals.FdInterface.SendMsgData(GuiMsg.TEST_FEEDER, ref set);
        }

    }
}
