using EZ_Editor.Models;
using GE_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EZ_Editor.Views.TrackingView
{
    /// <summary>
    /// Interaction logic for TrackingCtrl.xaml
    /// </summary>
    public partial class TrackingCtrl : UserControl
    {
        private int _CheckIdx = -1;
        private bool _Pause   = false;

        public TrackingCtrl()
        {
            InitializeComponent();

            Product.List.CollectionChanged += List_CollectionChanged;

            _init_products();
            SetCheck(5);
        }

        private void _setCheck(int idx, bool check)
        {
            if (idx>=0 && idx<TrackingGrid.Children.Count) (TrackingGrid.Children[idx] as ProductCtrl).IsChecked=check;
        }

        //--- SetCheck --------------------
        public void SetCheck(int idx)
        {
            _setCheck(_CheckIdx, false);
            _CheckIdx = idx;
            _setCheck(_CheckIdx, true);
        }

        //--- List_CollectionChanged -----------------------------------------
        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                GeBindable.Invoke(()=>
                {
                    for (int i=0; i < TrackingGrid.Children.Count; i++)
                    {
                        TrackingGrid.Children[i].Visibility = Visibility.Collapsed;
                    }
                });
            }
            else if (e.Action==System.Collections.Specialized.NotifyCollectionChangedAction.Add && !_Pause)
            {
                GeBindable.Invoke(()=>
                {
                    _setCheck(_CheckIdx, false);
                    for (int i=0; i < Product.List.Count; i++)
                    {
                        (TrackingGrid.Children[i] as ProductCtrl).DataContext = Product.List[i];
                    }
                   _setCheck(_CheckIdx, true);
                });
            }
        }

        //--- _init_products ------------------------------------------
        private void _init_products()
        {
            GridLength colWidth  = new GridLength(75);
            GridLength rowHeight = new GridLength(70);

            int cols=(int)(TrackingGrid.ActualWidth/colWidth.Value);
            int rows=(int)(TrackingGrid.ActualHeight/rowHeight.Value);

            if (rows*cols==0) return;
            
            if (cols>20) cols=20;

            for (int w=0; w<cols; w++)
            {
                if (TrackingGrid.ColumnDefinitions.Count<=w)
                    TrackingGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width=colWidth});
                else
                    TrackingGrid.ColumnDefinitions[w].Width=colWidth;
            }
            for (int h=0; h<rows; h++)
            {
                if (TrackingGrid.RowDefinitions.Count<=h)
                    TrackingGrid.RowDefinitions.Add(new RowDefinition() { Height=rowHeight});
                else TrackingGrid.RowDefinitions[h].Height=rowHeight;
            }

            for (int i=0; i<rows*cols && i<Product.Size; i++)
            {
                if (TrackingGrid.Children.Count<=i) TrackingGrid.Children.Add(new ProductCtrl());
                int row=i/cols;
                int col=i%cols;
                Grid.SetRow   (TrackingGrid.Children[i], row);
                Grid.SetColumn(TrackingGrid.Children[i], col);
            }
        }

        //--- TrackingGrid_SizeChanged -----------------------
        private void TrackingGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _init_products();
        }

        //--- Pause_Clicked -----------------------------------------------
        private void Pause_Clicked(object sender, RoutedEventArgs e)
        {
            _Pause=!_Pause;
            
            BTN_Pause.Kind = _Pause? MahApps.Metro.IconPacks.PackIconMaterialKind.Play : MahApps.Metro.IconPacks.PackIconMaterialKind.Pause;            
        }
    }
}
