using GE_Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Feedy.Models
{
    public class FileList : ObservableCollection<GeEnum<string>>
    {
        public FileList()
        {
        }

        public void AddFile(string filename)
        {
            GeEnum<string> newItem=new GeEnum<string>(filename, filename);
            for(int i=0;i<this.Count;i++)
            {
                switch(this[i].Value.CompareTo(filename))
                {
                    case -1: break;  // list.item smaller than filename
                    case  0: return; // equal
                    case  1:  GeBindable.Invoke(()=>this.Insert(i, newItem)); return;  
                }
            }
            GeBindable.Invoke(()=>this.Add(newItem));
        }
    }
}
