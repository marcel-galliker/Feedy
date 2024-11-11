﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GE_Utilities
{
    /*
    public interface IBindableBase : INotifyPropertyChanged
    {
        internal bool SetProperty<type>(ref type property, type val, [CallerMemberName]String propName = null);
    }
    */

    public class GeBindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        //--- Property Dispatcher ---------------------------------------
        public static Dispatcher _Dispatcher;
        public static Dispatcher Dispatcher
        {
            set { _Dispatcher= value; }
            get { return _Dispatcher; }
        }

        //--- Invoke -------------------------
        public static void Invoke(Action action)
        {
            if (action==null) return;
            if (GeBindable._Dispatcher != null)
            {
                try
                {
                    GeBindable._Dispatcher.Invoke(action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try { action(); }
                catch {  };
            }
        }

        //--- InvokeDelayed -------------------------
        public static void InvokeDelayed(int ms, Action action)
        {
            if (action==null) return;
            Task.Run(()=>
            {
                Thread.Sleep(ms);
                GeBindable.Invoke(action);
            });
        }

        //--- SetProperty -------------------------------------------------------
        public bool SetProperty<type>(ref type property, type val, [CallerMemberName]String propName = null)
        {
            if (property == null || !property.Equals(val))
            {
                property = val;
                if (PropertyChanged != null)
                    Invoke(()=>PropertyChanged(this, new PropertyChangedEventArgs(propName)));
                return true;
            }
            return false;
        }

        //--- OnPropertyChanged --------------------------------------------------------------
        public void OnPropertyChanged([CallerMemberName]String info = null)
        {
            if (PropertyChanged != null)
                Invoke(()=>PropertyChanged(this, new PropertyChangedEventArgs(info)));
        }

        //--- SignalPropertyChanged --------------------------------------------------------------
        public void SignalPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        //--- AllPropertiesChanged --------------------------------------
        public void AllPropertiesChanged(object obj)
        {
            if (PropertyChanged != null)
            {
                Invoke(()=>
                    {
                        foreach(var prop in obj.GetType().GetProperties())
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs(prop.Name));
                        }   
                    }
                  );
            }
        }
    }
}