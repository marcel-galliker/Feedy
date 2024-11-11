using GE_Utilities;
using System.Collections;
using System.Collections.Generic;


namespace Feedy.Models.Enums
{
    public class EN_ScreenSize : IEnumerable
    {
        private static List<GeEnum<int>> _List;

        //--- _init_list ---------------------
        internal static List<GeEnum<int>> _init_list()
        {
            List<GeEnum<int>> list; 
            list = new List<GeEnum<int>>();
            list.Add(new GeEnum<int>(2560<<16 | 1440, "FULL SCREEN"));
            list.Add(new GeEnum<int>(1920<<16 | 1200, "1920 x 1200"));
            list.Add(new GeEnum<int>(1600<<16 | 1200, "1600 x 1200"));
            list.Add(new GeEnum<int>(1280<<16 | 1024, "1280 x 1024"));
            list.Add(new GeEnum<int>(1280<<16 |  800, "1280 x  800"));
            list.Add(new GeEnum<int>(1024<<16 |  768, "1024 x  768"));
            list.Add(new GeEnum<int>( 800<<16 |  600, " 800 x  600"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_ScreenSize()
        {
            if (_List==null) _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<int>>(_List);
        }

    }
}
