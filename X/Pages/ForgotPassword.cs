using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using X.API;
using X.API.Base;
using Xamarin.Forms;

namespace X.Pages
{
    public partial class ForgotPassword : ContentPage, IBaseAPIInterface
    {
        public ForgotPassword()
        {
            Constants.onforgotPassword = true;
            Constants.OnRegister = false;
            Constants.OnLogin = false;
            InitializeComponent();
        }
        public void OnError(string errMsg, BaseAPI caller)
        {
            DisplayAlert("Scuttle", errMsg, "OK");
            screenLoading.IsVisible = false;
        }
        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }
        public void OnSuccess(JObject response, BaseAPI caller)
        {
            if ((bool)response["success"])
            {
                screenLoading.IsVisible = false;
                DisplayAlert("Scuttle", response["message"].ToString(), "OK");
                Navigation.PopModalAsync();

            }
            else 
            {
                DisplayAlert("Scuttle", response["message"].ToString(), "OK");

            }
        } 
        public void CloseTapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        public void resetTapped(object sender, EventArgs e)
        {
            if (Email.Text != "" && Email.TextColor != Color.Red)
            {
                screenLoading.IsVisible = true;
                ResetPasswordAPI api = new ResetPasswordAPI(Email.Text);
                api.setCallbacks(this);
                api.getResponse();
            }else
            {
                DisplayAlert("Scuttle","Please enter a valid email", "OK");
            }
        }

    }
}
