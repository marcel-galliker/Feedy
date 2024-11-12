using Feedy.Models;
using Feedy.Models.Enums;
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

    }
}
