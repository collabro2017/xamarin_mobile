using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class SignUpAPI : BaseAPI
    {
        public SignUpAPI(String Email, String Password,String fname, String lname)
        {

            var request = new
            {
                    email = Email,
                    password = Password,
                    first_name = fname,
                    last_name = lname,
                
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
            return "/api/v1/auth";
        }
    }
}
