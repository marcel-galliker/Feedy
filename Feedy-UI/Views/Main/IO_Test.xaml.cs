using Feedy.Models;
using GE_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for IO_Test.xaml
    /// </summary>
    public partial class IO_Test : Popup
    {
        static string[] _InputText =
        {
            "Product Detect",
            "Input 2",
            "Input 3",
            "Input 4",
        };
        static string[] _OutputText =
        {
            "Running",
            "SORT",
            "UV-Lamp",
            "Camera",
        };

        static Ellipse[] _Input  = new Ellipse[4];
        static Ellipse[] _Output  = new Ellipse[4];
        static GeButton[] _OutputBtn = new GeButton[4];

        //--- constructor -----------------------------------------
        public IO_Test()
        {
            InitializeComponent();

       //     FdGolbals.HyperBoard.PropertyChanged += HyperBoard_PropertyChanged;
        }

        //--- HyperBoard_PropertyChanged -----------------
        private void HyperBoard_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("Input")) _update_io();
        }

        //--- _InitControls -------------------------------
        private void _InitControls()
        {            
            Brush brush=(Brush)FindResource("DataGrid.Brush.AlternateRow");
            int row=2;
            for (int i=0; i<4; i++)
            {
                if ((i&1)==1)
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle() { Fill=brush};
                    Grid.SetRow(rect, row+i);
                    Grid.SetColumnSpan(rect, 3);
                    IO_Grid.Children.Add(rect);
                }

                TextBlock text=new TextBlock(){Text="I"+(i+1).ToString(), VerticalAlignment=VerticalAlignment.Center };
                Grid.SetColumn(text, 0);
                Grid.SetRow(text, row+i);
                IO_Grid.Children.Add(text);

                TextBlock descr =new TextBlock(){ Text = _InputText[i], VerticalAlignment=VerticalAlignment.Center };
                Grid.SetColumn(descr, 1);
                Grid.SetRow(descr, row+i);
                IO_Grid.Children.Add(descr);

                _Input[i] = new Ellipse(){ Width=25, Height=25, Fill=Brushes.Transparent, Stroke=Brushes.DarkGray};
                Grid.SetColumn(_Input[i], 2);
                Grid.SetRow(_Input[i], row+i);
                IO_Grid.Children.Add(_Input[i]);
            }

            row=8;
            for (int i=0; i<4; i++)
            {
                if ((i&1)==1)
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle() { Fill=brush};
                    Grid.SetRow(rect, row+i);
                    Grid.SetColumnSpan(rect, 3);
                    IO_Grid.Children.Add(rect);
                }

                TextBlock text=new TextBlock(){Text="O"+(i+1).ToString(), VerticalAlignment=VerticalAlignment.Center };
                Grid.SetColumn(text, 0);
                Grid.SetRow(text, row+i);
                IO_Grid.Children.Add(text);

                TextBlock descr =new TextBlock(){ Text = _OutputText[i], VerticalAlignment=VerticalAlignment.Center };
                Grid.SetColumn(descr, 1);
                Grid.SetRow(descr, row+i);
                IO_Grid.Children.Add(descr);

                _OutputBtn[i] = new GeButton(){ Width=25, Height=25,Confirm = Output_Clicked, Background=Brushes.Transparent};
                Grid.SetColumn(_OutputBtn[i], 2);
                Grid.SetRow(_OutputBtn[i], row+i);
                IO_Grid.Children.Add(_OutputBtn[i]);
            }
            _update_io();
        }

        //--- _update_io ---------------------------------------
        private void _update_io()
        {
            /*
            for (int i=0; i<4; i++)
            {
                if (_OutputBtn[i]!=null) _OutputBtn[i].IsChecked = ((FdGolbals.HyperBoard.Output & (1<<i))!=0);
                if ( _Input[i]!=null)    _Input[i].Fill          = ((FdGolbals.HyperBoard.Input  & (1<<i))==0)? Brushes.White : Brushes.Yellow;
            }
            */
        }

        //--- Output_Clicked ------------------
        private void Output_Clicked(object sender, RoutedEventArgs e)
        {
            GeButton btn = sender as GeButton;
            btn.IsChecked = !btn.IsChecked;
            int data=0;
            for (int i=0; i<4; i++)
            {
                if (_OutputBtn[i].IsChecked) data |=  (1<<i);
            }
        //    FdGolbals.HyperBoard.OutputTest = data;
        }

        //--- Popup_Opened --------------------------------------
        private void Popup_Opened(object sender, EventArgs e)
        {
            if (_OutputBtn[0]==null)
                _InitControls();
            _update_io();
        }

        //--- Popup_Closed --------------------------------------
        private void Popup_Closed(object sender, EventArgs e)
        {

        }
    }
}
