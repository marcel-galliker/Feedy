using Feedy.Models;
using Feedy.Models.Enums;
using Feedy.Services;
using Feedy.Views.UserControls;
using NUnit.Framework.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static Feedy.Services.feedy_def;

namespace Feedy.Views.PickPlaceView
{
    /// <summary>
    /// Interaction logic for PickPlaceView.xaml
    /// </summary>
    public partial class PickPlaceView : UserControl
    {
        private PositionCtrl[] _PosCtrl = new PositionCtrl[3+FdGolbals.JobSettings.PickPlace.PlacePos.Count];
        private PositionCtrl _SelectedPosCtrl;

        //--- constructor ---------------------------------------
        public PickPlaceView()
        {
            InitializeComponent();

            DataContext = this;

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
                Grid.SetColumn(pos, 2);
                switch(i)
                {
                    case 0: pos.DataContext = FdGolbals.JobSettings.PickPlace.ParkPos; pos.Title="Park"; break;
                    case 1: pos.DataContext = FdGolbals.JobSettings.PickPlace.PickPos; pos.Title="Pick"; break;
                    case 2: pos.DataContext = FdGolbals.JobSettings.PickPlace.TestPos; pos.Title="Test"; break;
                }
                _PosCtrl[i]=pos;
                MainGrid.Children.Add(pos);
            }

            for (int i=0; i<FdGolbals.JobSettings.PickPlace.PlacePos.Count; i++)
            {
                PositionCtrl pos=new PositionCtrl();
                pos.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                pos.Title=string.Format("{0}", i+1);
                Grid.SetRow(pos, 3);
                Grid.SetColumn(pos, 2+i);
                pos.DataContext = FdGolbals.JobSettings.PickPlace.PlacePos[i];
                MainGrid.Children.Add(pos);
                _PosCtrl[3+i] = pos;
            }
        }

        //--- MotorPosChanged --------------------
        private void MotorPosChanged()
        {
            Icon_Save.Foreground = (Brush)FindResource("DataGrid.Brush.Selected.Background");
            CB_Save.Background = Brushes.White;
        }

        //--- Move_Clicked ---------------------------------
        private void Move_Clicked(object sender, RoutedEventArgs e)
        {
            if (_SelectedPosCtrl!=null)
            {
                Position ctrl = _SelectedPosCtrl.DataContext as Position;
                SPosition pos = ctrl.Get();
                FdGolbals.FdInterface.SendMsgData(GuiMsg.MOTOR_MOVE2POS, ref pos);
            }
        }

        //--- Save_Clicked ---------------------------------
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            if (_SelectedPosCtrl!=null)
            {
                Position ctrl = _SelectedPosCtrl.DataContext as Position;
                ctrl.Set(FdGolbals.FeedyStatus.MotorPos[0], FdGolbals.FeedyStatus.MotorPos[1], FdGolbals.FeedyStatus.MotorPos[2], FdGolbals.FeedyStatus.MotorPos[3]);
            }
            Icon_Save.Foreground = Brushes.Black;
            CB_Save.Background   = Brushes.White; //(Brush)FindResource("Button.Static.Background");
        }

        //--- Gripper_Open ------------------------------------------
        private void Gripper_Open(object sender, System.Windows.RoutedEventArgs e)
        {
            FdGolbals.FdInterface.SendCommand(GuiMsg.OPEN_GRIPPER);
        }

        //--- Gripper_Close --------------------------------------------
        private void Gripper_Close(object sender, System.Windows.RoutedEventArgs e)
        {
            FdGolbals.FdInterface.SendCommand(GuiMsg.CLOSE_GRIPPER);
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
