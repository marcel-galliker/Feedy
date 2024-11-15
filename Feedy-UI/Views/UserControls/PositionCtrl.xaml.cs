using GE_Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Position.xaml
    /// </summary>
    public partial class PositionCtrl : UserControl
    {
        public static PositionCtrl SelectedCtrl;

        public PositionCtrl()
        {
            InitializeComponent();
        }

        //--- Property Title ---------------------------------------
        public string Title
        {
            set { TX_Title.Text = value; }
        }

        //--- Property isSelected ---------------------------------------
        private bool _isSelected;
        private Thickness th1=new Thickness(1);
        private Thickness th3=new Thickness(3);
        public bool IsSelected
        {
            get { return _isSelected; }
            set { 
                    if (value!=_isSelected)
                    {
                        _isSelected = value;
                        if (_isSelected)
                        {
                            if (SelectedCtrl!=null) SelectedCtrl.IsSelected=false;
                            Box.BorderBrush     = (Brush)FindResource("DataGrid.Brush.Selected.Background");
                            Box.BorderThickness = th3;
                            Box.Background      = Brushes.White;
                            SelectedCtrl = this;
                        }
                        else
                        {
                            Box.BorderBrush     = Brushes.DarkGray;
                            Box.BorderThickness = th1;
                            Box.Background      = Brushes.Transparent;
                            SelectedCtrl = null;
                        }
                    } 
                }
        }

    }
}
