using System;
using Newtonsoft.Json;
using SQLite;

namespace X
{
    public class Emojis
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(500)]

        public int emoji_ID { get; set; }
        public string rating_category_id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int status { get; set; }
        public int position { get; set; }


    }

   
}
