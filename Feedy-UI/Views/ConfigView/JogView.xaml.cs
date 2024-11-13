using Feedy.Models;
using GE_Utilities;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Feedy.Views.ConfigView
{
    /// <summary>
    /// Interaction logic for JogView.xaml
    /// </summary>
    public partial class JogView : UserControl
    {
        public JogView()
        {
            InitializeComponent();
           
            DataContext = FdGolbals.JogInfo;
        }

        //--- Jog_Clicked ------------------------------------------------
        private void Jog_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int row = Grid.GetRow(btn);
            int col = Grid.GetColumn(btn);
            if (col==0) FdGolbals.JogInfo.Pos[row-1]--;
            else        FdGolbals.JogInfo.Pos[row-1]++;
        }

        //--- Jog_Confirm ------------------------------------------------
        private void Jog_Confirm(object sender, System.Windows.RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int row = Grid.GetRow(btn);
            int col = Grid.GetColumn(btn);
            if (col==0) FdGolbals.JogInfo.Pos[row-1]--;
            else        FdGolbals.JogInfo.Pos[row-1]++;
        }

        //--- StartJog -------------------------------
        private bool _running=false;
        private void StartJog(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int row = Grid.GetRow(btn);
            int col = Grid.GetColumn(btn);
            int step = (col==0)? -1 : 1;
            _running = true;
            Thread t = new Thread(()=>
            {
                while (_running)
                {
                    Thread.Sleep(100);
                    GeBindable.Invoke(()=>FdGolbals.JogInfo.Pos[row-1]+=step);
                }
            });
            t.Start();
        }

        //--- StopJog -------------------------------------
        private void StopJog(object sender, RoutedEventArgs e)
        {
            _running = false;
        }
    }
}
