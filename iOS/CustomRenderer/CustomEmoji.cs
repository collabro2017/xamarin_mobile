using System;
using CoreGraphics;
using Foundation;
using UIKit;
using X;
using X.iOS.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEmoji), typeof(CustomEmoji))]
namespace X.iOS.CustomRenderer
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards
    /// </summary>

    public class CustomEmoji : LabelRenderer
    {
        ScaledFrame fControl;
        string element;
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            element = e.NewElement.ToString();
           
        }

    }
}
