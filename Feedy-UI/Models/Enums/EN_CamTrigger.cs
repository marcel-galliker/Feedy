using GE_Utilities;
using System.Collections;
using System.Collections.Generic;
using static Feedy.Models.Settings;
using static Feedy.Services.FdDef;

namespace Feedy.Models.Enums
{
    public class EN_CamTrigger: IEnumerable
    {
        private static List<GeEnum<EnCamTrigger>> _List;

        //--- _init_list ---------------------
        internal static List<GeEnum<EnCamTrigger>> _init_list()
        {
            List<GeEnum<EnCamTrigger>> list; 
            list = new List<GeEnum<EnCamTrigger>>();
            list.Add(new GeEnum<EnCamTrigger>(EnCamTrigger.CAM_early, "early"));
            list.Add(new GeEnum<EnCamTrigger>(EnCamTrigger.CAM_late, "late"));
            return list;
        }

        //--- constructor --------------------------------
        public EN_CamTrigger()
        {
            _List=_init_list();
        }

        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<EnCamTrigger>>(_List);
        }

    }
}