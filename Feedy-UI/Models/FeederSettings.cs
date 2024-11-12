using GE_Utilities;
using static Feedy.Services.FdDef;

namespace Feedy.Models
{
    public class FeederSettings : GeBindable
    {
        //--- Set ----------------------------------------
        public void Set(SFeeder bin)
        {
            IsUsed = bin.used!=0;
            Speed = bin.speed;
            Slope = bin.slope;
            Turns = bin.turns;
            MaxTurns = bin.maxTurns;
        }

        //--- Get -----------------------------------
        public SFeeder Get()
        {
            SFeeder bin = new SFeeder();
            bin.used = IsUsed? 1:0;
            bin.speed = Speed;
            bin.slope = Turns;
            bin.turns = Turns;
            bin.maxTurns = MaxTurns;
            return bin;
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
