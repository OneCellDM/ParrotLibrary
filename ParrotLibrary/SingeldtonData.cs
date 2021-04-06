using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParrotLibrary
{
    public static class SingeldtonData
    {
        public delegate void ArticleSelected(DataModel.ParrotItem parrotItem);
        public static event ArticleSelected ArticleSelectedEvents;
       

        public static void CallArticleSelectedEvent(DataModel.ParrotItem parrotItem)=>
            ArticleSelectedEvents?.Invoke(parrotItem);
        
    }
}
