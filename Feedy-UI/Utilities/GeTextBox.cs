using Feedy.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GE_Utilities
{
    public class GeTextBox : TextBox
    {
        private int _SelectionStart;
        private int _SelectionLength;

        public GeTextBox()
        {
            SelectionChanged += GeSelectionChanged;
            PreviewKeyDown   += GePreviewKeyDown;
            PreviewMouseDown += GeTextBox_PreviewMouseDown;
            TouchUp          += GeTextBox_TouchUp;
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
        private void GeTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // used because touch is disabled
            if (GeScreen.IsTouch) 
            {
                e.Handled=true;
                GeTextBox_TouchUp(sender, null);
            }
        }

        //--- GeTextBox_TouchUp -----------------------------------
        void GeTextBox_TouchUp(object sender, TouchEventArgs e)
        {
            IsPadActive = true;
            GeTextPad pad = new GeTextPad(this, this.Text);
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
            DependencyProperty.Register("IsPadActive", typeof(bool), typeof(GeTextBox), new PropertyMetadata(false));

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

        //--- SelectField ----------------------------------
        public static void SelectField(TextBox ctrl, int selStart=-1)
        {
            int start= ctrl.SelectionStart;
            int fieldStart=-1;
            int fieldEnd=0;
            string text=ctrl.Text;

            if (selStart>=0) start=selStart;

            /*
            //--- heck whether in field ----
            for (int i = 0; i < ctrl.Text.Length; i++)
            {
                if (ctrl.Text[i] == FdDef.FieldStart[0]) 
                {
                    if (i<start) fieldStart=i;
                }
                else if (ctrl.Text[i] == FdDef.FieldEnd[0]) 
                {
                    fieldEnd=i;
                    if (i>=start) break;
                    fieldStart=-1;
                }
            }
            */
            if (fieldStart>=0)
            {
                if (fieldEnd<fieldStart) fieldEnd=ctrl.Text.Length+1;
                ctrl.Select(fieldStart, fieldEnd-fieldStart+1);
            }
        }

        //--- SelectionChanged ----------------------------
        private void GeSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox ctrl= sender as TextBox;

            SelectField(ctrl);

            _SelectionStart  = ctrl.SelectionStart;
            _SelectionLength = ctrl.SelectionLength;
            if (e!=null) e.Handled = true;
        }
    }
}
