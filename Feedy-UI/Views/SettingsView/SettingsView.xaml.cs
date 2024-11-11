using Feedy.Models;
using Feedy.Models.Enums;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Feedy.Views.SettingsView
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private string _TeamViewerQS = "C:\\Program Files\\TeamViewer\\TeamViewerQS_x64.exe";
        private string _TeamViewer = "C:\\Program Files\\TeamViewer\\TeamViewer.exe";
        private string _Explorer = "C:\\Windows\\Explorer.exe";

        public SettingsView()
        {
            InitializeComponent();

            DataContext = FdGolbals.Settings;
            FdGolbals.Language.LanguageChanged   += _Language;
            FdGolbals.User.UserChanged           += _UserChanged;

            CB_Language.DataContext = FdGolbals.Language;
            CB_Language.ItemsSource = FdGolbals.Language.Languages;
            CB_Units.ItemsSource    = new EN_Units();

            TX_Version.DataContext  = FdGolbals.Version;
            TX_Date.DataContext     = FdGolbals.Version;
            TX_Branch.DataContext   = FdGolbals.Version;
            TX_Commit.DataContext   = FdGolbals.Version;
            _Language();
            _UserChanged();
        }
         
        //--- _Language ----------------------------------------------
        private void _Language()
        {
            TB_Settings.Text  = FdGolbals.Language.GetText("Settings");
            TB_Language.Text  = FdGolbals.Language.GetText("Language");
            TB_IdAppress.Text = FdGolbals.Language.GetText("IpAddress");
            TB_DisplayWidth.Text = FdGolbals.Language.GetText("DisplayWidth");
            _showTeamViewer();
        }

        //--- _showTeamViewer ----------------------------
        private void _showTeamViewer()
        {
            Visibility visible = File.Exists(_TeamViewer) || File.Exists(_TeamViewerQS)? Visibility.Visible: Visibility.Collapsed;
            SEP_Web.Visibility = visible;
            BN_TeamViewer.Visibility = visible;
        }

        //--- _UserChanged --------------------
        private void _UserChanged()
        {
            foreach(UIElement ctrl in SettingsGrid.Children)
            {
                CheckBox cb= ctrl as CheckBox;
                ctrl.IsEnabled = FdGolbals.User.Enabled;
            }
            _showTeamViewer();
            CB_Language.IsEnabled  = true;
            TX_IdAppress.IsEnabled = true;
        }

        //--- Save_Clicked ---------------------------------------
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            FdGolbals.Settings.Save();
        }

        //--- Reload_Clicked ---------------------------
        private void Reload_Clicked(object sender, RoutedEventArgs e)
        {
            FdGolbals.Settings.Load();
        }

        //--- TeamViewer_Clicked ---------------------------
        private void TeamViewer_Clicked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_TeamViewer)) Process.Start(_TeamViewer);
            else Process.Start(_TeamViewerQS);
        }

        //--- Explorer_Clicked ---------------------------
        private void Explorer_Clicked(object sender, RoutedEventArgs e)
        {
            Process.Start(_Explorer);
        }
    }
}
