using Feedy.Models;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace GE_Utilities
{
    public class GeNumBoxMM : GeNumBox
    {
        private double _UnitsFactor;

        public Action OnValueChanged;

        private Object       _BindingCtrl;
        private PropertyInfo _BindingProperty;
        private bool         _Editing=false;

        public GeNumBoxMM()
        {
            FdGolbals.Settings.PropertyChanged += Settings_PropertyChanged;
            TextChanged += GeNumBoxMM_TextChanged;
            GotFocus     += GeNumBoxMM_GotFocus;
            LostFocus    += GeNumBoxMM_LostFocus;
        }

        //--- GeNumBoxMM_GotFocus -------------------------
        void GeNumBoxMM_GotFocus(object sender, RoutedEventArgs e)
        {
            _Editing = true;
        }

        //--- GeNumBox_LostFocus -------------------------
        void GeNumBoxMM_LostFocus(object sender, RoutedEventArgs e)
        {
            _Editing = false;
            GeNumBoxMM_TextChanged(this, null);
        }

        //--- Bind ---------------------------------------
        public void Bind(Object ctrl, string PropertyName)
        {
            try
            {
                _BindingCtrl = ctrl;
                _BindingProperty = ctrl.GetType().GetProperty(PropertyName);
                var val=_BindingProperty.GetValue(_BindingCtrl, null);
                Value = (double)System.Convert.ToDouble(val);

                OnValueChanged = ()=>
                {
                    _BindingProperty.SetValue(_BindingCtrl, Value);
                };
                (_BindingCtrl as INotifyPropertyChanged).PropertyChanged += _PropertyChanged;
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //--- _PropertyChanged ---------------------------
        private void _PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName==_BindingProperty.Name)
            {
                try
                {
                    var val=_BindingProperty.GetValue(_BindingCtrl, null);
                    Value = (double)System.Convert.ToDouble(val);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        
        //--- Property IsSpeed ---------------------------------------
        private bool _IsSpeed=false;
        public bool IsSpeed
        {
            get { return _IsSpeed; }
            set { _IsSpeed = value; }
        }

        //--- _SetUnitsFactor ------------------------------------
        private void _SetUnitsFactor()
        {
            if (IsSpeed)    
            {
                if (FdGolbals.Settings.Units==Settings.EUnits.inches)
                    _UnitsFactor = 1/3.28084;
                else
                    _UnitsFactor = 1;
            }
            else 
            {
                if (FdGolbals.Settings.Units==Settings.EUnits.inches)
                    _UnitsFactor = 25400.0;
                else
                    _UnitsFactor = 1000.0;
            }
        }

        //--- Settings_PropertyChanged --------------------------------------------
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="Units")
            {
                _ValueChanged = true;
                _SetUnitsFactor();
                Text = (Value / _UnitsFactor).ToString("#0.###");
            }
        }

        //--- Property Value ---------------------------------------
        private double? _Value;
        private bool _ValueChanged;
        public new double Value
        {
            set { 
                    if (_UnitsFactor==0) _SetUnitsFactor();
                    Text = (value / _UnitsFactor).ToString("#0.###");
                    _Value=value;
                    if (OnValueChanged!=null) 
                        OnValueChanged();
                }
            get { 
                    if(_Value==null) return Ge.StrToDouble(Text);
                    else return (double) _Value;
                }
        }

        //--- TextChanged ----------------------------------------
        private void GeNumBoxMM_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_Editing) return;

            if (_ValueChanged) 
               _ValueChanged=false;
            else if (!Text.EndsWith("."))
            {
                double val=Ge.StrToDouble(Text);
                if (_Value!=null)
                    Value = val*_UnitsFactor; // FdGolbals.Settings.UnitsFactor;
            }
        }
    }
}
