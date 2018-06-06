using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using X.iOS.CustomRenderer;
using X;

[assembly: ExportRenderer(typeof(TransparentEntry), typeof(TransparentEntryRenderer))]
namespace X.iOS.CustomRenderer
{
    public class TransparentEntryRenderer : EntryRenderer
    {

        TransparentEntry fControl;
        UITextField nControl;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.SpellCheckingType = UITextSpellCheckingType.No;             // No Spellchecking
                Control.AutocorrectionType = UITextAutocorrectionType.No;           // No Autocorrection
                Control.AutocapitalizationType = UITextAutocapitalizationType.None; // No Autocapitalization
            }

            fControl = (TransparentEntry)Element;
            nControl = this.Control;

            nControl.ReturnKeyType = UIReturnKeyType.Go;
            this.Control.Layer.CornerRadius = 1;
            this.Control.BackgroundColor = UIColor.Clear;

            this.Control.Layer.BorderWidth = 0;
            this.Control.Layer.BorderColor = UIColor.Clear.CGColor;
            this.Control.BorderStyle = UITextBorderStyle.None;




        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            if (nControl == null)
                return;


            switch (fControl.TextGravityAlignment)
            {

                //leftmost
                case TransparentEntry.TextGravity.START_TOP:
                    Control.TextAlignment = UITextAlignment.Left;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
                    break;

                case TransparentEntry.TextGravity.START_CENTER:
                    Control.TextAlignment = UITextAlignment.Left;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    break;
                case TransparentEntry.TextGravity.START_BOTTOM:
                    Control.TextAlignment = UITextAlignment.Left;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
                    break;

                //center

                case TransparentEntry.TextGravity.CENTER_TOP:
                    Control.TextAlignment = UITextAlignment.Center;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;

                    break;
                case TransparentEntry.TextGravity.CENTER:
                    Control.TextAlignment = UITextAlignment.Center;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;

                    break;
                case TransparentEntry.TextGravity.CENTER_BOTTOM:
                    Control.TextAlignment = UITextAlignment.Center;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;

                    break;

                //rightmost

                case TransparentEntry.TextGravity.END_TOP:
                    Control.TextAlignment = UITextAlignment.Right;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
                    break;
                case TransparentEntry.TextGravity.END_CENTER:
                    Control.TextAlignment = UITextAlignment.Right;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    break;
                case TransparentEntry.TextGravity.END_BOTTOM:
                    Control.TextAlignment = UITextAlignment.Right;
                    Control.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
                    break;

            }
        }

    }
}
