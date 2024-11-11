using Feedy.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GE_Utilities
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class GeRangeSlider : UserControl
    {
        public GeRangeSlider()
        {
            InitializeComponent();

            UpperSlider.Visibility= Visibility.Collapsed;
        }

        //--- InitControls ---------------------------
        public void InitControls(Orientation orientation)
        {
            if (orientation==Orientation.Horizontal)
            {
                LowerSlider.Orientation=Orientation.Horizontal;
                LowerSlider.IsDirectionReversed=false;
                LowerSlider.Template = (ControlTemplate)FindResource("RangeSliderH");

                UpperSlider.Orientation=Orientation.Horizontal;
                UpperSlider.IsDirectionReversed=false;
                UpperSlider.Template = (ControlTemplate)FindResource("RangeSliderH");
            }
            else
            {
                LowerSlider.Orientation=Orientation.Vertical;
                LowerSlider.IsDirectionReversed=true;
                LowerSlider.Template = (ControlTemplate)FindResource("RangeSliderV");

                UpperSlider.Orientation=Orientation.Vertical;
                UpperSlider.IsDirectionReversed=true;
                UpperSlider.Template = (ControlTemplate)FindResource("RangeSliderV");
            }
        }

        //--- Property LowerSliderVisible ---------------------------------------
        public bool LowerSliderVisible
        {
            get { return  LowerSlider.Visibility == Visibility.Visible;}

            set 
            { 
                if(value) LowerSlider.Visibility = Visibility.Visible;
                else LowerSlider.Visibility = Visibility.Collapsed;
            }
        }

        //--- Property LowerSliderVisible ---------------------------------------
        public bool UpperSliderVisible
        {
            get { return  UpperSlider.Visibility == Visibility.Visible; }

            set 
            { 
                if(value) UpperSlider.Visibility = Visibility.Visible;
                else UpperSlider.Visibility = Visibility.Collapsed;
            }
        }

        //--- Minimum --------------------------------------------
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(GeRangeSlider), new UIPropertyMetadata(0d));

        //--- Maximum -------------------------------------------
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(GeRangeSlider), new UIPropertyMetadata(1d));

        //--- LowerValue -----------------------------------------
        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(GeRangeSlider), new UIPropertyMetadata(0d));

        //--- UpperValue -----------------------------------------------------
        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(GeRangeSlider), new UIPropertyMetadata(0d));

        //--- _UserControl_Loaded ----------------------
        private void _UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LowerSlider.ValueChanged  += _LowerSlider_ValueChanged;
            UpperSlider.ValueChanged  += _UpperSlider_ValueChanged;
        }

        //--- LowerSlider_ValueChanged -------------------------------
        private const int MIN=5000;

        public Action<double> OnLower_ValueChanged;
        private double _LowerValue=0;
        private void _LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (UpperSlider.IsDirectionReversed)
            {
                double lv=LowerSlider.Value-MIN;
                if (lv<0) lv=0;
                if (lv<UpperSlider.Value) UpperSlider.Value = lv;
                if (UpperSlider.Visibility==Visibility.Visible && LowerSlider.Value<MIN) LowerSlider.Value=MIN;
            }
            else
            {
	            LowerSlider.Value  = Math.Min(LowerSlider.Value,  LowerSlider.Maximum);
	            UpperSlider.Value  = Math.Max(UpperSlider.Value,  LowerSlider.Value);
            }
            _LowerValue = LowerSlider.Value;
            if (OnLower_ValueChanged!=null) OnLower_ValueChanged(LowerValue);
        }

        //--- UpperSlider_ValueChanged ------------------------------------
        public Action<double> OnUpper_ValueChanged;
        private double _UpperValue=0;
        private void _UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (UpperSlider.Value==_UpperValue) return;
            if (UpperSlider.IsDirectionReversed)
            {
            }
            else
            {
                double lv=UpperSlider.Value-MIN;
                if (lv<0) lv=0;
                if (LowerSlider.Value>lv) LowerSlider.Value = lv;
                if (UpperSlider.Value<MIN) UpperSlider.Value=MIN;
            }
            _UpperValue = UpperSlider.Value;
            if (OnUpper_ValueChanged!=null) OnUpper_ValueChanged(UpperValue);
        }
    }
}
