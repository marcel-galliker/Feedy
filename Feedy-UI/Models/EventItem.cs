using GE_Utilities;
using System.Linq;

namespace Feedy.Models
{

    public enum ENEventType 
    {
        undef,
        log,
        warn,
        err
    }

    public class EventItem : GeBindable
    {
        public EventItem()
        {

        }

        //--- Constructor -----------------------------------
        private string _Message;
        public EventItem(string msg)
        {
            _Message = msg;
            string[] list=_Message.Split(';');
            if (list.Length>5)
            {
                Time = list[0];
                if (list[1].Equals("ERR"))       Type=ENEventType.err;
                else if (list[1].Equals("WARN")) Type=ENEventType.warn;
                else                             Type=ENEventType.log;

                FileLine = list[2]+':'+list[3];
                ErrNo=  Ge.StrToInt32(list[4]);
            }
            Language();
        }

        //--- _argcnt -------------------
        private int _argcnt(string str)
        {
            int cnt = 0;
            foreach(char ch in str)
                if (ch=='{') cnt++;
            return cnt;
        }

        //--- Language ---------------------------------------------------
        public void Language()
        {
            string[] list=_Message.Split(';');
            if (list.Length>5)
            {
                string formatStr;
                if (ErrNo==0) formatStr = list[5];
                else formatStr = FdGolbals.Language.GetError(list[2], list[4], list[5]);
                int arcCnt=_argcnt(formatStr);
                if (list.Count()>=arcCnt+6)
                {
                    switch (arcCnt)
                    {
                        case  0: Msg = formatStr; break;
                        case  1: Msg = string.Format(formatStr, list[6]); break;
                        case  2: Msg = string.Format(formatStr, list[6], list[7]); break;
                        case  3: Msg = string.Format(formatStr, list[6], list[7], list[8]); break;
                        case  4: Msg = string.Format(formatStr, list[6], list[7], list[8], list[9]); break;
                        case  5: Msg = string.Format(formatStr, list[6], list[7], list[8], list[9], list[10]); break;
                        case  6: Msg = string.Format(formatStr, list[6], list[7], list[8], list[9], list[10], list[11]); break;
                        default: Msg = string.Format(formatStr, list[6], list[7], list[8], list[9], list[10], list[11], list[12]); break;
                    }
                }
            }
        }

        //--- Property Type ---------------------------------------
        private ENEventType _Type;
        public ENEventType Type
        {
            get { return _Type; }
            set { SetProperty(ref _Type, value); }
        }

        //--- Property Date ---------------------------------------
        private string _Date;
        public string Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }

        //--- Property Time ---------------------------------------
        private string _Time;
        public string Time
        {
            get { return _Time; }
            set { 
                    string[] str=value.Split('@');
                    if (str.Length>1)
                    {
                        Date= str[0];
                        SetProperty(ref _Time, str[1]); 
                    }
                    else
                    {
                        SetProperty(ref _Time, value); 
                    }
                }
        }

        //--- Property FileLine ---------------------------------------
        private string _FileLine;
        public string FileLine
        {
            get { return _FileLine; }
            set { SetProperty(ref _FileLine, value); }
        }

        //--- Property ErrNo ---------------------------------------
        private int _ErrNo;
        public int ErrNo
        {
            get { return _ErrNo; }
            set { SetProperty(ref _ErrNo, value); }
        }

        //--- Property Msg ---------------------------------------
        private string _Msg;
        public string Msg
        {
            get { return _Msg; }
            set { SetProperty(ref _Msg, value); }
        }

    }
}
