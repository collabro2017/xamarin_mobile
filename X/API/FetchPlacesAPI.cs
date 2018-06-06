using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class FetchPlacesAPI : BaseAPI
    {
        string lat, lng;
        int zoom;
        public FetchPlacesAPI(string Lat, string Lng, int Zoom)
        {
            lat = Lat;
            lng = Lng;
            zoom = Zoom;


            //var request = new
            //{
            //    user = new
            //    {
            //        email = Email,
            //        password = Password,
            //        //device_token = CacheSettings.UserDeviceTokenn,
            //        //latitude = CacheSettings.CurrentUserLatitude,
            //        //longitude = CacheSettings.CurrentUserLatitude
            //    }
            //};
            //JObject par = JObject.Parse(JsonConvert.SerializeObject(request));
            //parameters = par;
        }

        public override string getMethod()
        {
            return "GET";
        }

        public override string getURLTail()
        {
            return "/api/v1/restaurants?lat="+lat+"&lng="+lng;
        }
    }
}
