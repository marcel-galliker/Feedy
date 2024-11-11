using Feedy.Models;
using Feedy.Models.Enums;
using Feedy.Services;
using GE_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Feedy
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (GeScreen.IsTouch) this.WindowStyle = WindowStyle.None;
            FdGolbals.MainWindow  = this;
            GeBindable.Dispatcher = Dispatcher;
            FdGolbals.FdInterface.PropertyChanged += EzInterface_PropertyChanged;
            _show_connected();
      //      FdGolbals.Job.LoadLastJob();
            _update_Title();
        }

        //--- SetTitle -------------------
        private void _update_Title()
        {
            string title="Feedy";
            if (FdGolbals.FdArgs.IpAddr!=null) 
            {
                title += " " + FdGolbals.FdArgs.IpAddr;
            }
            Title = title;
        }

        //--- EzInterface_PropertyChanged --------------------------------------
        private void EzInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected") _show_connected();
        }

        //--- _show_connected -------------------------------------
        private void _show_connected()
        {
            if (!FdGolbals.FdInterface.Connected)
                TX_NotConnected.Visibility = System.Windows.Visibility.Visible;
            else 
                TX_NotConnected.Visibility = System.Windows.Visibility.Collapsed;
        }
        //--- Property MainView ---------------------------------------
        public UserControl MainView
        {
            get { return _MainView.Child as UserControl;}
            set { _MainView.Child = value;}
        }

        //--- Property ShowBottomRow ---------------------------------------
        private bool _ShowBottomRow=true;
        private GridLength _Row1Height, _Row2Height;
        public bool ShowBottomRow
        {
            get { return _ShowBottomRow; }
            set { 
                    if (_ShowBottomRow!=value)
                    {
                        _ShowBottomRow = value;
                        if (_ShowBottomRow)
                        {
                            Row1.Height = _Row1Height;
                            Row2.Height = _Row2Height;
                        }
                        else
                        {
                            _Row1Height = Row1.Height;
                            _Row2Height = Row2.Height;
                            Row1.Height = Row2.Height = new GridLength(0);
                        }
                    }
                }
        }

        //--- RowExpander_Expanded ------------------------------
        private void RowExpander_Expanded(object sender, RoutedEventArgs e)
        {
            FdGolbals.MainWindow.ShowBottomRow=false;
        }

        //--- RowExpander_Collapsed ---------------------------------------
        private void RowExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            FdGolbals.MainWindow.ShowBottomRow = true;
        }

        //--- Window_Closing ---------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*
            if (FdGolbals.Settings.Demo)
            {
                FdGolbals.Job.Stop();
            }
            */
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        //--- Window_Closed -------------------------------------
        private void Window_Closed(object sender, System.EventArgs e)
        {
            FdGolbals.FdInterface.ShutDown();
        }
    }
}
