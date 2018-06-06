using System;
using System.Collections.Generic;
using Xamarin.Forms;
using static X.RatingsView;

namespace X.Models
{
    public class EmojiCategory
    {
        public EmojiCategory()
        {
        }
        public int ID { get; set; }
        public string name { get; set; }
        public string emoji { get; set; }
        public string Font_size { get; set; }
        public List<Emojis> ratingEmoji { get; set; }
        public IEmojiTapped pagecallback { get; set; }
    }
}
