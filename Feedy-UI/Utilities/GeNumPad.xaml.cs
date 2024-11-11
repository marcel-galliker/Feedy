﻿using Feedy.Models;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GE_Utilities
{
    public partial class GeNumPad : Window, INotifyPropertyChanged
    {        
        private GeScreen _screen;
        private double  _OrgValue;
        private Point   _Position;

        //--- creator -----------------------------------
        public GeNumPad(TextBox obj)
        {
            InitializeComponent();
            this.DataContext = this;
            KeyDown += GeNumPad_KeyDown;  
            
            //--- timer as work around first touch behavior of ELO ------

            _OrgValue = Ge.StrToDouble(obj.Text);

            _Position  = obj.PointToScreen(new Point(0, 0));

            _screen =new GeScreen();
            _screen.PlaceWindow(this);
            Top=0;
            Left=0;
            Width=_screen.Width;
            Height=_screen.Height;

            #if DEBUG
                this.Topmost = false;
            #else
                this.Topmost = true;
            #endif
        }
        
        //--- Pad_SizeChanged ------------------------------
        private void Pad_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            double left = _Position.X*_screen.Scale;
            double top  = _Position.Y*_screen.Scale;
            double maxHeight = FdGolbals.MainWindow.ActualHeight-16;
            double maxWidth  = FdGolbals.MainWindow.ActualWidth-16;
            if (top + Pad.ActualHeight > maxHeight) top  = maxHeight-Pad.ActualHeight;
            if (left + Pad.ActualWidth  > maxWidth) left = maxWidth-Pad.ActualWidth;
            Canvas.SetLeft (Pad, left);        
            Canvas.SetTop  (Pad, top);
        }

        //--- Property result ------------------------------------------------
        private string _result="0";
        public string Result
        {
            get { return _result; }
            private set { _result = value; this.OnPropertyChanged("Result"); }
        }

        //--- GeNumPad_KeyDown ----------------------------------------------
        void GeNumPad_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==Key.Decimal || e.Key==Key.OemPeriod)     _handle_key(".");
            else if (e.Key==Key.Return || e.Key==Key.Back || e.Key==Key.Escape) _handle_key(e.Key.ToString());
            else 
            {
                int code=System.Windows.Input.KeyInterop.VirtualKeyFromKey(e.Key);         
                if (e.Key>=Key.NumPad0 && e.Key<=Key.NumPad9) code = '0'+e.Key-Key.NumPad0;
                _handle_key(new string(Convert.ToChar(code),1));
            } 
        }

        //--- button clicked -----------------------------
        static int _Time=Environment.TickCount;
        private void button_clicked(object sender, RoutedEventArgs e)
        {
            //--- filter out double clicks ---
            {
                if  (Environment.TickCount-_Time<100) return;
                _Time=Environment.TickCount;
            }
            Button button = sender as Button;
            if (button==null)
            {
                Control ctrl = sender as Control;
                button = ctrl.Parent as Button;
            }
            if (button==null) return;
            if (button.CommandParameter==null)  _handle_key(button.Content.ToString());
            else                                _handle_key(button.CommandParameter.ToString());
        }    

        //--- _handle_key --------------------------------------------------
        private void _handle_key(string key)
        {
            if (key.Equals("Escape"))       this.DialogResult = false;
            else if (key.Equals("Return"))  
                this.DialogResult = true;
            else if (key.Equals("+="))      
            {          
                double val=_OrgValue+Ge.StrToDouble(Result);
                Result = val.ToString(CultureInfo.InvariantCulture);     
                this.DialogResult = true;
            }
            else if (key.Equals("-="))      
            {
                double val=_OrgValue-Ge.StrToDouble(Result);
                Result = val.ToString(CultureInfo.InvariantCulture);     
                this.DialogResult = true;
            }
            else if (key.Equals("Back"))
            {
                if (Result.Length > 0) Result = Result.Remove(Result.Length - 1);
            }
            else if (key.Equals("+/-"))
            {
                if (Result[0]=='-') Result = Result.Remove(0,1);
                else                Result = '-'+Result;
            }
            else if (Result.Equals("0")) Result=key;
            else Result += key;
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
    }
}
