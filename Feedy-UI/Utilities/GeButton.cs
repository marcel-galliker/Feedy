using System;
using System.Windows;
using System.Windows.Controls;

namespace GE_Utilities
{
    public class GeButton : Button
    {
        public GeButton()
        {
            Click            += GeButton_Click;
            PreviewMouseDown += GeButton_PreviewMouseDown;
            MouseLeave       += GeButton_MouseLeave;
            ClickFontSize = 8;
        }

        //--- Property IsChecked ---------------------
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(GeButton));

        //--- Property IsBusy ---------------------
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(GeButton));


        //--- Property IsClicked ---------------------
        public bool IsClicked
        {
            get { return (bool)GetValue(IsClickedProperty); }
            set { SetValue(IsClickedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClickedProperty =
            DependencyProperty.Register("IsClicked", typeof(bool), typeof(GeButton));

        //--- Property ClickFontSize ---------------------
        public double ClickFontSize
        {
            get { return (double)GetValue(ClickFontSizeProperty); }
            set { SetValue(ClickFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickFontSizeProperty =
            DependencyProperty.Register("ClickFontSize", typeof(double), typeof(GeButton));

        //--- Property Confirm ---------------------
        public Action<object, RoutedEventArgs> Confirm
        {
            get { return (Action<object, RoutedEventArgs>)GetValue(ConfirmProperty); }
            set { SetValue(ConfirmProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfirmProperty =
            DependencyProperty.Register("Confirm", typeof(Action<object, RoutedEventArgs>), typeof(GeButton));

        //--- Property Start ---------------------
        public Action<object, RoutedEventArgs> Start
        {
            get { return (Action<object, RoutedEventArgs>)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(Action<object, RoutedEventArgs>), typeof(GeButton));

        //--- Property Start ---------------------
        public Action<object, RoutedEventArgs> Stop
        {
            get { return (Action<object, RoutedEventArgs>)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.Register("Stop", typeof(Action<object, RoutedEventArgs>), typeof(GeButton));

        //--- Property Reject ---------------------
        public Action<object, RoutedEventArgs> Reject
        {
            get { return (Action<object, RoutedEventArgs>)GetValue(RejectProperty); }
            set { SetValue(RejectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsButtonChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RejectProperty =
            DependencyProperty.Register("Reject", typeof(Action<object, RoutedEventArgs>), typeof(GeButton));

        //--- GeButton_Click ------------------------
        private int _MouseRunningState=0;

        private void GeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_MouseRunningState==2) 
            {
                if (Stop!=null) Stop(this, null);
                _MouseRunningState = 0;
                e.Handled = true;
                return;
            }
            _MouseRunningState = 0;
            if (Confirm!=null)
            {
                e.Handled = true;
                if(IsClicked)
                {
                    IsClicked= false;
                    Confirm(sender, e);
                }
                else
                {
                    ClickFontSize = ActualHeight/3;
                    IsClicked=true;
                    GeBindable.InvokeDelayed(500, ()=>
                    {
                        if (IsClicked && Reject!=null) Reject(sender, e);
                        IsClicked=false;
                    });
                }
            }
        }

        //--- GeButton_PreviewMouseDown ------------------------------
        private void GeButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Start!=null) 
            {
                _MouseRunningState = 1;
                GeBindable.InvokeDelayed(100, () => 
                {
                    if (_MouseRunningState==1)
                    {
                        if (Start!=null) Start(this, null);
                        _MouseRunningState = 2;
                    }
                });
            }
            else _MouseRunningState=0;
        }

        private void GeButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_MouseRunningState==2) 
            {
                if (Stop!=null) Stop(this, null);
                _MouseRunningState = 0;
                e.Handled = true;
                return;
            }
        }

    }
}
