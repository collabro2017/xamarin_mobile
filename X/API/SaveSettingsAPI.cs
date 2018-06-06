using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class SaveSettingsAPI : BaseAPI
    {
        public SaveSettingsAPI(String Email, String Fname,String Lname, List<int> catOrder,bool locNotif, bool trendNotif)
        {
            var strcatOrder = new List<string>();
            foreach(var item in catOrder)
            {
                strcatOrder.Add(item.ToString());
            }
            var request = new
            {
                user = new
                {
                    first_name = Fname,
                    last_name = Lname,
                    email = Email,
                    user_setting_attributes = new
                    {   
                        location_notification = locNotif,
                        trend_notification = trendNotif,
                        ratings_order = strcatOrder
                    }
                }

            };

            JObject par = JObject.Parse(JsonConvert.SerializeObject(request));
            parameters = par;
        }

        public override string getMethod()
        {
            return "patch";
        }

        public override string getURLTail()
        {
            return "/api/v1/account";
        }
    }
}
