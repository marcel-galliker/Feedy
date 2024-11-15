using Feedy.Models;
using Feedy.Views.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Feedy.Views.PickPlaceView
{
    /// <summary>
    /// Interaction logic for PickPlaceView.xaml
    /// </summary>
    public partial class PickPlaceView : UserControl
    {
        private PositionCtrl[] _PosCtrl = new PositionCtrl[3+FdGolbals.JobSettings.PickPlace.Place.Count];
        private PositionCtrl _SelectedPosCtrl;

        //--- constructor ---------------------------------------
        public PickPlaceView()
        {
            InitializeComponent();

            JogPanel.Visibility = Visibility.Collapsed;
            Jog.PosChanged = MotorPosChanged;
            _CreateControls();
        }

        //--- _CreateControls ---------------------------------
        private void _CreateControls() 
        {
            for (int i=0; i<3; i++)
            {
                PositionCtrl pos=new PositionCtrl();
                pos.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(pos, i);
                Grid.SetColumn(pos, 1);
                switch(i)
                {
                    case 0: pos.DataContext = FdGolbals.JobSettings.PickPlace.Park; pos.Title="Park"; break;
                    case 1: pos.DataContext = FdGolbals.JobSettings.PickPlace.Pick; pos.Title="Pick"; break;
                    case 2: pos.DataContext = FdGolbals.JobSettings.PickPlace.Test; pos.Title="Test"; break;
                }
                _PosCtrl[i]=pos;
                MainGrid.Children.Add(pos);
            }

            for (int i=0; i<FdGolbals.JobSettings.PickPlace.Place.Count; i++)
            {
                PositionCtrl pos=new PositionCtrl();
                pos.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                pos.Title=string.Format("{0}", i+1);
                Grid.SetRow(pos, 3);
                Grid.SetColumn(pos, 1+i);
                pos.DataContext = FdGolbals.JobSettings.PickPlace.Place[i];
                MainGrid.Children.Add(pos);
                _PosCtrl[3+i] = pos;
            }
        }

        //--- MotorPosChanged --------------------
        Brush _freground;
        private void MotorPosChanged()
        {
            if (_freground==null) _freground = CB_Save.Foreground;
            Icon_Save.Foreground = (Brush)FindResource("DataGrid.Brush.Selected.Background");
            CB_Save.Background = Brushes.White;
        }

        //--- Save_Clicked ---------------------------------
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            if (_SelectedPosCtrl!=null)
            {
                Position ctrl = _SelectedPosCtrl.DataContext as Position;
                ctrl.Set(FdGolbals.FeedyStatus.MotorPos[0], FdGolbals.FeedyStatus.MotorPos[1], FdGolbals.FeedyStatus.MotorPos[2], FdGolbals.FeedyStatus.MotorPos[3]);
            }
            Icon_Save.Foreground = _freground;
            CB_Save.Background   = (Brush)FindResource("Button.Static.Background");
        }

        //--- _PtInCtrl -----------------
        private bool _PtInCtrl(Point pt, Control ctrl)
        {
            Point client=MainGrid.PointFromScreen(new Point(0,0));
            client.X = -client.X;
            client.Y = -client.Y;
            Point test=ctrl.PointFromScreen(client);
            double x0=-test.X;
            double x1=x0+ctrl.ActualWidth;
            double y0=-test.Y;
            double y1=y0+ctrl.ActualHeight;
            return (x0<=pt.X && pt.X<=x1 && y0<=pt.Y && pt.Y<=y1);
        }

        //--- UserControl_PreviewMouseDown ------------------------------------
        private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.MouseDevice.GetPosition(MainGrid);
            for (int i=0; i<_PosCtrl.Length; i++) 
            {
                if (_PtInCtrl(pt, _PosCtrl[i]))
                {
                    _PosCtrl[i].IsSelected = true;
                    JogPanel.Visibility = Visibility.Visible;
                    _SelectedPosCtrl = _PosCtrl[i];
                    break;
                }
            }
        }
    }
}
