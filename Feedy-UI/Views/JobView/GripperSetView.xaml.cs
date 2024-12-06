using Feedy.Models;
using Feedy.Models.Enums;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

            IMG_Gripper.Source = new BitmapImage(new Uri("D:\\Feedy\\Jobs\\Job1\\Greifer B.png"));
        }

        //--- _Language ----------------------------------------------
        private void _Language()
        {
        }

    }
}
