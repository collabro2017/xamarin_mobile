using System;
using System.Collections.Generic;
using System.Diagnostics;
using X.Pages;
using Xamarin.Forms;

namespace X
{

    //public class RatingEventArgs: EventArgs
    //{
    //    public Emoji Emoji { get; set; }

    //    public RatingEventArgs(Emoji e): base()
    //    {
    //        Emoji = e;
    //    }
    //}

    public interface IEmojiTapped
    {
        void DidTapEmoji(Label emoji,double x,double y, int emoji_id);
    }

    public class RatingsView : ContentView
    {
        //static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("Emoji", typeof(List<Emoji>), typeof(EmailValidator), false);
        //public List<Emoji> Emojis { get; set; }
        //public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        //public List<Emoji> Emoji
        //{
        //    get { return (List<Emoji>)base.GetValue(IsValidProperty); }
        //    private set { base.SetValue(IsValidPropertyKey, value); }
        //}

        //public class Whatever : AddFriendViewGestures
        //{
        //    public void didtapEmoji(Image emoji)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        public static readonly BindableProperty EmojisProperty = BindableProperty.Create(nameof(Emojis), typeof(List<Emojis>), typeof(RatingsView), default(List<Emoji>), BindingMode.Default, null, (bindable, oldValue, newValue) =>
          {
              var view = bindable as RatingsView;
              var oldval = oldValue as List<Emojis>;
            var newval = newValue as List<Emojis>;

              view.AddEmojis(newval);
          });
        public List<Emojis> Emojis
        {
            get { return (List<Emojis>)GetValue(EmojisProperty); }
            set { SetValue(EmojisProperty, value); }
        }

        public readonly BindableProperty RatingProperty = BindableProperty.Create(nameof(Ratings), typeof(IEmojiTapped), typeof(RatingsView), default(IEmojiTapped), BindingMode.Default, null, (bindable, oldValue, newValue) =>
        {
            var view = bindable as RatingsView;
            var oldval = oldValue as IEmojiTapped;
            var newval = newValue as IEmojiTapped;

            view.Ratings = newval;
        });
        public IEmojiTapped Ratings
        {
            get { return (IEmojiTapped)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        //public event EventHandler<RatingEventArgs> EmojiTapped;


        private StackLayout mainLayout;
        public RatingsView()
        {
            mainLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                MinimumWidthRequest = App.ScreenWidth - 200,

            };

            Content = mainLayout;
        }
        private static int col = 0;
        StackLayout lRow;
        private Label recent = new Label();
        Random rand = new Random();
        public void AddEmojis(List<Emojis> emojis)
        {
            List<Emojis> tobesorted = emojis;

            Emojis temp = new Emojis();
            for (int x = 0; x < tobesorted.Count; x++)
            {
                for (int sort = 0; sort < tobesorted.Count- 1; sort++)
                {
                    if (tobesorted[sort].position > tobesorted[sort + 1].position)
                    {
                        temp = tobesorted[sort + 1];
                        tobesorted[sort + 1] = tobesorted[sort];
                        tobesorted[sort] = temp;
                    }
                }
            }

          
            //Emojis = emojis;

            int Added = 0;
            if (Emojis.Count > 0)
            {
                int Index = 0, col = 0, row = 0;
                while(Index < Emojis.Count)
                {
                    if(Emojis[Added].status == 1){}
                    if (col == 0)
                    {
                        lRow = new StackLayout()
                        {
                            VerticalOptions = LayoutOptions.Fill,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Orientation = StackOrientation.Vertical,

                            //Spacing = rand.Next(5,10),
                        };
                        if (Constants.is_android) 
                        {
                            lRow.Margin = new Thickness(0, 5, 0, 0);
                        }
                        else
                        {
                            lRow.Margin = new Thickness(0, 20, 0, 0);
                        }
                        mainLayout.Children.Add(lRow);
                    }

                    var Rand_Col = rand.Next(2,3);
                    
                    for (; col < 2 && Index < Emojis.Count; col++, Index++)
                    {
                        System.Diagnostics.Debug.WriteLine("Displaying {0} {1}", Emojis[Index].name, Emojis[Index].image);
                        var EmojiView = new StackLayout()
                        {   BackgroundColor=Color.Transparent,
                            Spacing = 0,
                            Margin = new Thickness(0,0,0,0),
                            //TranslationY = rand.Next(-4,7),
                            //TranslationX = rand.Next(-15,15),
                        };

                        var image = new Label()
                        {
                            ClassId = Emojis[Index].emoji_ID.ToString(),
                            Text = Emojis[Index].image,
                            FontSize = 25 * App.scale,
                            TextColor = Color.Black,
                            //WidthRequest = 25 * App.scale,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalOptions= LayoutOptions.Center,
                            Opacity = 1

                        };
                        var name = new Label
                        {
                            Text = Emojis[Index].name,
                            FontSize = 14 * App.scale,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            Opacity = 1
                        };

                        //if(Constants.is_android)
                        //{
                        //    image.WidthRequest = 20 * App.scale;
                        //    image.FontSize = 20 * App.scale;
                        //    name.FontSize = 12 * App.scale;
                        //}
                        EmojiView.Children.Add(image);
                        if (name.Text.Equals("5m") || name.Text.Equals("10m") || name.Text.Equals("15m+"))
                        {
                            image.FontSize = 20 * App.scale;

                            EmojiView.Margin = new Thickness(0,3,0,5);
                            name.Text = "Wait";
                        }

                            EmojiView.Children.Add(name);

                        

                        TapGestureRecognizer tap = new TapGestureRecognizer();
                        tap.Tapped += (sender, e) =>
                        {   
                            if(recent != null)
                            {
                                recent.Opacity = 1;
                            }
                            recent = image;
                            image.Opacity = 0.4;

                            //RaiseTappedEvent(Emojis[Index]);
                            var current = App.currentPage as MainPage;
                            var y = EmojiView.Y; 
                            var x = EmojiView.X; 
                            var parent = (VisualElement)image.Parent; 
                            while (parent != null && parent.Parent.GetType() !=  typeof(App))
                            {
                            y += parent.Y;
                            x += parent.X;

                            parent = (VisualElement)parent.Parent; 
                            } 
                            current.DidTapEmoji(image, x,y, int.Parse(image.ClassId));
                        };
                        EmojiView.GestureRecognizers.Add(tap);

                        lRow.Children.Add(EmojiView);
                         

                    }
                    if (col == Rand_Col)
                    {
                        col = 0;
                        row++;
                    }
                
                }
            }

            //void RaiseTappedEvent(Emoji e) {
            //    var handler = EmojiTapped;
            //    if (handler != null) {
                    
            //    }
            //}
            //void HandleEmojisChanged(BindableObject bindable, object oldValue, object newValue)
            //{

            //}

            //bool stackfull = false;
            //foreach (var e in Emojis)
            //{    
            //     stackfull = false;
            //    if (col == 0)
            //    {
            //        lRow = new StackLayout()
            //        {
            //            VerticalOptions = LayoutOptions.Fill,
            //            HorizontalOptions = LayoutOptions.FillAndExpand,
            //            Orientation = StackOrientation.Horizontal,
            //            Spacing = 40,
            //            Margin = new Thickness(0, 15, 0, 0)
            //        };
            //        mainLayout.Children.Add(lRow);
            //    }
            //    EmojiView.Children.Add(new Image()
            //    {
            //        Source = e.Image,
            //        WidthRequest= 20,
            //    });
            //    EmojiView.Children.Add(new Label 
            //    {
            //        Text=e.Name,
            //        FontSize = 14
            //    });
            //    ctr++;
            //    for (; col < 3; col++)
            //    {
            //        lRow.Children.Add(EmojiView);
            //    }
            //    if(ctr>newmax)
            //    {   
            //        col = 0;
            //        stackfull = true;
            //        newmax += ctr + 5;
            //    };
            //    //if(stackfull)
            //    //mainLayout.Children.Add(horizontalstack);
            //}
        }
    }
}

