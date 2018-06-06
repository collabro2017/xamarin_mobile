using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API;
using X.API.Base;
using Xamarin.Forms;

namespace X.Pages
{
    public partial class SignUp : ContentPage, IBaseAPIInterface
    {
        public SignUp()
        {
            InitializeComponent();
            Constants.OnRegister = true;
            Constants.OnLogin = false;
            Constants.onforgotPassword = false;
        }

       

        void BackTapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void SignUpClicked(object sender, EventArgs e)
        {
            if (once)
            {
                once = false;
                SignUpAPI api = new SignUpAPI(email.Text, password.Text, fname.Text, lname.Text);
                api.setCallbacks(this);
                api.getResponse();
            }
        }


        public void OnError(string errMsg, BaseAPI caller)
        {
            DisplayAlert("Scuttle",errMsg,"OK");
            Constants.OnRegister = false;
            once = true;
        }
        bool once = true;
        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
        }

        public async void OnSuccess(JObject response, BaseAPI caller)
        {   
            
            if (response["status"].Value<String>().Equals("200"))
            {
                Constants.current_user = JsonConvert.DeserializeObject<User>(response.ToString(), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                Cacher.SaveCurrentUser(Constants.current_user.data.id, Constants.accessToken, Constants.client, Constants.uid);


                Debug.WriteLine("accessToken---"+Constants.accessToken+"uid---"+Constants.uid+"Client---"+Constants.client);
                await Navigation.PushModalAsync(new MainPage());
            }else
            {
                once = true;
                DisplayAlert("Scuttle",response["errors"]["full_messages"][0].ToString(), "OK");
            }
        }
        
    }
}
