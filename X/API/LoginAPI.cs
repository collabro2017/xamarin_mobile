using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class LoginAPI : BaseAPI
    {
        public LoginAPI(String Email, String Password)
        {

            var request = new
            {
                
                    email = Email,
                    password = Password,
                    //device_token = CacheSettings.UserDeviceTokenn,
                    //latitude = CacheSettings.CurrentUserLatitude,
                    //longitude = CacheSettings.CurrentUserLatitude
             };

            JObject par = JObject.Parse(JsonConvert.SerializeObject(request));
            parameters = par;
        }

        public override string getMethod()
        {
            return "POST";
        }

        public override string getURLTail()
        {
            return "/api/v1/auth/sign_in";
        }
    }
}
