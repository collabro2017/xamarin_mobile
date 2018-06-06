using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class SendRatingAPI : BaseAPI
    {
        String place_ID;

        public SendRatingAPI(String place_id, int emoji_id)
        {
            place_ID = place_id;
            var request = new
            {
                rating_id = emoji_id,
            };

            JObject par = JObject.Parse(JsonConvert.SerializeObject(request));
            parameters = par;
            Debug.WriteLine(parameters);
        }

        public override string getMethod()
        {
            return "POST";
        }

        public override string getURLTail()
        {
            return "/api/v1/restaurants/"+place_ID+"/rate";
        }
    }
}
