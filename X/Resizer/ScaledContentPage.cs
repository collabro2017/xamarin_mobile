using System;
using Xamarin.Forms;

namespace X
{
    public class ScaledContentPage: ContentPage
    {
        public ScaledContentPage()
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }
        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(Resizer.scaleChild(child));
        }
    }
}
 

