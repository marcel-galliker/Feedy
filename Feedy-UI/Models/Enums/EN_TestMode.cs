using GE_Utilities;
using System.Collections;
using System.Collections.Generic;
using static Feedy.Services.feedy_def;

namespace Feedy.Models.Enums
{
    public class EN_TestMode: IEnumerable
    {
        private static List<GeEnum<EnTestMode>> _List;

        //--- _init_list ---------------------
        public static List<GeEnum<EnTestMode>> _init_list()
        {
            List<GeEnum<EnTestMode>> list; 
            list = new List<GeEnum<EnTestMode>>();
            list.Add(new GeEnum<EnTestMode>(EnTestMode.TEST_Off,  "OFF"));
            list.Add(new GeEnum<EnTestMode>(EnTestMode.TEST_FLY,  "FLY"));
            list.Add(new GeEnum<EnTestMode>(EnTestMode.TEST_STOP, "STOP"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_TestMode()
        {
            _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<EnTestMode>>(_List);
        }

    }
}