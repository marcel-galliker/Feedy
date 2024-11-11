using Feedy.Models;
using GE_Utilities;
using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;


namespace Feedy.Views.LogView
{

    /// <summary>
    /// Interaction logic for EventsView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        private Events _Log = new Events();
        private string _Date;

        public LogView()
        {
            InitializeComponent();
            DataContext = this;
          //  EventsGrid.ItemsSource = _Log.List;
            TB_Log.Text = "Log";
            BN_Export.IsEnabled = false;

            FdGolbals.Language.LanguageChanged   += _Language;
            _Language();
        }
               
        //--- _Language ----------------------------------------------
        private void _Language()
        {
            FdGolbals.Language.SetText(TB_Log);
            Col_Time.Header = FdGolbals.Language.GetText("Col_Time");
            Col_Error.Header = FdGolbals.Language.GetText("Col_Error");
            Col_Message.Header = FdGolbals.Language.GetText("Col_Message");
            Col_Info.Header = FdGolbals.Language.GetText("Col_Info");
        }

        //--- _update_files -------------------------
        private FileList _Files()
        {
            FileList list= new FileList();
            string dir =  FdGolbals.Dir.Logs;
            string[] files = Directory.GetFiles(dir);
            foreach(string file in files)
            {
                if (Path.GetExtension(file)==".csv")
                    list.Add(new GeEnum<string>(Path.GetFileName(file), Path.GetFileName(file)));
            }
            return list;
        }

        //--- LoadLog ----------------------------
        private void LoadLog(string filename)
        {
            _Log.Confirm();
            string path =  FdGolbals.Dir.Logs + filename;
            if (!File.Exists(path)) return;
            TB_Log.Text=filename;
            EventsGrid.ItemsSource = null;
            new Thread(()=>
                {
                    try 
                    { 
                        int cnt=0;
                        var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using (StreamReader reader = new StreamReader(file))
                        {
                            bool header=true;
                            string line = String.Empty;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (header) header=false;
                                else  GeBindable.Invoke(()=>_Log.AddItem(line));     
                                cnt++;
                                /*
                                if (cnt%100==0)
                                {
                                    Thread.Sleep(10);
                                    Console.WriteLine("{0} Lines", cnt);
                                }
                                */
                              //  if (cnt==100) 
                              //      break;
                            }
                        }
                    }
                    catch(Exception e)
                    { Console.WriteLine(e.Message);}
                    GeBindable.Invoke(()=>
                    {
                        EventsGrid.ItemsSource = _Log.List;
                        BN_Export.IsEnabled = true;
                    }
                    );
                 }).Start();
        }

        //--- AddItem ------------------------------------
        public void AddItem(EventItem item)
        {
            if (item.Date==null) return;

            string lastdate=_Date;
            _Date = item.Date;
            if (lastdate==null)
            {
                LoadLog(item.Date+".csv");
            }
            else if (lastdate==item.Date)
            {
                _Log.AddEvent(item);
            }
        }

        //--- Log_Clicked -------------------------------------
        private void Log_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            FileList files= _Files();
            FileOpenPopup.IsSaveAs = false;
            FileOpenPopup.Title  ="Open Log";
            FileOpenPopup.Files  = files;
            FileOpenPopup.IsOpen = true;
            FileOpenPopup.OnSave = LoadLog;
        }

        //--- Export_Clicked -------------------------------------
        private void Export_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog ofd = new System.Windows.Forms.SaveFileDialog();
            ofd.Filter = "CSV Flies(*.csv)|*.csv";
            ofd.RestoreDirectory = true;
            ofd.Title = ("Export File");

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                new Thread(()=>
                {
                    try
                    {
                        using (StreamWriter writer = File.CreateText(ofd.FileName))
                        {
                            writer.WriteLine("Time;Type;ErrNo;Message;Info");

                            foreach(EventItem item in _Log.List)
                            {
                                string evt=String.Format("{0};{1};{2};{3};{4}", 
                                    item.Time, item.Type, item.ErrNo, item.Msg, item.FileLine);
                                writer.WriteLine(evt);
                            }
                        }
                    }
                    catch(Exception ex)
                    { Console.WriteLine(ex.Message);}
                }).Start();
            }         
        }
    }
}
