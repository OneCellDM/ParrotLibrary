using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParrotLibrary.DataModel
{
    public class ParrotItem
    {
        private string image = "";
        


        [JsonProperty("Gender", NullValueHandling = NullValueHandling.Ignore)]
        public string Gender { get; set; }

        [JsonProperty("Data", NullValueHandling = NullValueHandling.Ignore)]
        public string Data { get; set; }
        private string title;

        [JsonProperty("Title", NullValueHandling = NullValueHandling.Ignore)]

        public string Title { get=> title;  
            set{ 
            title= value;
                FN = value[0];
                
            } 
        }

        [JsonProperty("Image")]
        public string Image { get; set; }

        [JsonProperty("Images")]
        public string[] Images { get; set; }

        [JsonProperty("GenderInfo")]
        public string GenderInfo { get; set; }
        public string Path { get; set; }

        public char FN { get; set; }
        public string DefaultGroup { get; set; } = "Поиск";


    }
      
}
