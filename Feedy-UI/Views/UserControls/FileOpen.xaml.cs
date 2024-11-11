using Feedy.Models;
using GE_Utilities;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace Feedy.Views.UserControls
{
    /// <summary>
    /// Interaction logic for FileOpen.xaml
    /// </summary>
    public partial class FileOpen : Popup
    {
        private int _FilterIndex=-1;


        public FileOpen()
        {
            InitializeComponent();

            BTN_Import.Visibility = Visibility.Collapsed;
            BTN_Save.Visibility   = Visibility.Collapsed;
            BTN_Delete.Visibility = Visibility.Collapsed;
        }

        //--- Popup_Closed -------------------------
        private void Popup_Closed(object sender, EventArgs e)
        {
            OnSelected = null;
            OnSave     = null;
            OnImport   = null;
            OnDelete   = null;
        }

        //--- Property OnSave ---------------------------------------
        private Action<string> _OnSave;
        public Action<string> OnSave
        {
            set { 
                    _OnSave = value; 
                    BTN_Save.Visibility = (_OnSave==null)? Visibility.Collapsed: Visibility.Visible;
                }
        }


        //--- Property OnSave ---------------------------------------
        private Func<string, bool> _OnDelete;
        public Func<string, bool> OnDelete
        {
            set { 
                    _OnDelete = value; 
                    BTN_Delete.Visibility = (_OnDelete==null)? Visibility.Collapsed: Visibility.Visible;
                }
        }

        //--- Property OnImport ---------------------------------------
        private Action<string> _OnImport;
        public Action<string> OnImport
        {
            set { 
                    _OnImport = value; 
                    BTN_Import.Visibility = (_OnImport==null)? Visibility.Collapsed : Visibility.Visible;
                }
        }

        //--- Property OnImport ---------------------------------------
        private Action<string> _OnSelected;
        public Action<string> OnSelected
        {
            set { 
                    _OnSelected = value; 
                 //   BTN_Import.Visibility = (_OnImport==null)? Visibility.Collapsed : Visibility.Visible;
                }
        }

        //--- Property Title ---------------------------------------
        public string Title
        {
            set { TB_Title.Text=value; }
        }

        //--- Property SelectFolder ---------------------------------------
        private bool _SelectFolder=false;
        public bool SelectFolder
        {
            get { return _SelectFolder; }
            set { _SelectFolder =value; }
        }

        //--- Property IsSaveAs ---------------------------------------
        private bool _IsSaveAs;
        public bool IsSaveAs
        {
            set 
            { 
                _IsSaveAs=value; 
                if (_IsSaveAs) 
                {
                    SaveRow.Height        = GridLength.Auto;
                    BTN_Save.Visibility   = Visibility.Visible;
                    BTN_Import.Visibility = Visibility.Collapsed;
                }
                else
                {
                    SaveRow.Height = new GridLength(0);
                    BTN_Save.Visibility = Visibility.Collapsed;
                }
            }
        }

        //--- Property Files ---------------------------------------
        public FileList Files
        {
            set 
            {
                FileList.Items.Clear();
                foreach(GeEnum<string> item in value)
                {
                    FileList.Items.Add(item.Display);
                }
            }
        }

        //--- Extension ---------------------------
        string _Filter="CSV files (*.csv)|*.csv";
        public string Filter
        {
            set { _Filter=value;}
        }

        //--- Import_Clicked ---------------------------------------------
        private void Import_Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectFolder)
            {
                ImportFolder_Clicked(sender, e);
            }
            else
            {
                Action<string> onImport= _OnImport;
                Action<string> onSave  = _OnSave;
                IsOpen = false;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = _Filter;
                ofd.RestoreDirectory = true;
                ofd.Title = ("Import File");
                ofd.Multiselect = true;
                if (_FilterIndex>=0) ofd.FilterIndex = _FilterIndex;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _FilterIndex = ofd.FilterIndex;
                    foreach(string filename in ofd.FileNames)
                    {
                        string dst=FdGolbals.Job.Dir+Path.DirectorySeparatorChar+Path.GetFileName(filename);
                        if (onImport!=null)    onImport(ofd.FileName);
                    }
                    if (ofd.FileNames.Length==1 && onSave!=null) 
                    {
                        onSave(Path.GetFileName(ofd.FileName));
                    }
                }
            }
        }

        //--- ImportFolder_Clicked ---------------------------------------------
        private string _lastFolder;
        private void ImportFolder_Clicked(object sender, RoutedEventArgs e)
        {   
            Action<string> onImport  = _OnImport;

            IsOpen = false;
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            ofd.SelectedPath = _lastFolder;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _lastFolder = ofd.SelectedPath;
                onImport(ofd.SelectedPath);
            }
        }

        //--- Save_Clicked ---------------------------------------------
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            if (_OnSave!=null) _OnSave(TB_Name.Text);
            IsOpen = false;
        }

        //--- Delete_Clicked ---------------------------------------------
        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            if (_OnDelete!=null)
            {
                if (_OnDelete(TB_Name.Text))
                    FileList.Items.Remove(TB_Name.Text);
            }
        }

        //--- Cancel_Clicked ---------------------------------------------
        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        //--- List_SelectionChanged ----------------------------------
        private void List_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FileList.SelectedItem==null) return;
            if (FileList.SelectedItem!=null) TB_Name.Text=FileList.SelectedItem.ToString();
            if (_OnSelected!=null) 
            {
                _OnSelected(TB_Name.Text);
                IsOpen = false;
            }
        }
    }
}
