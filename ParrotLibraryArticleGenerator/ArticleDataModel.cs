using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParrotLibraryArticleGenerator
{
    class ArticleDataModel
    {
      
        public int Family { get; set; }
        public int Gender { get; set; }
        public string Data { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string[] Images { get; set; }
    }
}
