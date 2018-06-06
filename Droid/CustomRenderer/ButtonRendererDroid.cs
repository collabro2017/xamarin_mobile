using System;
using Android.Content;
using Xamarin.Forms;
using X.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using X;

[assembly: ExportRenderer(typeof(CustomButtonRenderer), typeof(ButtonRendererDroid))]
namespace X.Droid.CustomRenderer
{
    public class ButtonRendererDroid : ButtonRenderer
    {
        public ButtonRendererDroid(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
            {
                Control.SetPadding(0, 0, 0, 0);
                Control.TransformationMethod = null; //Prevents newline
            }
        }
    }
}