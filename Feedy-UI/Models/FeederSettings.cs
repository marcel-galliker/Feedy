using GE_Utilities;
using static Feedy.Services.feedy_def;

namespace Feedy.Models
{
    public class FeederSettings : GeBindable
    {
        //--- Set ----------------------------------------
        public void Set(SFeeder fdr)
        {
            IsUsed = fdr.used!=0;
            Speed = fdr.speed;
            Slope = fdr.slope;
            Turns = fdr.turns;
            MaxTurns = fdr.maxTurns;
        }

        //--- Get -----------------------------------
        public SFeeder Get()
        {
            SFeeder fdr = new SFeeder();
            fdr.used = IsUsed? 1:0;
            fdr.speed = Speed;
            fdr.slope = Turns;
            fdr.turns = Turns;
            fdr.maxTurns = MaxTurns;
            return fdr;
        }
        
        //--- Property IsChanged ---------------------------------------
        private bool _IsChanged=false;
        public bool IsChanged
        {
            get { return _IsChanged; }
            set { 
                   // if(IsLoading) SetProperty(ref _IsChanged, false);
                   // else 
                    {
                        SetProperty(ref _IsChanged, value);
                    }
                }
        }

        //--- Property IsUsed ---------------------------------------
        private bool _IsUsed;
        public bool IsUsed
        {
            get { return _IsUsed; }
            set { SetProperty(ref _IsUsed, value); }
        }

        //--- Property Speed ---------------------------------------
        private int _Speed;
        public int Speed
        {
            get { return _Speed; }
            set { SetProperty(ref _Speed, value); }
        }

        //--- Property Slope ---------------------------------------
        private int _Slope;
        public int Slope
        {
            get { return _Slope; }
            set { SetProperty(ref _Slope, value); }
        }

        //--- Property Turns ---------------------------------------
        private int _Turns;
        public int Turns
        {
            get { return _Turns; }
            set { SetProperty(ref _Turns, value); }
        }

        //--- Property MaxTurns ---------------------------------------
        private int _MaxTurns;
        public int MaxTurns
        {
            get { return _MaxTurns; }
            set { SetProperty(ref _MaxTurns, value); }
        }
    }
}
