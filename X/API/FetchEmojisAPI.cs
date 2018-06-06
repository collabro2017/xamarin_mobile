using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class FetchEmojisAPI : BaseAPI
    {
        public FetchEmojisAPI( )
        {
        }

        public override string getMethod()
        {
            return "Get";
        }

        public override string getURLTail()
        {
            return "/api/v1/ratings";
        }
    }
}
