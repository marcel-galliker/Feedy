using GE_Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Feedy.Models
{
    public class Language : GeBindable
    {        
        public List<GeEnum<int>> Languages;
        
        public delegate void LanguageChagedDelegate();
        public LanguageChagedDelegate LanguageChanged;


        private bool _Loaded=false;
        private Dictionary<string, string[]> _Translation = new Dictionary<string, string[]>();
        private Dictionary<string, string[]> _Errors      = new Dictionary<string, string[]>();

        //--- constructor ----------------------------
        public Language()
        {
            Thread thread=new Thread(()=>
            {
                _load(_Translation, AppDomain.CurrentDomain.BaseDirectory+"Language_EZ-Editor.csv");
                _load(_Errors,      AppDomain.CurrentDomain.BaseDirectory+"Language_Errors.csv");
            });
            thread.Start();
        }

        //--- Property LanguageIdx ---------------------------------------
        private int _LanguageIdx=0;
        public int LanguageIdx
        {
            get { return _LanguageIdx; }
            set { if (SetProperty(ref _LanguageIdx, value))
                    {
                        if (LanguageChanged!=null)
                            LanguageChanged();
                    }
                }
        }

        //--- _load ----------------------------
        private void _load(Dictionary<string, string[]> translation, string path)
        {
            try
            { 
                using(TextReader file=File.OpenText(path))
                {
                    for(int row=0; true; row++)
                    {
                        string line=file.ReadLine();
                        if (line==null) break;
                        string[] data=line.Split(';');
                        if (row==0)
                        {
                            List<GeEnum<int>> list = new List<GeEnum<int>>();
                            for (int i = 3; i<data.Length; i++)
                            {
                                if (data[i].Length!=0)
                                    list.Add(new GeEnum<int>(i-3, data[i]));
                            }
                            Languages=list;
                        }
                        else
                        { 
                            string[] texts=new string[Languages.Count];
                            for (int i = 0; i<texts.Length; i++)
                            {
                                texts[i] = data[3+i];
                            }
                            translation.Add(data[2], texts);
                        }
                    }
                }
            }
            catch(Exception )
            {
            }
            _Loaded = true;
        }

        //--- GetText -----------------------
        public string GetText(string variable, string className=null)
        {
            if (className==null)
            {
                var methodInfo  = new StackTrace().GetFrame(1).GetMethod();
                className       = methodInfo.ReflectedType.FullName;
            }
            while (!_Loaded) Thread.Sleep(100);
            string key=className+'.'+variable;
            if (_Translation.ContainsKey(key)) 
            {
                var item= _Translation[key];
                if (_LanguageIdx<item.Length && item[_LanguageIdx].Length>0 && item[_LanguageIdx]!="???") return item[_LanguageIdx];
                else return item[0];
            }
            return key+"NF";
        }

        //--- SetText ---------
        public void SetText(TextBlock ctrl)
        {
            var methodInfo   = new StackTrace().GetFrame(1).GetMethod();
            string className = methodInfo.ReflectedType.FullName;
            ctrl.Text = FdGolbals.Language.GetText(ctrl.Name, className);
        }

        //--- GetError --------------------------------
        public string GetError(string fileName, string no, string org)
        {
            string key=fileName+'.'+no;
            if (_Errors.ContainsKey(key)) 
            {
                var item=_Errors[key];
                if (item[_LanguageIdx].Length>0) return item[_LanguageIdx];
                else                             return item[0];
            }
            FdGolbals.Events.AddLog(0, string.Format("Language Error:>>{0}.{1}<< not found", fileName, no));
            return org;

        }
    }
}
