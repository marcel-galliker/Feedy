﻿using Feedy.Models;
using Feedy.Models.Enums;
using System.IO;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Feedy.Views.JobView
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class GripperSetView : UserControl
    {
        public GripperSetView()
        {
            InitializeComponent();

            FdGolbals.Language.LanguageChanged += _Language;

            DataContext = FdGolbals.JobSettings.Gripper;
        
            CB_GripperType.ItemsSource      = new EN_GripperType();

        //    IMG_Gripper.Source = new BitmapImage(new Uri("D:\\Feedy\\Jobs\\Job1\\Greifer B.png"));
            IMG_Gripper.SetBinding(Image.SourceProperty, new Binding("ImagePath"));
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

                //--- _update_files -------------------------
        private FileList _update_files()
        {
            FileList fileList=new FileList();
            string[] files=Directory.GetFiles(FdGolbals.Job.Dir);
            foreach (string file in files) 
            {
                string lfile=file.ToLower();
                if (lfile.EndsWith(".png") || lfile.EndsWith(".bmp"))
                    fileList.AddFile(Path.GetFileName(file));
            }
            return fileList;
        }

        //--- IMG_Image_Clicked ---------------------------------------
        private void IMG_Image_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            FileList files=_update_files();
            FileOpenPopup.IsSaveAs = false;
            FileOpenPopup.Filter = "BMP files (*.bmp)|*.bmp|PNG files (*.png)|*.png|All files|*";
            FileOpenPopup.Title  = "Open image";
            FileOpenPopup.Files  = files;
            FileOpenPopup.IsOpen = true;
            FileOpenPopup.OnImport = _ImportBitmap;
            FileOpenPopup.OnSelected = (file)=> 
            {
                FdGolbals.JobSettings.Gripper.Name = file;
            };
            e.Handled = true;
        }

        //--- _ImportBitmap --------------------------------------
        private void _ImportBitmap(string path)
        {
            string dir     = Path.GetDirectoryName(path);
            string dstPath = FdGolbals.Job.Dir+Path.DirectorySeparatorChar+Path.GetFileName(path);
            try
            {
                File.Copy(path, dstPath, true);
                FdGolbals.JobSettings.Gripper.Name = Path.GetFileName(path);
            }
            catch (Exception) { };
        }


    }
}
