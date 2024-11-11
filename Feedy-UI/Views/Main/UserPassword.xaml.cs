using GE_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for UserPassword.xaml
    /// </summary>
    public partial class UserPassword : Popup
    {
        private bool _localInput=false;
        public UserPassword()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnGlobalMousepDown), true);
            TB_ShowPassword.Visibility = Visibility.Collapsed;
            TB_Password.IsPassword = true;
        }

        //--- Property OnEnter ---------------------------------------
        private Action<string> _OnEnter;
        public Action<string> OnEnter
        {
            set { _OnEnter = value; }
        }

        //--- Popup_Opened --------------------------
        private void Popup_Opened(object sender, EventArgs e)
        {
            TB_Password.Focus();
        }

        //--- Popup_Closed ------------------------------------
        private void Popup_Closed(object sender, EventArgs e)
        {
            if (_OnEnter!=null) _OnEnter("");
        }

        //--- TB_Password_KeyDown -------------------------------
        private void TB_Password_KeyDown(object sender, KeyEventArgs e)
        {
            _localInput = true;
            if (e.Key == Key.Enter)
            {
            //    if (_OnEnter!=null) _OnEnter(TB_Password.Password.Trim());
                if (_OnEnter!=null) _OnEnter(TB_Password.Text.Trim());
            }
        }

        //--- Property Valid ---------------------------------------
        public bool Valid
        {
            set { 
                    if(value==false)
                    {
                     //   TB_ShowPassword.Text       = TB_Password.Password;
                        TB_ShowPassword.Text       = TB_Password.Text;
                        TB_Password.Visibility     = Visibility.Collapsed;
                        TB_ShowPassword.Visibility = Visibility.Visible;
                        GeBindable.InvokeDelayed(3000, ()=>
                        {
                            TB_Password.Text = "";

                            TB_Password.Visibility     = Visibility.Visible;
                            TB_ShowPassword.Visibility = Visibility.Collapsed;
                            TB_Password.Focus();
                        });
                    }
                }
        }


        //--- OnGlobalMousepDown ---------------------------
        private void OnGlobalMousepDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(Child);
            Border border = Child as Border;

            if (TB_Password.IsPadActive) return;

            if (border!=null)
            {
                if (this.IsOpen && (pos.X<0 || pos.X>border.ActualWidth || pos.Y<0 || pos.Y>border.ActualHeight))
                {
                    this.IsOpen = false;
                    e.Handled = true;
                }
            }

            Grid grid = this.Child as Grid;
            if (grid!=null)
            {
                if (this.IsOpen && (pos.X<0 || pos.X>grid.ActualWidth || pos.Y<0 || pos.Y>grid.ActualHeight))
                {
                    this.IsOpen = false;
                    e.Handled = true;
                }
            }
        }

        //--- TB_Password_TextChanged ----------------------------
        private void TB_Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_localInput && _OnEnter!=null) _OnEnter(TB_Password.Text.Trim());
            _localInput = false;
        }
    }
}
