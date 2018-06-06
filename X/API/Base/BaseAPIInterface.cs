using System;
using Newtonsoft.Json.Linq;

namespace X.API.Base
{
    public class BaseAPIInterface : IBaseAPIInterface
    {
        public BaseAPIInterface()
        {
        }

        public void OnError(string errMsg, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

        public void OnSuccess(JObject response, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

       
    }
}
