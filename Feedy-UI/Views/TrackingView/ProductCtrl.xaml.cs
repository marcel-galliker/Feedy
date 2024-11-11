using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EZ_Editor.Views.TrackingView
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ProductCtrl : UserControl
    {
        private static Brush Brush_BorderSelected;
        private static Brush Brush_GridSelected;
        private static Brush Brush_Border;

        public ProductCtrl()
        {
            InitializeComponent();
            Brush_BorderSelected = (Brush)FindResource("DataGrid.Brush.Selected.Background");
            Brush_GridSelected   = (Brush)FindResource("DataGrid.Brush.Selected.BackgroundLight");
            Brush_Border         = (Brush)FindResource("EzGroupBox.BorderBrush");

            Visibility = Visibility.Collapsed;
        }

        //--- Property IsChecked ---------------------------------------
        public bool IsChecked
        {
            set { 
                    if (value) 
                    {
                        Box.BorderBrush = Brush_BorderSelected;
                        Box.Background  = Brush_GridSelected;
                        Grid.Background = Brush_GridSelected;
                    }
                    else 
                    {
                        Box.BorderBrush = Brush_Border;
                        Grid.Background = Brushes.White;
                    }   
                }
        }

        //--- id changed ---------------------------
        private void TextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            TextBlock txt = sender as TextBlock;
            string text = txt.Text;
            if (text=="" || text.StartsWith("-")) this.Visibility = Visibility.Collapsed;
            else this.Visibility = Visibility.Visible;
        }
    }
}
