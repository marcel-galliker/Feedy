using Feedy.Models;
using Feedy.Services;
using System.Windows.Controls;

namespace Feedy.Views.EventsView
{

    /// <summary>
    /// Interaction logic for EventsView.xaml
    /// </summary>
    public partial class EventsView : UserControl
    {
        public EventsView()
        {
            InitializeComponent();
            DataContext = FdGolbals.Events;
            EventsGrid.ItemsSource = FdGolbals.Events.List;
        //  TB_Count.DataContext   = FdGolbals.Events.List; // for tests
            FdGolbals.Language.LanguageChanged   += _Language;
            _Language();
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
            FdGolbals.Language.SetText(TB_Events);
            Col_Time.Header = FdGolbals.Language.GetText("Col_Time");
            Col_Error.Header = FdGolbals.Language.GetText("Col_Error");
            Col_Message.Header = FdGolbals.Language.GetText("Col_Message");
            Col_Info.Header = FdGolbals.Language.GetText("Col_Info");
        }

        private void Confirm_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            FdGolbals.Events.Confirm();
           
            FdGolbals.FdInterface.SendCommand(EzGuiMsg.ERROR_RESET);
        }
    }
}
