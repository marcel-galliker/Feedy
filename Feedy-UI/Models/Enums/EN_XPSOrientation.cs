using GE_Utilities;
using System.Collections;
using System.Collections.Generic;


namespace EZ_Editor.Models.Enums
{
   public class EN_XPSOrientation: IEnumerable
    {
        private static List<GeEnum<bool>> _List;

        //--- _init_list ---------------------
        internal static List<GeEnum<bool>> _init_list()
        {
            List<GeEnum<bool>> list; 
            list = new List<GeEnum<bool>>();
            list.Add(new GeEnum<bool>(false, "Portrait"));
            list.Add(new GeEnum<bool>(true, "Landscape"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_XPSOrientation()
        {
            if (_List==null) _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<bool>>(_List);
        }

    }
}
