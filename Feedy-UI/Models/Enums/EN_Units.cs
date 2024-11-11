﻿using GE_Utilities;
using System.Collections;
using System.Collections.Generic;
using static Feedy.Models.Settings;

namespace Feedy.Models.Enums
{
    public class EN_Units: IEnumerable
    {
        private static List<GeEnum<EUnits>> _List;

        //--- _init_list ---------------------
        internal static List<GeEnum<EUnits>> _init_list()
        {
            List<GeEnum<EUnits>> list; 
            list = new List<GeEnum<EUnits>>();
            list.Add(new GeEnum<EUnits>(EUnits.metric, "mm"));
            list.Add(new GeEnum<EUnits>(EUnits.inches, "inch"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_Units()
        {
            _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<EUnits>>(_List);
        }

    }
}