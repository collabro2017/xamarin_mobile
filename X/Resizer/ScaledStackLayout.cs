using System;
using Xamarin.Forms;

namespace X
{
     
        public class ScaledStackLayout : StackLayout
        {
        public ScaledStackLayout()
            {
                this.Spacing = 0;
            }

            protected override void OnChildAdded(Element child)
            {
                base.OnChildAdded(Resizer.scaleChild(child));
            }
        }
}
