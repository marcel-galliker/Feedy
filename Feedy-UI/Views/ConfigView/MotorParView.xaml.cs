using Feedy.Models;
using Feedy.Services;
using GE_Utilities;
using System.Windows.Controls;
using System.Windows.Data;

namespace Feedy.Views.ConfigView
{
    /// <summary>
    /// Interaction logic for MotorParView.xaml
    /// </summary>
    public partial class MotorParView : UserControl
    {
        int _MotorCnt;

        public MotorParView()
        {
            InitializeComponent();

          //  _MotorCnt=FdGolbals.MotorSettings.Length;
            _create_controls();
        }

        //--- _create_controls ----------------------------
        private void _create_controls() 
        {
            _MotorCnt = feedy_def.MotorCnt;

            string[] propname={"PowerHold", "PowerMove", "Acc", "Speed", "", "TestPosStart", "TestPosEnd", "TestSpeed"};

            for (int m=0; m<_MotorCnt; m++)
            {
                for (int p=0; p<propname.Length; p++)
                {
                    if (propname[p].Length>1)
                    {
                        GeNumBox box = new GeNumBox();
                        Grid.SetColumn(box, 1+p);
                        Grid.SetRow(box, 1+m);
                        box.DataContext = FdGolbals.Cfg.Motor[m];
                        Binding binding = new Binding(propname[p]);
                        box.SetBinding(TextBox.TextProperty, binding);
                        Grid_par.Children.Add(box);
                    }
                }
            }
        }
    }
}
