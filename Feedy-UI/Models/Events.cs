using Feedy.Services;
using GE_Utilities;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Feedy.Models
{
    public class Events : GeBindable
    {
        public ObservableCollection<EventItem> List   = new ObservableCollection<EventItem>();
        private EventItem _TopItem=null;

        public Events()
        {
            FdGolbals.Language.LanguageChanged += _LanguageChanged;  
        }

        //--- _LanguageChanged -------------------------------------
        private void _LanguageChanged()
        {
            foreach(var item in List) 
            {
                item.Language();
            }
            if (_TopItem!=null) Message=_TopItem.Msg;
        }

        //--- Confirm ------------------------------
        public void Confirm()
        {
            GeBindable.Invoke(()=>
            {
                List.Clear();
                Type = ENEventType.undef;
                Message = null;
            });
        }

        //--- AddItem --------------------------------------
        public void AddItem(string msg)
        {
            EventItem item=new EventItem(msg);
            List.Insert(0, item);
            while (List.Count > 100)
            {
                List.RemoveAt(List.Count-1);
            }
            if (item.Type>Type) 
            {
                Type= item.Type;
                Message= item.Msg;
                _TopItem = item;
            }
            else if (Type==ENEventType.log && item.Type==ENEventType.log) 
            {
                _TopItem = item;
                Message  = item.Msg;
            }
            FdGolbals.MainWindow.LogView.AddItem(item);
        }

        //--- AddEvent -----------------
        public void AddEvent(EventItem item)
        {   
            List.Insert(0, item);
        }

        //--- AddLog -------------------------------
        public void AddLog(int no, string msg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            string[] list=filePath.Split('\\');
            StringBuilder str=new StringBuilder(512);
            str.Append(DateTime.Now.ToString("HH:MM:ss"));
            str.Append(";LOG;");
            if (list.Length>0) str.Append(list[list.Length-1]+';'+lineNumber.ToString()+';');
            else str.Append(";;");
            str.Append(no.ToString()+';'+msg+";");

            GeBindable.Invoke(()=>AddItem(str.ToString()));
        }
        //--- AddLog -------------------------------
        // simulate print-engine error
        public void AddLog(string pe_filename, int no, string msg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            StringBuilder str=new StringBuilder(512);
            str.Append(DateTime.Now.ToString("HH:MM:ss"));
            str.Append(";LOG;");
            str.Append(pe_filename+';'+lineNumber.ToString()+';');
            str.Append(no.ToString()+';'+msg+";");
            GeBindable.Invoke(()=>AddItem(str.ToString()));
        }

        //--- AddError -------------------------------
        public void AddError(int no, string msg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            string[] list=filePath.Split('\\');
            StringBuilder str=new StringBuilder(512);
            str.Append(DateTime.Now.ToString("HH:MM:ss"));
            str.Append(";ERR;");
            if (list.Length>0) str.Append(list[list.Length-1]+';'+lineNumber.ToString()+';');
            else str.Append(";;");
            str.Append(no.ToString()+';'+msg+";");

            GeBindable.Invoke(()=>AddItem(str.ToString()));
        }

        //--- AddError -------------------------------
        // simulate print-engine error
        public void AddError(string pe_filename, int no, string msg, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0)
        {
            StringBuilder str=new StringBuilder(512);
            str.Append(DateTime.Now.ToString("HH:MM:ss"));
            str.Append(";ERR;");
            str.Append(pe_filename+';'+lineNumber.ToString()+';');
            str.Append(no.ToString()+';'+msg+";");
            GeBindable.Invoke(()=>AddItem(str.ToString()));
        }

        //--- Property Type ---------------------------------------
        private ENEventType _Type=ENEventType.undef;
        public ENEventType Type
        {
            get { return _Type; }
            set { SetProperty(ref _Type, value);}
        }

        //--- Property Message ---------------------------------------
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

    }
}
