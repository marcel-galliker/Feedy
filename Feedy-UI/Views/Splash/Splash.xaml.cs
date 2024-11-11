using Feedy.Models;
using GE_Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Feedy.Views.Splash
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
        //	GeTrace.WriteLine("Splash.Creator start");
            InitializeComponent();
        //	GeTrace.WriteLine("Splash.Creator end");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         //   GeTrace.WriteLine("Splash.Window_Loaded start");
            GeScreen.RemoveSysMenu(this);
            Task.Run(() => 
            { 
                Dispatcher.Invoke(() =>
                {
                    FdGolbals.MainWindow = new MainWindow();
                    FdGolbals.MainWindow.Show();
                    Close();
                });
            });
         //   GeTrace.WriteLine("Splash.Window_Loaded end");
        }
    }
}
