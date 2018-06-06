using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace X.Models
{
    public class PinModel : Pin
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public List<string> Emojis { get; set; }

        public string pin { get; set; }
    }
}
