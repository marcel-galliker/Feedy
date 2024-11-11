using MahApps.Metro.IconPacks;
using MS.WindowsAPICodePack.Internal;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace GE_Utilities
{
    public partial class GeTextPad : Window, INotifyPropertyChanged
    {        
        private double  _OrgValue;
        private Point   _Position;
        private double  _ObjWidth=0;
        private double  _ObjHeight=0;
        private bool    _Shift = false;

        private const int _Rows=5;
        private const int _Cols=11;

        private string[][] _Keys = new string[_Rows][];
        private Button[][] _Button = new Button[_Rows][];

        //--- creator -----------------------------------
        public GeTextPad(TextBox obj, string txt=null)
        {
            InitializeComponent();
            this.DataContext = this;
            PreviewKeyDown += GeTextPad_PreviewKeyDown;
            KeyDown += GeTextPad_KeyDown;
            
            //--- timer as work around first touch behavior of ELO ------

            _init_controls();

            _OrgValue = Ge.StrToDouble(obj.Text);

            _Position  = obj.PointToScreen(new Point(0, 0));
            _ObjWidth  = obj.ActualWidth;
            _ObjHeight = obj.ActualHeight;

            GeScreen screen =new GeScreen();
            screen.PlaceWindow(this);
            Top=0;
            Left=0;
            Width=screen.Width;
            Height=screen.Height;

            #if DEBUG
                this.Topmost = false;
            #else
                this.Topmost = true;
            #endif
            _Left.Width    = new GridLength(_Position.X*screen.Scale);
            _Width.Width   = new GridLength(_ObjWidth);
            _Top.Height    = new GridLength(_Position.Y*screen.Scale);
            _Height.Height = new GridLength(_ObjHeight);
            _Position.X += obj.ActualWidth;

         //   _Position.Y += obj.ActualHeight
            Result.Text = txt;
        }
        
        //--- _init_controls ------------------------------
        private void _init_controls()
        {
            int row, col;
            for(row=0; row<_Rows+1; row++) Pad.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(64)});
            for(col=0; col<_Cols; col++) Pad.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(64)});

            _Keys[0] = new string[]{"", "", "", "", "", "", "", "", "", "", "Cancel"};
            _Keys[1] = new string[]{"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Backspace"};
            _Keys[2] = new string[]{"q", "w", "e", "r", "t", "z", "u", "i", "o", "p", "KeyboardReturn"};
            _Keys[3] = new string[]{"a", "s", "d", "f", "g", "h", "j", "k", "l", " ", "ArrowUpBoldOutline"};
            _Keys[4] = new string[]{"y", "x", "c", "v", "b", "n", "m", ",", ".", "ArrowExpandLeft", "ArrowExpandRight"};

            _add_keys(0, _Keys[0]);
            _add_keys(1, _Keys[1]);
            _add_keys(2, _Keys[2]);
            _add_keys(3, _Keys[3]);
            _add_keys(4, _Keys[4]);
        }

        //--- _add_keys ---------------------------------
        private void _add_keys(int row, string[] keys)
        {
            int col;
            _Button[row] = new Button[_Cols];
            for(col=0; col<_Cols; col++)
            {
                Button b=new Button();
                _Button[row][col]=b;
                Grid.SetRow(b, row);
                Grid.SetColumn(b, col);
                b.Click += button_clicked;
                if (keys[col].Length==1) b.Content = keys[col];
                else if (keys[col].Length>1)
                { 
                    PackIconMaterial icon= new PackIconMaterial();
                    icon.Kind = (PackIconMaterialKind)Enum.Parse(typeof(PackIconMaterialKind), keys[col]);
                    b.Content = icon;
                    b.CommandParameter = keys[col];
                }
                if (b.Content!=null) 
                    Pad.Children.Add(b);
            }
        }

        //--- Pad_SizeChanged ------------------------------
        private void Pad_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            GeScreen screen =new GeScreen();
            double left = _Position.X*screen.Scale;
            double top  = _Position.Y*screen.Scale;
            double border=2;
            if (screen.Surface || screen.Elo)
            {
                top  += _ObjHeight;
                left -= 32;
                border=10;
            }
            if (top  + Pad.ActualHeight > Height-border)
            {
               left += _ObjWidth;
               top  = Height-Pad.ActualHeight-border;
            }
            if (left + Pad.ActualWidth  > Width-border )
            {
                left = Width-Pad.ActualWidth-border;
                top  += _ObjHeight;
            }
            Canvas.SetLeft (Pad, left);        
            Canvas.SetTop  (Pad, top);
        }

        //--- GeTextPad_PreviewKeyDown ----------------------------------------------
        void GeTextPad_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string key=e.Key.ToString();
            if (key.Equals("Back"))
            {
                _handle_key(key);
                e.Handled = true;
            }
        }

        //--- GeTextPad_KeyDown ----------------------------------------------
        void GeTextPad_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            _handle_key(e.Key.ToString());
        }

        //--- button clicked -----------------------------
        static int _Time=Environment.TickCount;
        private void button_clicked(object sender, RoutedEventArgs e)
        {
            //--- filter out double clicks ---
            {
                if  (Environment.TickCount-_Time<50) return;
                _Time=Environment.TickCount;
            }
            Button button = sender as Button;
            if (button==null) return;
            if (button.CommandParameter==null)  _handle_key(button.Content.ToString());
            else                                _handle_key(button.CommandParameter.ToString());
        }    

        //--- _shift -------------------------------
        private void _shift(bool shift)
        {
            if (shift!=_Shift)
            {
                int row, col;
                for(row=0; row<_Rows; row++)
                {
                    for(col=0; col<_Cols; col++)
                    {
                        string str = _Button[row][col].Content as string;
                        if (str!=null && str.Length==1)
                        {
                            if (shift) _Button[row][col].Content = str.ToUpper();
                            else       _Button[row][col].Content = str.ToLower();
                        }
                    }
                }
                _Shift=shift;
            }
        }

        //--- _handle_key --------------------------------------------------
        private void _handle_key(string key)
        {
            if (key.Equals("Cancel") || key.Equals("Escape"))
            {
                this.DialogResult = false;
            }
            else if (key.Equals("Backspace") || key.Equals("Back"))
            {
                if (Result.SelectionStart>0)
                {
                    if (Result.SelectionLength==0) 
                    {
                        GeTextBox.SelectField(Result, Result.SelectionStart-1);
                        if (Result.SelectionLength==0) Result.Select(Result.SelectionStart-1, 1);
                    }
                    Result.SelectedText = "";
                }
            }
            else if (key.Equals("KeyboardReturn") || key.Equals("Return"))
            {
                this.DialogResult = true;
            }
            else if (key.Equals("ArrowExpandRight"))
            {
                if (Result.SelectionStart<Result.Text.Length)
                {
                    Result.Select(Result.SelectionStart+1, 0);
                }
            }
            else if (key.Equals("ArrowExpandLeft"))
            {
                if (Result.SelectionStart>0)
                {
                    Result.Select(Result.SelectionStart-1, 0);
                }
            }
            else if (key.Equals("ArrowUpBoldOutline"))
            {
                _shift(!_Shift);
            }
            else if (key.Length==1) 
            { 
                Result.SelectedText = key;
                Result.Select(++Result.SelectionStart, 0);
                _shift(false);
            }
            Result.Focus();
        }

        //--- property changd -----------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        //--- special for the first touch ---------------------------------------------
        private void Window_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            button_clicked(e.Source, null);
        }

        //--- TextBox_IsVisibleChanged ---------------------
        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Result.Select(Result.Text.Length, 0);
                Result.Focus ();
            }
        }

        //--- Result_SelectionChanged -----------------------------
        private void Result_SelectionChanged(object sender, RoutedEventArgs e)
        {
            GeTextBox.SelectField(sender as TextBox);
        }
    }
}
