﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GE_Utilities
{
    public class GeNumBox : TextBox
    {

        static CultureInfo _CultureInfo = new CultureInfo("en-US");

        //--- Constructor ----------------------------
        public GeNumBox()
        {
            Style = (Style)FindResource("GeNumBox");
            PreviewMouseDown +=GeNumBox_PreviewMouseDown;
            TouchUp     += GeNumBox_TouchUp;
            KeyDown     += OnKeyDown;
            LostFocus   += GeNumBox_LostFocus;
        }

        //--- GeNumBox_LostFocus -------------------------
        void GeNumBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double min=Min;
            double max=Max;
            double val = Ge.StrToDouble(Text);
            if (min!=double.NaN && val<min) val = min;
            else if (max!=double.NaN && val>max) val = max;
            
            if (this.Tag==null) this.Tag="GeNumPad";

            if (IsInt) Text=((int)val).ToString(_CultureInfo);
            else       Text=val.ToString(_CultureInfo);
        //    e.Handled = true;
        }

        //--- GeNumBox_PreviewMouseDown --------------------------------------------
        private void GeNumBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // used because touch is disabled
            if (GeScreen.IsTouch) 
            {
                e.Handled=true;
                GeNumBox_TouchUp(sender, null);
            }
        }

        //--- GeNumBox_MouseUp ------------------------------
        void GeNumBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsPadActive) GeNumBox_TouchUp(sender, null);
        }

        //--- Property IsPadActive ------------------------------------------------
        public bool IsPadActive
        {
            get { return (bool)GetValue(IsPadActiveProperty); }
            set { SetValue(IsPadActiveProperty, value); }
        }        

        // Using a DependencyProperty as the backing store for IsPadActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPadActiveProperty =
            DependencyProperty.Register("IsPadActive", typeof(bool), typeof(GeNumBox), new PropertyMetadata(false));
        
        //--- Property IsInt ------------------------------------------------
        public bool IsInt
        {
            get { return (bool)GetValue(IsIntProperty); }
            set { SetValue(IsIntProperty, value); }
        }        

        // Using a DependencyProperty as the backing store for IsPadActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIntProperty =
            DependencyProperty.Register("IsInt", typeof(bool), typeof(GeNumBox), new PropertyMetadata(false));

        //--- Property Min ------------------------------------------------
        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }        

        // Using a DependencyProperty as the backing store for IsPadActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(GeNumBox), new PropertyMetadata(double.NaN));

        //--- Property Max ------------------------------------------------
        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }        

        // Using a DependencyProperty as the backing store for IsPadActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(GeNumBox), new PropertyMetadata(double.NaN));


        //--- GeNumBox_TouchUp -----------------------------------
        void GeNumBox_TouchUp(object sender, TouchEventArgs e)
        {
            if (IsPadActive) return;
            IsPadActive = true;
            GeNumPad pad = new GeNumPad(this);
            bool? result=pad.ShowDialog();
            IsPadActive = false;
            if (result!=null && (bool)result == true)
            {
                Text = pad.Result;
                RaiseEvent(new RoutedEventArgs(LostFocusEvent, this));
            }
        }

        //--- OnKeyDown ------------------------------------
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.ToString() == "Return")
                RaiseEvent(new RoutedEventArgs(FocusManager.LostFocusEvent));   // force refresh!            
            else
            {
                double d = GeKeys.Value(e);
                if (d!=0)
                {
                    try
                    { 
                        double val  = double.Parse(Text, CultureInfo.InvariantCulture) +d;
                        Text = val.ToString(CultureInfo.InvariantCulture);
                        RaiseEvent(new RoutedEventArgs(FocusManager.LostFocusEvent));   // force refresh!
                    }
                    catch (Exception)
                    {
                        Text = d.ToString(CultureInfo.InvariantCulture);
                        return;
                    }
                }

                if (Text.Equals("####"))
                {
                    Text =  "";
                    e.Handled=true;
                    return;
                }
            }
        }

        //--- Property Value ---------------------------------------
        public double Value
        {
            get { return Ge.StrToDouble(Text);}
        }

        //--- Val1000 ------------
        public int Val1000
        {
            get { return (int)(1000*Ge.StrToDouble(Text));}
        }
        
    }
}
