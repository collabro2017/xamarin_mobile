using System.Collections.Generic;
using Newtonsoft.Json;

namespace X
{
    public class User
    {   
     

        [JsonProperty("user")]
        public data data { get; set; }
    }

    public class data
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("photo")]
        public photo photo { get; set; }
        [JsonProperty("first_name")]
        public string first_name { get; set; }
        [JsonProperty("last_name")]
        public string last_name { get; set; }
        [JsonProperty("user_setting")]
        public settings settings { get; set; }
        [JsonProperty("scuttle_points")]
        public string points { get; set; }

    }

    public class photo
    {   
        
        [JsonProperty("url")]
        public string url { get; set; }

    }

    public class settings 
    {
        [JsonProperty("ratings_order")]
        public List<string> ratings_order { get; set; }
        [JsonProperty("location_notification")]
        public bool location_notification { get; set; }
        [JsonProperty("trend_notification")]
        public bool trend_notification { get; set; }

    }

}