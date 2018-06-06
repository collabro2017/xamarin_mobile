using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace X
{
    public partial class RatingObject : ContentView
    {
        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string), typeof(RatingObject), default(string),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var view = bindable as RatingObject;
                    view.emojiView.WidthRequest *= App.scale;
            view.emojiView.Text = (string)newValue.ToString();
                }
        );
        public static readonly BindableProperty SizeProperty = BindableProperty.Create(nameof(Size), typeof(double), typeof(RatingObject), default(double),
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var view = bindable as RatingObject;
                    view.emojiView.FontSize = (double)newValue;
               }
       );
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private bool isAnimating = false;
        private const int UPDATE_INTERVAL = 10;

        public RatingObject()
        {
            InitializeComponent();
        }

        public async Task AnimateLiveStream(int duration)
        {

            isAnimating = true;


            Easing e = Easing.CubicOut;

            double startX = -(5*App.scale);
            double endX = -(1.4 * App.ScreenWidth);
            double deltaX = endX - startX;

            Random r = new Random();

            double height = 0;
            double startY = 0;
            //if (Constants.is_android)
            //{

                //      startX = -(1 * App.scale);
                //      endX = -(1.1 * App.ScreenWidth);

                //    height = 0.05 * App.ScreenHeight;
                //    startY = 0.03 * App.ScreenHeight;
                //    this.FadeTo(.4, uint.Parse((duration).ToString()), Easing.SinIn);
           
            //}
            //else
            //{
                height = 0.05 * App.ScreenHeight;
                startY = 0.05 * App.ScreenHeight;
                this.FadeTo(.5, uint.Parse((duration - 2000).ToString()), Easing.SinIn);
            //}
            int sign = r.NextDouble() < 0.5 ? -1 : 1;
            double phase = r.NextDouble() * height;

            for (double t = 0; t < duration; t += UPDATE_INTERVAL)
            {
                var x = e.Ease(t / duration);
                this.TranslationX = startX + deltaX * x;
                this.TranslationY = startY + sign * height * Math.Sin(2*Math.PI*(t/duration) + phase);
                await Task.Delay(UPDATE_INTERVAL);
            }
            isAnimating = false;

        }

        public void Reset() {
            this.TranslationX = 0;
            this.TranslationY = 0;
            this.Opacity = 1.0;
            isAnimating = false;
        }

        
    }
}
