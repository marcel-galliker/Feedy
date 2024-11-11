using GE_Utilities;
using System.Collections;
using System.Collections.Generic;


namespace Feedy.Models.Enums
{
    public class EN_Int: IEnumerable
    {
        private static List<GeEnum<int>> _List;

        //--- constructor --------------------------------
        public EN_Int(int from, int to, int step=1)
        {
            _List=new List<GeEnum<int>>();
            for (int i= from; i <= to; i+=step)
            {
                _List.Add(new GeEnum<int>(i, i.ToString()));
            }
        }

        //--- constructor ---------------------------------------
        public EN_Int(int[] list)
        {
            _List=new List<GeEnum<int>>();
            if (list == null) return;
            for (int i=0; i<list.Length; i++)
            {
                _List.Add(new GeEnum<int>(i, list[i].ToString()));
            }
        }


        //--- enumerator -----------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new GeListEnumerator<GeEnum<int>>(_List);
        }

    }
}
