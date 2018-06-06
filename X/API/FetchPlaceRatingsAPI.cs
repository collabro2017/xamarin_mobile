using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class FetchPlaceRatingsAPI : BaseAPI
    {
        string place_ID;
        public FetchPlaceRatingsAPI(string place_id)
        {
            this.place_ID = place_id;   
        }

        public override string getMethod()
        {
            return "Get";
        }

        public override string getURLTail()
        {
            return "/api/v1/restaurants/"+place_ID;
        }
    }
}

