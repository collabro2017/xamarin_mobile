using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class ResetPasswordAPI: BaseAPI
    {   
        public ResetPasswordAPI(String Email)
        {
        var request = new
        {

            email =  Email,
            redirect_url ="http://198.58.101.247:57552/password_reset"
             
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
            return "/api/v1/auth/password";
    }
    }
}
