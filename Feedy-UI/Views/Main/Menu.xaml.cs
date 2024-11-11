using Feedy.Models;
using Feedy.Models.Enums;
using GE_Utilities;
using MahApps.Metro.IconPacks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        private EN_ScreenSize                   _ScreenSizeList = new EN_ScreenSize();
        private Dictionary<string, MenuItem>    _MenuItem = new Dictionary<string, MenuItem>();
        
        public Menu()
        {
            InitializeComponent();

            FdGolbals.Menu = this;
            FdGolbals.Events.PropertyChanged     += _Events_PropertyChanged;
            FdGolbals.User.UserChanged           += _UserChanged;
            FdGolbals.Language.LanguageChanged   += _Language;

            POP_Power.Visibility = Visibility.Collapsed;
            if (GeScreen.IsTouch)
            {
                BN_Power.Visibility      = Visibility.Visible;
            //  TB_ScreenSize.Visibility = Visibility.Collapsed;
                CB_ScreenSize.Visibility = Visibility.Collapsed;
            }
            else
            {
                BN_Power.Visibility      = Visibility.Collapsed;
            //  TB_ScreenSize.Visibility = Visibility.Visible;
                CB_ScreenSize.Visibility = Visibility.Visible;
            }
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
            foreach(var item in _MenuItem)
            {
                if (item.Value.Id=="User")
                        item.Value.Text = FdGolbals.User.Name;
                else item.Value.Text = FdGolbals.Language.GetText(item.Value.Id);
            }
        }

        //--- _FeedyConnectedChanged -----------------------------
        private void _FeedyConnectedChanged()
        {
            bool connected = FdGolbals.FdInterface.Connected;

            _MenuItem["Job"].Off        = !connected;
            _MenuItem["Events"].Off     = !connected;
            _MenuItem["Log"].Off        = !connected;
        }
        
        //--- Events_PropertyChanged ------------------------------
        private void _Events_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="Type") 
                _EventTypeChanged();
        }

        //--- EzInterface_PropertyChanged ---------------------------------------------
        private void EzInterface_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected") _FeedyConnectedChanged();
        }

        //--- MenuBar_Loaded --------------------------
        private void MenuBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (_MenuItem.Count>0) return;

            _MenuItem.Add("Job",        new MenuItem()
                                        {
                                            Group=0,
                                            Id="Job",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.TicketOutline, 
                                            View=FdGolbals.MainWindow.JobView,
                                            OnClick=_CheckMenu 
                                        });

            _MenuItem.Add("Events",     new MenuItem()
                                        {
                                            Group=1,
                                            Id="Events",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.FormatListBulleted, 
                                            View=FdGolbals.MainWindow.EventsView,
                                            OnClick=_CheckMenu 
                                        });

            _MenuItem.Add("Log",     new MenuItem()
                                        {
                                            Group=1,
                                            Id="Log",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.FileEyeOutline, 
                                            View=FdGolbals.MainWindow.LogView,
                                            OnClick=_CheckMenu 
                                        });

            _MenuItem.Add("Config",     new MenuItem()
                                        {
                                            Group=1,
                                            Id="Config",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.RobotIndustrial, 
                                            View=FdGolbals.MainWindow.ConfigView,
                                            OnClick=_CheckMenu 
                                        });

            _MenuItem.Add("Settings",   new MenuItem()
                                        {
                                            Group=1,
                                            Id="Settings",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.CogOutline, 
                                            View=FdGolbals.MainWindow.SettingsView,
                                            OnClick=_CheckMenu 
                                        });
            _MenuItem.Add("User",       new MenuItem()
                                        {
                                            Group=1,
                                            Id="User",
                                            Kind=MahApps.Metro.IconPacks.PackIconMaterialKind.CogOutline, 
                                            Control = new UserType(),
                                            OnClick=User_Clicked 
                                        });

            _Language();

            //--- group 0 -----------------------------
            foreach(var item in _MenuItem)
            {
                if ((item.Value as MenuItem).Group==0)
                    MenuBar.Children.Add(item.Value);
            }

            //--- group 1 ---------------------
            System.Windows.Shapes.Rectangle rect=new System.Windows.Shapes.Rectangle() { Width=200, Height=1, Fill=Brushes.Gray };

            MenuBar.Children.Add(rect);
            
            foreach(var item in _MenuItem)
            {
                if ((item.Value as MenuItem).Group==1)
                    MenuBar.Children.Add(item.Value);
            }

            CB_ScreenSize.SelectedIndex = 0;
            CB_ScreenSize.DataContext   = this;
            CB_ScreenSize.ItemsSource   = _ScreenSizeList;

          //  CB_UserType.DataContext     = FdGolbals.User;
          //  CB_UserType.ItemsSource     = new EN_UserTypeList();

            FdGolbals.FdInterface.PropertyChanged += EzInterface_PropertyChanged;
            _FeedyConnectedChanged();
            _UserChanged();
            _CheckMenu("Job");
            _MenuItem["Events"    ].Visibility = Visibility.Collapsed;
            _MenuItem["Log"       ].Visibility = Visibility.Collapsed;
        }

        //--- _CheckMenu ------------------
        private void _CheckMenu(string name)
        {
            int group=_MenuItem[name].Group;
            foreach(var item in _MenuItem)
            {
                if (item.Value.View!=null)
                {
                    if (item.Value.Group==group)
                    {
                        if (item.Key.Equals(name))
                        {
                            item.Value.IsChecked = true;
                            item.Value.View.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            item.Value.IsChecked = false;
                            item.Value.View.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        //--- Property IsExpanded ---------------------------------------
        private bool _IsExpanded=false;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set {
                    if (value!=_IsExpanded)
                    {
                        var col = ColDef;
                        _IsExpanded = value;
                        if (_IsExpanded) col.Width = new GridLength(0);
                        else col.Width = GridLength.Auto;
                }
            }
        }

        //--- _EventTypeChanged ------------------------------------
        private void _EventTypeChanged()
        {
            switch(FdGolbals.Events.Type)
            {
                case ENEventType.warn: _MenuItem["Events"].SetIconColor(Brushes.Orange); break;
                case ENEventType.err:  _MenuItem["Events"].SetIconColor(Brushes.Red); break;
                default: _MenuItem["Events"].UpdateIconColor(); break;
            }
        }

        //--- CB_ScreenSize_SelectionChanged -----------------------------------
        private void CB_UserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         //   FdGolbals.User.Type = (EN_UserType)CB_UserType.SelectedValue;
        }

        //--- _UserChanged ---------------------------------------------------
        private void _UserChanged()
        {
            _MenuItem["User"].Text = FdGolbals.User.Name;
            _MenuItem["User"].Kind = FdGolbals.User.Icon;
        }

        //--- CB_ScreenSize_SelectionChanged -----------------------------------
        private bool _first=true;
        private void CB_ScreenSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_first)
            {
                if (CB_ScreenSize.SelectedIndex==0)
                {
                    FdGolbals.MainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    var item = (CB_ScreenSize.SelectedItem) as GeEnum<int>;
                    FdGolbals.MainWindow.WindowState = WindowState.Normal;
                    FdGolbals.MainWindow.Width = item.Value >> 16;
                    FdGolbals.MainWindow.Height = item.Value & 0xffff;
                }
            }
            _first =false;
        }

        //--- User_Clicked ---------------------------------------
        private void User_Clicked(string name)
        {
            _MenuItem["User"].Control.IsOpen = true;
        }

        //--- Power_Clicked ---------------------------------------
        private bool _Power_MouseOver = false;
        private void Power_Clicked(object sender, RoutedEventArgs e)
        {
            POP_Power.Visibility = Visibility.Visible;
            _Power_MouseOver = false;
            GeBindable.InvokeDelayed(1000, () => { if (!_Power_MouseOver) POP_Power.Visibility = Visibility.Collapsed;});
        }

        //--- Power_MouseEnter ----
        private void Power_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _Power_MouseOver = true;
        }

        // Power_MouseLeave
        private void Power_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            POP_Power.Visibility = Visibility.Collapsed;
        }

        //--- ShutDown_Clicked ----------------------------------
        private void ShutDown_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/s /t 0");
        }

        //--- Restart_Clicked ----------------------------------
        private void Restart_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start("shutdown", "/r /t 0");
        }
    }
}