using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API;
using X.API.Base;
using Xamarin.Forms;

namespace X.Pages
{
    public partial class SettingsPage : ContentPage,IBaseAPIInterface
    {
        public SettingsPage()
        {

            InitializeComponent();
            Current_Category.HeightRequest = 100;
            var user = Constants.current_user;
            if (user != null)
            {
                user_fname.Text = user.data.first_name;
                user_lname.Text = user.data.last_name;
                user_email.Text = user.data.email;
                trendNotif.IsToggled = user.data.settings.trend_notification;
                locationNotif.IsToggled = user.data.settings.location_notification;
                Points.Text = user.data.points;

            }
            var position = 1;
           
            foreach (var cons in Constants.CurrentCategoryOrder)
            {   

                Device.BeginInvokeOnMainThread(async () =>
                {

                    await Task.Delay(300);
                    switch (position)
                    {
                        case (1):

                            var btn = new DragBtn
                            {
                                Text = "Food",
                                ClassId = "1",
                                FontSize = 15,
                                BackgroundColor = Color.Transparent
                            };
                            Current_Category.Children.Add(btn, Constraint.RelativeToParent((parent) =>
                            {
                                return 0;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return 0;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }));
                            if (cons == 1)
                            {
                                btn.CurrentDropBox = FoodBox;
                                btn.OnDragCompleted(FoodBox);
                                FoodBox.OnDraggableDropped(btn);
                            }
                            else if (cons == 2)
                            {
                                btn.CurrentDropBox = ServiceBox;
                                btn.OnDragCompleted(ServiceBox);
                                ServiceBox.OnDraggableDropped(btn);
                            }
                            else if (cons == 3)
                            {
                                btn.CurrentDropBox = CleanlinessBox;
                                btn.OnDragCompleted(CleanlinessBox);
                                CleanlinessBox.OnDraggableDropped(btn);
                            }
                            else if (cons == 4)
                            {
                                btn.CurrentDropBox = VibeBox;
                                btn.OnDragCompleted(VibeBox);
                                VibeBox.OnDraggableDropped(btn);
                            }
                            position++;


                            break;

                        case (2):

                            var btn2 = new DragBtn
                            {
                                Text = "Service",
                                ClassId = "2",
                                FontSize = 15,
                                BackgroundColor = Color.Transparent
                            };
                            Current_Category.Children.Add(btn2, Constraint.RelativeToParent((parent) =>
                            {
                                return 0;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }));
                            if (cons == 1)
                            {
                                btn2.CurrentDropBox = FoodBox;
                                btn2.OnDragCompleted(FoodBox);
                                FoodBox.OnDraggableDropped(btn2);
                            }
                            else if (cons == 2)
                            {
                                btn2.CurrentDropBox = ServiceBox;
                                btn2.OnDragCompleted(ServiceBox);
                                ServiceBox.OnDraggableDropped(btn2);
                            }
                            else if (cons == 3)
                            {
                                btn2.CurrentDropBox = CleanlinessBox;
                                btn2.OnDragCompleted(CleanlinessBox);
                                CleanlinessBox.OnDraggableDropped(btn2);
                            }
                            else if (cons == 4)
                            {
                                btn2.CurrentDropBox = VibeBox;
                                btn2.OnDragCompleted(VibeBox);
                                VibeBox.OnDraggableDropped(btn2);
                            }
                            position++;

                            break;

                        case (3):

                            var btn3 = new DragBtn
                            {
                                Text = "Cleanliness",
                                ClassId = "3",
                                FontSize = 15,
                                BackgroundColor = Color.Transparent
                            };
                            Current_Category.Children.Add(btn3, Constraint.RelativeToParent((parent) =>
                            {
                                return 0;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }));
                            if (cons == 1)
                            {
                                btn3.CurrentDropBox = FoodBox;
                                btn3.OnDragCompleted(FoodBox);
                                FoodBox.OnDraggableDropped(btn3);
                            }
                            else if (cons == 2)
                            {
                                btn3.CurrentDropBox = ServiceBox;
                                btn3.OnDragCompleted(ServiceBox);
                                ServiceBox.OnDraggableDropped(btn3);
                            }
                            else if (cons == 3)
                            {
                                btn3.CurrentDropBox = CleanlinessBox;
                                btn3.OnDragCompleted(CleanlinessBox);
                                CleanlinessBox.OnDraggableDropped(btn3);
                            }
                            else if (cons == 4)
                            {
                                btn3.CurrentDropBox = VibeBox;
                                btn3.OnDragCompleted(VibeBox);
                                VibeBox.OnDraggableDropped(btn3);
                            }
                            position++;

                            break;

                        case (4):
                            var btn4 = new DragBtn
                            {
                                Text = "Vibe",
                                ClassId = "4",
                                FontSize = 15,
                                BackgroundColor = Color.Transparent
                            };
                            Current_Category.Children.Add(btn4, Constraint.RelativeToParent((parent) =>
                            {
                                return 0;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .15;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }), Constraint.RelativeToParent((parent) =>
                            {
                                return .1;
                            }));
                            if (cons == 1)
                            {
                                btn4.CurrentDropBox = FoodBox;
                                btn4.OnDragCompleted(FoodBox);
                                FoodBox.OnDraggableDropped(btn4);
                            }
                            else if (cons == 2)
                            {
                                btn4.CurrentDropBox = ServiceBox;
                                btn4.OnDragCompleted(ServiceBox);
                                ServiceBox.OnDraggableDropped(btn4);
                            }
                            else if (cons == 3)
                            {
                                btn4.CurrentDropBox = CleanlinessBox;
                                btn4.OnDragCompleted(CleanlinessBox);
                                CleanlinessBox.OnDraggableDropped(btn4);
                            }
                            else if(cons == 4){
                                btn4.CurrentDropBox = VibeBox;
                                btn4.OnDragCompleted(VibeBox);
                                VibeBox.OnDraggableDropped(btn4);
                            }
                    
                    position++;

                    break;

                }

                });
            }

        }
        bool tapped = true;
        public async void CloseTapped(object sender, EventArgs e)
        {
            var fade = (Label)sender;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
            if (tapped)
            {
                tapped = false;
                SaveSettingsAPI api = new SaveSettingsAPI(user_email.Text, user_fname.Text, user_lname.Text, Constants.CurrentCategoryOrder, locationNotif.IsToggled, trendNotif.IsToggled);
                api.setCallbacks(this);
                api.getResponse();
                    
            }
        }

        public void cat_clicked(object sender, EventArgs e)
        {
            //var s = sender as CustomButtonRenderer;
            //Constants.CurrentCategoryOrder.Remove(int.Parse(s.ClassId));

            //var btn = new CustomButtonRenderer
            //{
            //    Text = s.Text,
            //    ClassId = s.ClassId,
            //               FontSize = 13,
            //};

            //btn.Clicked += add_cat;
            //Unselected_Category.Children.Add( btn);
            //Current_Category.Children.Remove(s);

        }

        public void add_cat(object s, EventArgs e)
        {   
            //var sender = s as CustomButtonRenderer;
            //Constants.CurrentCategoryOrder.Add(int.Parse(sender.ClassId));
            //var btn = new CustomButtonRenderer
            //{
            //    Text = sender.Text,
            //    ClassId = sender.ClassId,
            //                    FontSize = 13
            //};
            //btn.Clicked += cat_clicked;
            //Current_Category.Children.Add(btn);
            //Unselected_Category.Children.Remove(sender);
        }

       async void Handle_Clicked(object sender, System.EventArgs e)
        {       
            var fade = (CustomButtonRenderer)sender;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
            Cacher.SaveCurrentUser("","","","");
            App.Current.MainPage = new Login();
        }

        public async void OnSuccess(JObject response, BaseAPI caller)
        {
            if (!user_fname.Text.Equals("") && !user_lname.Text.Equals("")&& !user_email.Text.Equals(""))
            {
                Constants.current_user = JsonConvert.DeserializeObject<User>(response.ToString(), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                Constants.CurrentCategoryOrder.Clear();

                foreach (var order in response["user"]["user_setting"]["ratings_order"])
                {
                    Constants.CurrentCategoryOrder.Add(order.Value<int>());
                }
                Debug.WriteLine("Success");
                var current = App.currentPage as MainPage;
                current.SetUpEmojis();
                //await DisplayAlert("X","Saved","OK");
                Constants.NotificationAllowed = locationNotif.IsToggled;
                
                await Navigation.PopModalAsync();
            }else
            {
                DisplayAlert("Scuttle","Please fill all fields.","OK");
            }
        }

        public async void OnError(string errMsg, BaseAPI caller)
        {
            Debug.WriteLine("Failed");
            await DisplayAlert("Scuttle", "Failed", "OK");
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }
    }
}
