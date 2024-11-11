using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GE_Utilities
{
    public class GeMultiTextBox : TextBox
    {
        private int _SelectionStart=0;
        private int _SelectionLength=0;

        public GeMultiTextBox()
        {
            SelectionChanged += GeSelectionChanged;
            PreviewKeyDown   += GePreviewKeyDown;
            PreviewMouseDown += GeTextBox_PreviewMouseDown;
            TouchUp          += GeTextBox_TouchUp;
            IsInactiveSelectionHighlightEnabled=true;
        }

        //--- GePreviewKeyDown ------------------------------------
        private void GePreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==System.Windows.Input.Key.Back)
            {
                GeSelectionChanged(this, null);
            }
        }

        //--- GeTextBox_PreviewMouseDown --------------------------------------------
        private bool _AfterMouseDown=false;
        private void GeTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GeScreen.IsTouch) _AfterMouseDown = true;
        }

        //--- GeTextBox_TouchUp -----------------------------------
        void GeTextBox_TouchUp(object sender, TouchEventArgs e)
        {
            IsPadActive = true;
            GeTextPad pad = new GeTextPad(this);
            bool? result=pad.ShowDialog();
            IsPadActive = false;
            if (result!=null && (bool)result == true)
            {
                Text = pad.Result.Text;
                RaiseEvent(new RoutedEventArgs(LostFocusEvent, this));
            }
        }

        //--- Property IsPadActive ------------------------------------------------
        public bool IsPadActive
        {
            get { return (bool)GetValue(IsPadActiveProperty); }
            set { SetValue(IsPadActiveProperty, value); }
        }        

        // Using a DependencyProperty as the backing store for IsPadActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPadActiveProperty =
            DependencyProperty.Register("IsPadActive", typeof(bool), typeof(GeMultiTextBox), new PropertyMetadata(false));

        //--- Property IsPassword ---------------------------------------
        private bool _IsPassword;
        public bool IsPassword
        {
            get { return _IsPassword; }
            set { _IsPassword = value; }
        }

        //--- Property Password ---------------------------------------
        private string _Password;
        public string Password
        {
            get { return Text; }
            set { _Password = value; }
        }

        //--- Insert -----------------------------
        public void Insert(string txt)
        {
            int start = SelectionStart;
            string text = Text.Remove(_SelectionStart, _SelectionLength);
            Text = text.Insert(start, txt);
            SelectionStart = start+txt.Length;
            SelectionLength = 0;
            Focus();
        }

        private TextBox _SelectedText = new TextBox();

        //--- SelectionChanged ----------------------------
        private void GeSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox ctrl= sender as TextBox;

            if (GeScreen.IsTouch && _AfterMouseDown) 
            {
                e.Handled=true;

                SelectionChanged -= GeSelectionChanged;
                // select whole line ---
                int start= ctrl.SelectionStart;
                int end=start;
                while (start>0 && ctrl.Text[start-1]!='\n') start--;
                while (end+1<ctrl.Text.Length && ctrl.Text[end+1]!='\n' || end==ctrl.Text.Length-1) end++;
                string txt = ctrl.Text.Substring(start, end-start);
                if (start!=end) 
                {
                    ctrl.SelectionStart = start;
                    ctrl.SelectionLength = end-start;
                }
                SelectionChanged += GeSelectionChanged; 
                Action action=new Action(()=>
                {                                    
                    
                    IsPadActive = true;
                    GeTextPad pad = new GeTextPad(this, txt);
                    bool? result=pad.ShowDialog();
                    IsPadActive = false;
                    if (result!=null && (bool)result == true)
                    {
                        ctrl.SelectedText = pad.Result.Text    ;
                        RaiseEvent(new RoutedEventArgs(LostFocusEvent, this));
                    }
                });
                GeBindable.InvokeDelayed(10, action);
            }
            else 
            {
                GeTextBox.SelectField(ctrl);
                _SelectionStart  = ctrl.SelectionStart;
                _SelectionLength = ctrl.SelectionLength;
            }
            _AfterMouseDown = false;
        }

    }
}
