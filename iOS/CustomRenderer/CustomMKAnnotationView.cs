using System.Collections.Generic;
using MapKit;

namespace X.iOS
{
    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public List<string> emojis { get; set; }

        public string pin { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id,List<string>emojis , string pin) : base(annotation, id)
        {
            this.emojis = emojis; this.pin = pin;
        }
    }
}