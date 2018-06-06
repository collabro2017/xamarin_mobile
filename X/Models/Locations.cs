using System;
using Xamarin.Forms;

namespace X.Models
{
    public class Locations
    {
        public string ID { get; set; }
        public FormattedString NameAndAddress { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string rating { get; set; }

    }
}
