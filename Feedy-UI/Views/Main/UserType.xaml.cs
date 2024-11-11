using Feedy.Models;
using MahApps.Metro.IconPacks;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for UserType.xaml
    /// </summary>
    public partial class UserType : Popup
    {
        private bool            _AwaitPwd=false;
        private EN_UserType     _userType;
        private UserPassword    _pwdDialog;

        public UserType()
        {
            InitializeComponent();
            BN_UserType_Operator.Tag    = EN_UserType.USER_operator;
            BN_UserType_Supervisor.Tag  = EN_UserType.USER_supervisor;
            BN_UserType_Service.Tag     = EN_UserType.USER_service;
        }

        //--- User_MouseLeave -------------------------------------
        private void User_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_AwaitPwd) IsOpen=false;
        }

        //--- UserType_Clicked --------------------------------------------
        private void UserType_Clicked(object sender, RoutedEventArgs e)
        {
            Button btn= (Button)sender;
            if (btn!=null) 
            {
                _userType = (EN_UserType)btn.Tag;
                if (_userType<=FdGolbals.User.Type) 
                {
                    FdGolbals.User.Type = (EN_UserType)btn.Tag;
                    IsOpen=false;
                }
                else
                {
                    _AwaitPwd = true;
                    if (_pwdDialog!=null && _pwdDialog.IsOpen)
                    {
                        _pwdDialog.OnEnter=null;
                        _pwdDialog.IsOpen= false;
                    }
                    _pwdDialog = new UserPassword();
                    _pwdDialog.OnEnter = _Password;
                    _pwdDialog.PlacementTarget= btn;
                    _pwdDialog.Placement = PlacementMode.Right;
                    _pwdDialog.IsOpen= true;
                }
            }
        }

        //--- pwd -------------------
        private void _Password(string pwd)
        {
            bool ok=User.Login(_userType, pwd);
            if (ok) 
            { 
                _AwaitPwd = false;
                IsOpen=false;
            }
            else
            {
                _pwdDialog.Valid=false;
            }
        }

        //--- Popup_Opened ------------------------------------
        private void Popup_Opened(object sender, EventArgs e)
        {
            foreach(Button btn in UserTypes.Children)
            {
                Grid grid= btn.Content as Grid;
                (grid.Children[0] as PackIconMaterial).Kind = User.GetIcon((EN_UserType)btn.Tag);
                (grid.Children[1] as TextBlock).Text = User.GetName((EN_UserType)btn.Tag);
            }
        }
    }
}
