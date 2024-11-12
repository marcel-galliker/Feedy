using Feedy.Models;
using System.Windows.Controls;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for JobSet.xaml
    /// </summary>
    public partial class JobSetView : UserControl
    {
        public JobSetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
            TB_Description.Text = FdGolbals.Language.GetText("TB_Description");
        }

    }
}
