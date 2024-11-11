using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for MenuItem.xaml
    /// </summary>
    public partial class MenuItem : UserControl
    {
        private Action<string> _onClick;
        private string _id;

        //--- Property Top ---------------------------------------
        public int Group;
        public UserControl View;

        //--- creator --------------------------------------------------
        public MenuItem()
        {
            InitializeComponent();
            Line1.Visibility =  Line2.Visibility = Visibility.Collapsed;
        }

        //--- Property IsChecked ---------------------------------------
        private bool _IsChecked=false;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set 
            { 
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    UpdateIconColor();
                }
            }
        }

        //--- Property Off ---------------------------------------
        public bool Off
        {
            set { 
                    if (value) Line1.Visibility = Line2.Visibility = Visibility.Visible;
                    else Line1.Visibility = Line2.Visibility = Visibility.Collapsed;
                }
        }


        //--- OnClick ---------------------------------------
        public Action<string> OnClick
        {
            set { _onClick = value; }
        }

        //--- Property Id ---------------------------------------
        public string Id
        {
            get { return _id; } 
            set { _id = value; }
        }

        //--- Property Text ---------------------------------------
        public string Text
        {
            set { TB_Text.Text = value; }
        }

        public ImageSource Image
        {
            set { 
                    IconImage.Source = value;
                }
        }

        //--- Property Control ---------------------------------------
        Popup ctrl=null;
        public Popup Control
        {
            get { return ctrl; }
            set { 
                    if (ctrl == null) 
                    {
                        ctrl = value;
                        ctrl.Placement       = PlacementMode.Relative;
                        ctrl.PlacementTarget = TB_Text;
                        ctrl.VerticalOffset  = 0;
                        ctrl.HorizontalOffset= 0;
                    }
                }
        }

        //--- Property Icon ---------------------------------------
        public PackIconMaterialKind Kind
        {
            set { Icon.Kind = value; }
        }

        //--- UpdateIconColor -------------------------
        public void UpdateIconColor()
        {
            if (_IsChecked ) SetIconColor((Brush)FindResource("DataGrid.Brush.Selected.Background"));
            else SetIconColor(null);
        }

        //--- SetIconColor ------------------------------
        public void SetIconColor(Brush brush)
        {
            if (brush== null) Icon.Foreground = (Brush)FindResource("Menu.Icon.Brush");
            else Icon.Foreground = brush;
        }

        //--- Button_Clicked -------------------------------------------
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            _onClick(_id);
        }

        //--- Grid_SizeChanged -----------------------------
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Line1.X1=Line2.X1=0;
            Line1.Y1=Line2.Y1=0;
            Line1.X1=Line2.X1=Icon.ActualWidth+5;
            Line1.Y2=Line2.Y2=Icon.ActualHeight+5;
        }
    }
}
