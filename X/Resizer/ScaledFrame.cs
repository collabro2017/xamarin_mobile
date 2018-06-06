using System;
using Xamarin.Forms;

namespace X
{
    public class ScaledFrame : Frame
    {   
        public bool has_Shadow { get; set; }

        public ScaledFrame()
        {
            has_Shadow = false;
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(Resizer.scaleChild(child));
        }
    }
}

