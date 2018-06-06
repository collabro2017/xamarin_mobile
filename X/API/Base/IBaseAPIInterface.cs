using System;
using Newtonsoft.Json.Linq;

namespace X.API.Base
{
    public interface IBaseAPIInterface
    {
        void OnSuccess(JObject response, BaseAPI caller);
        void OnError(String errMsg, BaseAPI caller);
        void OnErrorCode(int errorCode, BaseAPI caller);
    }
}
