using GE_Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Feedy.Models
{
    public class Settings : GeBindable
    {
        private static string _AppName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        private static string _AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                     + Path.DirectorySeparatorChar 
                                     + _AppName 
                                     + Path.DirectorySeparatorChar;
        
        private static string _Filename = "Settings.xml";
        private static Dictionary<string, int> _Monitors = new Dictionary<string, int>();

        //--- EUnits ---------------------------------------------
        public enum EUnits
        {
            metric = 0,
            inches = 1,
        }

        //--- Constructor --------------------------------------
        public Settings()
        {
            IpAddress = "localhost";
            Load();
        }

        //--- Save ------------------------------------------------------
        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings() 
                            { 
                                Indent = true,
                                IndentChars = "\t",
                            };


            if (!Directory.Exists(_AppDataDir))
                Directory.CreateDirectory(_AppDataDir);

            XmlWriter xml= XmlWriter.Create(_AppDataDir+_Filename, settings);
            
            xml.WriteStartDocument();
            _SaveXmlSettings(xml);
            xml.Close();
        }

        //--- Load -------------------------------------------------------
        public void Load()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            try
            {
                Directory.CreateDirectory(_AppDataDir);
                XmlReader xml = XmlReader.Create(_AppDataDir+_Filename, settings);
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine("Start Element >>{0}<<", xml.Name);
                            if (xml.Name.Equals("Settings")) _LoadXmlSettings(xml);
                            break;
                        case XmlNodeType.Text: Console.WriteLine("Text Node: >>{0}<<", xml.Value); break;
                        case XmlNodeType.EndElement: Console.WriteLine("End Element >>{0}<<", xml.Name); break;
                        case XmlNodeType.Whitespace: break;
                        default:
                            Console.WriteLine("Other node >>{0}<< with value >>{1}<<", xml.NodeType, xml.Value);
                            break;
                    }
                }
                xml.Close();
            }
            catch(Exception)
            { }
        }

        //--- _LoadXmlSettings ----------------------------------
        private void _SaveXmlSettings(XmlWriter xml)
        {
            xml.WriteStartElement("Settings");
            {
                if (_Monitors.Count==0) _Monitors.Add(GeScreen.MonitorName(), ScreenWidth);
                foreach(var monitor in _Monitors)
                {
                    xml.WriteStartElement("Monitor");
                    {
                        xml.WriteAttributeString("Name",         monitor.Key);
                        xml.WriteAttributeString("ScreenWidth",  monitor.Value.ToString());
                    }
                    xml.WriteEndElement();
                }
                xml.WriteStartElement("Settings");
                { 
                    xml.WriteAttributeString("IpAddress",       IpAddress);
                    xml.WriteAttributeString("RemoveFiles",     RemoveFiles.ToString());
                    xml.WriteAttributeString("Language",        Language.ToString());
                    xml.WriteAttributeString("Units",           Units.ToString());
                }
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }

        //--- _LoadXmlSettings --------------------------------------
        private void _LoadXmlSettings(XmlReader xml)
        {
            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                        Console.WriteLine("Start Element >>{0}<<", xml.Name);
                        if (xml.Name.Equals("Monitor"))
                        {
                           string name=xml.GetAttribute("Name");
                           int    val =Ge.StrToInt32(xml.GetAttribute("ScreenWidth"));
                            _Monitors.Add(name, val);
                        }
                        if (xml.Name.Equals("Settings"))
                        {
                            string monitor = GeScreen.MonitorName();
                            if (_Monitors.ContainsKey(monitor)) ScreenWidth = _Monitors[monitor];
                            else ScreenWidth = 300;
                            if (FdGolbals.FdArgs.IpAddr==null)
                                IpAddress    = xml.GetAttribute("IpAddress");
                            else
                                IpAddress    = FdGolbals.FdArgs.IpAddr;
                            RemoveFiles  = Ge.StrToBool(xml.GetAttribute("RemoveFiles"));
                            Language     = Ge.StrToInt32(xml.GetAttribute("Language"));
                            string test=xml.GetAttribute("Units");
                            Enum.TryParse(xml.GetAttribute("Units"), out EUnits value);
                            Units = value;
                        }
                        break;
                    case XmlNodeType.Text: Console.WriteLine("Text Node: >>{0}<<", xml.Value); break;
                    case XmlNodeType.EndElement: Console.WriteLine("End Element >>{0}<<", xml.Name); break;
                    case XmlNodeType.Whitespace: break;
                    default:
                        Console.WriteLine("Other node >>{0}<< with value >>{1}<<", xml.NodeType, xml.Value);
                        break;
                }
            }
        }

        //--- Property RemoveFiles ---------------------------------------
        private bool _RemoveFiles;
        public bool RemoveFiles
        {
            get { return _RemoveFiles; }
            set { SetProperty(ref _RemoveFiles, value); }
        }

        //--- Property ScreenWidth ---------------------------------------
        private int _ScreenWidth=300;
        public int ScreenWidth
        {
            get { return _ScreenWidth; }
            set { 
                    if (SetProperty(ref _ScreenWidth, value))
                    {
                        string monitor = GeScreen.MonitorName();
                        if (_Monitors.ContainsKey(monitor)) _Monitors[monitor] = value;
                        else _Monitors.Add(monitor, value);
                    }; 
                }
        }

        //--- Property Language ---------------------------------------
        public int Language
        {
            get { return FdGolbals.Language.LanguageIdx; }
            set { FdGolbals.Language.LanguageIdx = value; }
        }

        //--- Property Units ---------------------------------------
        private EUnits _Units;
        public EUnits Units
        {
            get { return _Units; }
            set { 
                    if (_Units != value)
                    {
                    //    UnitsFactor = (value==EUnits.inches)? 25400.0 : 1000.0;
                        SetProperty(ref _Units, value);
                    }     
                }
        }

        /*
        //--- Property UnitsFactor ---------------------------------------
        private double _UnitsFactor=1000;
        public double UnitsFactor
        {
            get { return _UnitsFactor; }
            private set { SetProperty(ref _UnitsFactor, value); }
        }
        */

        //--- Property IpAddress ---------------------------------------
        private string _IpAddress;
        public string IpAddress
        {
            get { return _IpAddress; }
            set { SetProperty(ref _IpAddress, value);}                
        }

    }
}
