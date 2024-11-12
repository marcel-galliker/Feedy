using GE_Utilities;
using System.Collections;
using System.Collections.Generic;
using static Feedy.Models.Settings;
using static Feedy.Services.FdDef;

namespace Feedy.Models.Enums
{
    public class EN_GripperType: IEnumerable
    {
        private static List<GeEnum<EnGripperType>> _List;

        //--- _init_list ---------------------
        internal static List<GeEnum<EnGripperType>> _init_list()
        {
            List<GeEnum<EnGripperType>> list; 
            list = new List<GeEnum<EnGripperType>>();
            list.Add(new GeEnum<EnGripperType>(EnGripperType.GRP_Vacc,    "vacuum"));
            list.Add(new GeEnum<EnGripperType>(EnGripperType.GRP_Std,     "standard"));
            list.Add(new GeEnum<EnGripperType>(EnGripperType.GRP_Inverse, "inverse"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_GripperType()
        {
            _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<EnGripperType>>(_List);
        }

    }
}