using Feedy.Models;
using Feedy.Services;
using System.Windows;
using System.Windows.Controls;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class TraySetView : UserControl
    {
        public TraySetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;

            DataContext = FdGolbals.JobSettings.Tray;
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

        //--- Test_Clicked ----------------------------------------
        private void Test_Clicked(object sender, RoutedEventArgs e)
        {
            Services.feedy_def.STray set=FdGolbals.JobSettings.Tray.Get();
            FdGolbals.FdInterface.SendMsgData(GuiMsg.TEST_TRAY, ref set);
        }

    }
}
