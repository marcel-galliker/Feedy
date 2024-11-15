using Feedy.Models;
using Feedy.Services;
using GE_Utilities;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Feedy.Services.feedy_def;

namespace Feedy.Views.Main
{
    /// <summary>
    /// Interaction logic for TopBar.xaml
    /// </summary>
    public partial class TopBar : UserControl
    {

        private System.Windows.Shapes.Rectangle[] _Inputs = new System.Windows.Shapes.Rectangle[4];
        private System.Windows.Shapes.Rectangle[] _Outputs= new System.Windows.Shapes.Rectangle[4];

        public TopBar()
        {
            InitializeComponent();

            DataContext = FdGolbals.JobSettings;
            TEST.DataContext = FdGolbals.JobSettings;
            FileCommands.DataContext = FdGolbals.Job;
            TX_Remaining.DataContext = FdGolbals.Job;
            Grid_Speed.DataContext   = FdGolbals.Job;

            _create_io();
            _update_io();
            _show_progress();
            BTN_IO_Test.Visibility = Visibility.Collapsed;
        }

        //--- _show_progress -----------------------------------------------------------
        private void _show_progress()
        {
            Visibility visible = Visibility.Visible;

            UC_Progress.Visibility    = visible;
            TX_Remaining.Visibility   = visible;
        }

        //--- _HeadType_Changed ----------------------------------------------
        private void _HeadType_Changed()
        {
            BTN_IO_Test.Visibility = Visibility.Visible;
            _show_progress();
        }

        //--- Expander_Expanded ------------------------------
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (FdGolbals.Menu!=null) FdGolbals.Menu.IsExpanded=true;
        }

        //--- Expander_Collapsed ---------------------------------------
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (FdGolbals.Menu!=null) FdGolbals.Menu.IsExpanded = false;
        }

        //--- Start_Clicked ----------------------------------------
        private int _LastClickTime=0;
        private void Start_Clicked(object sender, RoutedEventArgs e)
        {
       //     BTN_Run.IsBusy=true;
            if (FdGolbals.FeedyStatus.JobState>EnJobState.JS_off)
            {
                FdGolbals.Job.Stop();
            }
            else
            { 
                if (Environment.TickCount-_LastClickTime<1000) return; // filter double clicks
                {
                    IO_Test.IsOpen = false;
                    FdGolbals.Job.Save();
                    FdGolbals.Job.Start();
                }
            }
            _LastClickTime = Environment.TickCount;
        }

        //--- _update_files -------------------------
        private void _update_files(Action action)
        {
            Thread thread = new Thread(()=>
            {
                int cnt=FdGolbals.Job.ListLoaded;
                FdGolbals.Job.LoadList();
                while (FdGolbals.Job.ListLoaded==cnt)
                    Thread.Sleep(10);
                GeBindable.Invoke(action);
            });            
            thread.Start();
        }

        //--- Open_Clicked ---------------------------------------
        private void Open_Clicked(object sender, RoutedEventArgs e)
        {
            _update_files(()=>FileOpenPopup.Files  = FdGolbals.Job.Files);
            FileOpenPopup.IsSaveAs = false;
            FileOpenPopup.Title    = "Open job";
            FileOpenPopup.Filter   = "XML files (*.xml)|*.xml";
            FileOpenPopup.Files    = FdGolbals.Job.Files;
            FileOpenPopup.IsOpen   = true;
            FileOpenPopup.OnSelected   = FdGolbals.Job.Load;
            FileOpenPopup.OnImport = _ImportJob;
            FileOpenPopup.SelectFolder=true;
            JobCmdPopup.IsOpen   = false;
        }

        //--- _ImportJob --------------------------------
        private void _ImportJob(string srcPath)
        {
            Console.WriteLine("ImportJob {0}", srcPath);
            string[] part=srcPath.Split(Path.DirectorySeparatorChar);
            if (part.Length>0)
            {
                string jobName=part[part.Length-1];
                string dstPath = FdGolbals.Dir.Jobs+jobName;
                if (Directory.Exists(dstPath))
                {
                    foreach(var file in Directory.GetFiles(dstPath))
                    {
                        try { File.Delete(file);}
                        catch(Exception){}
                    }
                }
                else 
                {
                    try { Directory.CreateDirectory(dstPath);}
                    catch(Exception) { return;}
                }

                dstPath = dstPath +Path.DirectorySeparatorChar;
                foreach(var file in Directory.GetFiles(srcPath))
                {
                    if (!file.Contains("done"))
                    {
                        try {File.Copy(file, dstPath+Path.GetFileName(file), true); }
                        catch (Exception) { return;}
                    }
                }

                 FdGolbals.Job.Load(jobName);
            }
        }

        //--- New_Clicked -----------------------------------------------
        private void New_Clicked(object sender, RoutedEventArgs e)
        {
            _update_files(()=>FileOpenPopup.Files  = FdGolbals.Job.Files);

            FileOpenPopup.IsSaveAs = true;
            FileOpenPopup.Title  = "New job";
            FileOpenPopup.Files  = FdGolbals.Job.Files;
            FileOpenPopup.OnSave = FdGolbals.Job.SaveNew;
            FileOpenPopup.IsOpen = true;
            JobCmdPopup.IsOpen   = false;
        }

        //--- Save_Clicked ---------------------------------------
        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            FdGolbals.Cfg.Save();
            FdGolbals.Job.Save();
            JobCmdPopup.IsOpen   = false;
        }

        //--- Reload_Clicked ---------------------------
        private void Reload_Clicked(object sender, RoutedEventArgs e)
        {
            FdGolbals.Job.Load();
            JobCmdPopup.IsOpen   = false;
        }

        //--- SaveAs_Clicked -----------------------------------------------
        private void SaveAs_Clicked(object sender, RoutedEventArgs e)
        {
            _update_files(()=>FileOpenPopup.Files  = FdGolbals.Job.Files);

            FileOpenPopup.IsSaveAs = true;
            FileOpenPopup.Title  = "Save job as";
            FileOpenPopup.Files  = FdGolbals.Job.Files;
            FileOpenPopup.OnSave = FdGolbals.Job.SaveAs;
            FileOpenPopup.IsOpen = true;
            JobCmdPopup.IsOpen   = false;
        }

        //--- Delete_Clicked -----------------------------------------------
        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            _update_files(()=>FileOpenPopup.Files  = FdGolbals.Job.Files);

            FileOpenPopup.IsSaveAs = false;
            FileOpenPopup.Title    = "Delete Job";
            FileOpenPopup.Files    = FdGolbals.Job.Files;
            FileOpenPopup.OnDelete = FdGolbals.Job.Delete;
            FileOpenPopup.IsOpen   = true;
            JobCmdPopup.IsOpen     = false;
        }

        //--- Job_Clicked ----------------------------
        private void Job_Clicked(object sender, RoutedEventArgs e)
        {
            if (FdGolbals.FeedyStatus.IsRunning) 
            {
                TB_JobBtnDisabled_Text.Text="Disabled while running";
                TB_JobBtnDisabled.Visibility= Visibility.Visible;
                GeBindable.InvokeDelayed(2000, ()=>TB_JobBtnDisabled.Visibility= Visibility.Collapsed);
                return;
            }
            if (!FileOpenPopup.IsOpen)
            {

            //    JobCmdPopup.HorizontalOffset = BTN_Job.ActualWidth-JobCmdPopup.ActualWidth;
                JobCmdPopup.IsOpen = !JobCmdPopup.IsOpen;
                Visibility visible = (FdGolbals.User.Type==EN_UserType.USER_operator) ? Visibility.Collapsed : Visibility.Visible;
                CB_Delete.Visibility= visible;
            }
        }

        //--- _create_io --------------------
        private void _create_io()
        {
            Grid_IO.RowDefinitions.Add(new RowDefinition { Height=new GridLength(1, GridUnitType.Star)});
            Grid_IO.RowDefinitions.Add(new RowDefinition { Height=new GridLength(1, GridUnitType.Star)});

            for (int i=0; i<4; i++)
            {
                Grid_IO.ColumnDefinitions.Add(new ColumnDefinition { Width=new GridLength(12)});

                System.Windows.Shapes.Rectangle rect= new System.Windows.Shapes.Rectangle();
                rect.Width=10;
                rect.Height=10;
                rect.RadiusX=5;
                rect.RadiusY=5;
                rect.Stroke = Brushes.DarkGray;
                rect.StrokeThickness = 1;
                rect.VerticalAlignment = VerticalAlignment.Center;
                rect.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(rect, i);
                Grid.SetRow(rect, 0);
                _Inputs[i] = rect;
                Grid_IO.Children.Add(_Inputs[i]);

                rect= new System.Windows.Shapes.Rectangle();
                rect.Width=10;
                rect.Height=10;
                rect.RadiusX=5;
                rect.RadiusY=5;
                rect.Stroke = Brushes.DarkGray;
                rect.StrokeThickness = 1;
                rect.Fill = Brushes.LightGreen;
                rect.VerticalAlignment = VerticalAlignment.Center;
                rect.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(rect, i);
                Grid.SetRow(rect, 1);
                _Outputs[i] = rect;
                Grid_IO.Children.Add(_Outputs[i]);
            }
        }

        //--- _update_io ---------------------------------------
        private void _update_io()
        {
            /*
            for (int i=0; i<4; i++)
            {
                _Inputs[i].Fill  = ((FdGolbals.HyperBoard.Input  & (1<<i))==0)? Brushes.Transparent : Brushes.Yellow;
                _Outputs[i].Fill = ((FdGolbals.HyperBoard.Output & (1<<i))==0)? Brushes.Transparent : Brushes.LightGreen;
            }
            */
        }

        //--- IO_Clicked ----------------------------
        private void IO_Clicked(object sender, RoutedEventArgs e)
        {
            if (FdGolbals.FeedyStatus.IsRunning) 
            {
                TB_JobBtnDisabled_Text.Text="Disabled while running";
                TB_JobBtnDisabled.Visibility = Visibility.Visible;
                GeBindable.InvokeDelayed(2000, ()=>TB_JobBtnDisabled.Visibility= Visibility.Collapsed);
                return;
            }

            if (FdGolbals.User.Type!=EN_UserType.USER_service)
            {
                TB_JobBtnDisabled_Text.Text="Disabled for user";
                TB_JobBtnDisabled.Visibility = Visibility.Visible;
                IO_Test.IsOpen = false;
                GeBindable.InvokeDelayed(2000, ()=>TB_JobBtnDisabled.Visibility= Visibility.Collapsed);
                return;
            }

            IO_Test.IsOpen = !IO_Test.IsOpen;
        }
    }
}
