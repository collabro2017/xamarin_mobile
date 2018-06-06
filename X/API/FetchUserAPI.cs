using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using X.API.Base;

namespace X.API
{
    public class FetchUserAPI : BaseAPI
    {
        
        public FetchUserAPI( )
        {
        }

        public override string getMethod()
        {
            return "GET";
        }

        public override string getURLTail()
        {
            return "/api/v1/account";
        }
    }
}
