using Android.Views;
using X;
using X.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TransparentEntry), typeof(TransparentEntryRenderer))]
namespace X.Droid.CustomRenderer
{
    class TransparentEntryRenderer : EntryRenderer    {
        TransparentEntry fControl;
        FormsEditText nControl;


        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {

            base.OnElementChanged(e);
            if (Control != null)
            {
                fControl = (TransparentEntry)Element;
                nControl = this.Control;


                //Control.Background = Resources.GetDrawable(Resource.Drawable.RoundedEntry);
                //Control.SetBackgroundColor(Color.Transparent.ToAndroid());

                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
                Control.SetPadding(1, 3, 1, 1);
                Control.SetImeActionLabel("Go", Android.Views.InputMethods.ImeAction.Done);

            }

        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            switch (fControl.TextGravityAlignment)
            {

                //leftmost
                case TransparentEntry.TextGravity.START_TOP:
                    Control.Gravity = (GravityFlags.Start | GravityFlags.Top);
                    break;
                case TransparentEntry.TextGravity.START_CENTER:
                    Control.Gravity = (GravityFlags.Start | GravityFlags.CenterVertical);
                    break;
                case TransparentEntry.TextGravity.START_BOTTOM:
                    Control.Gravity = (GravityFlags.Start | GravityFlags.Bottom);
                    break;

                //center

                case TransparentEntry.TextGravity.CENTER_TOP:
                    Control.Gravity = (GravityFlags.CenterHorizontal | GravityFlags.Top);

                    break;
                case TransparentEntry.TextGravity.CENTER:
                    //Control.SetForegroundGravity(GravityFlags.Center);
                    Control.Gravity = (GravityFlags.Center);

                    break;
                case TransparentEntry.TextGravity.CENTER_BOTTOM:
                    Control.Gravity = (GravityFlags.CenterHorizontal | GravityFlags.Bottom);

                    break;

                //rightmost
                case TransparentEntry.TextGravity.END_TOP:
                    Control.Gravity = (GravityFlags.End | GravityFlags.Top);
                    break;
                case TransparentEntry.TextGravity.END_CENTER:
                    Control.Gravity = (GravityFlags.End | GravityFlags.CenterVertical);
                    break;
                case TransparentEntry.TextGravity.END_BOTTOM:
                    Control.Gravity = (GravityFlags.End | GravityFlags.Bottom);
                    break;

            }
        }

    }
}
