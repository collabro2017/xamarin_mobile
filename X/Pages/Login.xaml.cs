using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using X.API;
using X.API.Base;
using X.Helpers;
using Xamarin.Forms;

namespace X.Pages
{
    public partial class Login : ContentPage ,IBaseAPIInterface
    {
        public Login()
        {
            
            if (Cacher.getCurrentUser().Length > 0)
            {
                Constants.accessToken = Cacher.getAccessToken();
                Constants.uid = Cacher.getUid();
                Constants.client = Cacher.getClient();

                GoToMainPage();
            } else {
                InitializeComponent();
                Constants.OnLogin = true;
                Constants.OnRegister = false;
                Constants.onforgotPassword = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Constants.OnLogin = true;
            Constants.OnRegister = false;
            Constants.onforgotPassword = false;
        }
        async void GoToMainPage() {
            // this delay is necessary to make sure the 
            await Task.Delay(1000);

            Navigation.PushModalAsync(new MainPage());
        }

       async void SignUpTapped(object sender, EventArgs e)
        {   
            var fade = (Label)sender;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
              Navigation.PushModalAsync(new SignUp());
        }
         
        async void LoginTapped(object sender, EventArgs e)
        {   

            var fade = (Label)sender;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);

            screenLoading.IsVisible = true;

            if (Password.Text != "" && Email.Text != "")
            {
                if (Email.TextColor != Color.Red)
                {
                    LoginAPI api = new LoginAPI(Email.Text, Password.Text );
                    api.setCallbacks(this);
                    api.getResponse();
                }
                else
                {
                    screenLoading.IsVisible = false;
                    DisplayAlert("X", "Please enter a valid email", "OK");
                }
            }else
            {
                screenLoading.IsVisible = false;

                DisplayAlert("X", "Your email or password is blank", "OK");

            }
            //await Navigation.PushModalAsync(new MainPage());

        }

        public async void ForgotPasswordTapped(object s, EventArgs e)
        {   
            var fade = (Label)s;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
            Navigation.PushModalAsync(new ForgotPassword());
        }

        public void OnSuccess(JObject response, BaseAPI caller)
        {
            Constants.OnLogin = false;
            Constants.current_user = JsonConvert.DeserializeObject<User>(response.ToString(), new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
            Constants.CurrentCategoryOrder = new List<int>();
            Cacher.SaveCurrentUser(Constants.current_user.data.id,Constants.accessToken,Constants.client,Constants.uid);
            foreach (var orderNum in response["user"]["user_setting"]["ratings_order"])
            {
                Constants.CurrentCategoryOrder.Add(orderNum.Value<int>());
                //Constants.CurrentCategoryOrder.Add(response["user"]["user_setting"]["ratings_order"][1].Value<int>());
                //Constants.CurrentCategoryOrder.Add(response["user"]["user_setting"]["ratings_order"][2].Value<int>());
                //Constants.CurrentCategoryOrder.Add(response["user"]["user_setting"]["ratings_order"][3].Value<int>());
            }
            screenLoading.IsVisible = false;

            Navigation.PushModalAsync(new MainPage());
            
        }

        public void OnError(string errMsg, BaseAPI caller)
        {
            DisplayAlert("X","Invalid email or password.","OK");
            screenLoading.IsVisible = false;
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }
    }
}
