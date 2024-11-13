using Feedy.Converters;
using Feedy.Models;
using MahApps.Metro.IconPacks;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using static Feedy.Services.feedy_def;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorState.xaml
    /// </summary>
    public partial class ErrorState : UserControl
    {
        private Brush[] _Color = 
            {
                Brushes.White,
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.Red),
            };

        //--- constructor ---------------------
        public ErrorState()
        {
            InitializeComponent();

            DataContext = FdGolbals.Events;
            FdGolbals.Events.PropertyChanged += Events_PropertyChanged;
            FdGolbals.FeedyStatus.PropertyChanged += PrinterStatus_PropertyChanged;
            _SetEventType();
            _SetMessage();
            Image.Visibility = System.Windows.Visibility.Collapsed;
        }

        //--- PrinterStatus_PropertyChanged -------------------------------------
        private void PrinterStatus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="Ready") 
                _SetEventType();
        }

        //--- Events_PropertyChanged --------------------------------
        private void Events_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Type") _SetEventType();
            else if(e.PropertyName == "Message") _SetMessage();
        }

        //--- Property State ---------------------------------------
        private void _SetEventType()
        {
            if (FdGolbals.Events.Type<ENEventType.warn)
            {
                Icon.Kind = PackIconMaterialKind.None;
                if (FdGolbals.FeedyStatus.JobState>=EnJobState.JS_running)
                    Image.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                var kind  = new EventIconType_Converter().Convert(FdGolbals.Events.Type, null, null, null);
                var color = new EventIconColor_Converter().Convert(FdGolbals.Events.Type, null, null, null);
                Image.Visibility = System.Windows.Visibility.Collapsed;
                if (kind==null) Icon.Kind = PackIconMaterialKind.None;
                else            Icon.Kind = (PackIconMaterialKind)kind;
                Icon.Foreground = color as Brush;
            }    
        }

        //--- _SetMessage ----------------------------
        private void _SetMessage()
        {
            TB_Message.Text = FdGolbals.Events.Message;
        }
    }
}
