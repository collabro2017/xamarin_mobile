using System;
using CoreGraphics;
using UIKit;
using X;
using X.iOS.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ScaledFrame), typeof(CustomFrameRenderer))]
[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace X.iOS.CustomRenderer
{
    /// <summary>
    /// Renderer to update all frames with better shadows matching material design standards
    /// </summary>

    public class CustomFrameRenderer : FrameRenderer
    {
        ScaledFrame fControl;
        string element;
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                element = e.NewElement.ToString();
            }
            if (element.Equals("X.ScaledFrame"))
            {
                fControl = (ScaledFrame)e.NewElement;
            }
        }
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (element.Equals("X.ScaledFrame"))
            {
                if (!fControl.has_Shadow)
                {
                    Layer.ShadowRadius = 0.01f;
                    Layer.ShadowColor = UIColor.White.CGColor;
                    Layer.ShadowOffset = new CGSize(2, 2);
                    Layer.ShadowOpacity = 0.01f;
                    Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                }else
                {
                    //Layer.CornerRadius = 15.0f;
                    // Update shadow to match better material design standards of elevation
                    Layer.ShadowRadius = 2.0f;
                    Layer.ShadowColor = UIColor.Gray.CGColor;
                    Layer.ShadowOffset = new CGSize(2, 2);
                    Layer.ShadowOpacity = 0.70f;
                    Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                    Layer.MasksToBounds = false;
                }

            }else
            {
                //Layer.CornerRadius = 15.0f;
                // Update shadow to match better material design standards of elevation
                Layer.ShadowRadius = 2.0f;
                Layer.ShadowColor = UIColor.Gray.CGColor;
                Layer.ShadowOffset = new CGSize(2, 2);
                Layer.ShadowOpacity = 0.70f;
                Layer.ShadowPath = UIBezierPath.FromRect(Layer.Bounds).CGPath;
                Layer.MasksToBounds = false;
            }
        }
    }
}
