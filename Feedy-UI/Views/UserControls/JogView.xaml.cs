using Feedy.Models;
using Feedy.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using static Feedy.Services.feedy_def;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for JogView.xaml
    /// </summary>
    public partial class JogView : UserControl
    {
        public Action PosChanged=null;

        public JogView()
        {
            InitializeComponent();
           
            DataContext = FdGolbals.FeedyStatus;
        }

        //--- _getMotorTest -------------------------------
        private SMotorTest _getMotorTest(Button btn)
        {
            int row = Grid.GetRow(btn);
            int col = Grid.GetColumn(btn);
            SMotorTest test = new SMotorTest();
            test.motorNo = row-1;
            test.direction = (col==0)? -1:1;
            test.settings  = FdGolbals.Cfg.Motor[test.motorNo].Get();
            test.testSpeed = FdGolbals.FeedyStatus.TestSpeed;
            return test;
        }

        //--- Jog_Clicked ------------------------------------------------
        private void Jog_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PosChanged!=null) PosChanged();
            SMotorTest test = _getMotorTest(sender as Button);
            FdGolbals.FdInterface.SendMsgData(GuiMsg.STEP_MOTOR, ref test);
        }

        //--- Jog_Confirm ------------------------------------------------
        private void Jog_Confirm(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PosChanged!=null) PosChanged();
            SMotorTest test = _getMotorTest(sender as Button);
            FdGolbals.FdInterface.SendMsgData(GuiMsg.RUN_MOTOR, ref test);
        }

        //--- StartJog -------------------------------
        private void StartJog(object sender, RoutedEventArgs e)
        {
            if (PosChanged!=null) PosChanged();
            SMotorTest test = _getMotorTest(sender as Button);
            test.settings.testPosStart = -1000000;
            test.settings.testPosEnd   =  1000000;
            FdGolbals.FdInterface.SendMsgData(GuiMsg.START_MOTOR, ref test);
        }

        //--- StopJog -------------------------------------
        private void StopJog(object sender, RoutedEventArgs e)
        {
            if (PosChanged!=null) PosChanged();
            SMotorTest test = _getMotorTest(sender as Button);
            FdGolbals.FdInterface.SendMsgData(GuiMsg.STOP_MOTOR, ref test);
        }
    }
}
