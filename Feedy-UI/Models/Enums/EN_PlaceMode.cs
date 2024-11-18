using GE_Utilities;
using System.Collections;
using System.Collections.Generic;
using static Feedy.Services.feedy_def;

namespace Feedy.Models.Enums
{
    public class EN_PlaceMode: IEnumerable
    {
        private static List<GeEnum<EnPlaceMode>> _List;

        //--- _init_list ---------------------
        public static List<GeEnum<EnPlaceMode>> _init_list()
        {
            List<GeEnum<EnPlaceMode>> list; 
            list = new List<GeEnum<EnPlaceMode>>();
            list.Add(new GeEnum<EnPlaceMode>(EnPlaceMode.PLACE_Z,  "Z-Axes"));
            list.Add(new GeEnum<EnPlaceMode>(EnPlaceMode.PLACE_X,  "X-Axes"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_PlaceMode()
        {
            _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<EnPlaceMode>>(_List);
        }

    }
}