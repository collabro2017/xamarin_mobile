using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace X
{
    public partial class CVindicator : ContentView
    {
        public CVindicator()
        {
            InitializeComponent();
            //view1.WidthRequest *= App.scale;
            //view2.WidthRequest *= App.scale;
            //view3.WidthRequest *= App.scale;
            //view4.WidthRequest *= App.scale;
        }

        public void cvmoved(int currentview)
        {
            switch(currentview)
            {
                case 0: 
                    view1.Source = "ic_circle_black";
                    view2.Source = "ic_circle";
                    view3.Source = "ic_circle";
                    view4.Source = "ic_circle";
                break;
                case 1: 
                    view1.Source = "ic_circle";
                    view2.Source = "ic_circle_black";
                    view3.Source = "ic_circle";
                    view4.Source = "ic_circle";
                    break;
                case 2:
                    view1.Source = "ic_circle";
                    view2.Source = "ic_circle";
                    view3.Source = "ic_circle_black";
                    view4.Source = "ic_circle";
                    break;
                case 3:
                    view1.Source = "ic_circle";
                    view2.Source = "ic_circle";
                    view3.Source = "ic_circle";
                    view4.Source = "ic_circle_black";
                    break;
            }
        }
    }
}
