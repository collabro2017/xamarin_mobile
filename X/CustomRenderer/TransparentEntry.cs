using System;
using Xamarin.Forms;

namespace X
{
    public class TransparentEntry : Entry
    {

        public enum TextGravity
        {
            START_TOP,
            START_CENTER,
            START_BOTTOM,
            CENTER_TOP,
            CENTER,
            CENTER_BOTTOM,
            END_TOP,
            END_CENTER,
            END_BOTTOM
        }

        public static readonly BindableProperty TAProperty = BindableProperty.Create(nameof(TextAlignment), typeof(TextGravity), typeof(TransparentEntry), default(TextGravity),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var view = bindable as TransparentEntry;

                }
        );
        public TextGravity TextGravityAlignment
        {
            get { return (TextGravity)GetValue(TAProperty); }
            set { SetValue(TAProperty, value); }
        }

        public TransparentEntry()
        {
            TextGravityAlignment = TextGravity.START_TOP;
        }
    }
}
