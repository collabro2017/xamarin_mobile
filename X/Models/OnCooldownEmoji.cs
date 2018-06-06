using System;
namespace X.Models
{
    public class OnCooldownEmoji
    {
        public OnCooldownEmoji()
        {
        }

        public int id { get; set; }
        public bool onCooldown { get; set; }
        public bool added { get; set; }
        public DateTime dateClicked { get; set; }
        public Locations pin { get; set; }
    }
}
