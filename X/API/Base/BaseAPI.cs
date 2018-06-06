using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace X.API.Base
{
    public abstract class BaseAPI
    {

        public abstract String getURLTail();
        public abstract String getMethod();

        HttpClient client;
        IBaseAPIInterface callback = new BaseAPIInterface();
        private String API_URL = Constants.SERVER_URL;
        private String REQUEST_URL = "";

        public JObject parameters;

        public List<StreamContent> imageStream;
        public List<StringContent> data;

        public BaseAPI()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public BaseAPI getResponse()
        {


            REQUEST_URL = API_URL + "" + getURLTail();

            if (getMethod().ToLower().Equals("get"))
            {
                System.Diagnostics.Debug.WriteLine("GetRequest() running..");
                getRequest();
            }
            else if (getMethod().ToLower().Equals("post"))
            {
                System.Diagnostics.Debug.WriteLine("postRequest() running..");
                postRequest(parameters.ToString());
            }
            else if (getMethod().ToLower().Equals("patch"))
            {
                System.Diagnostics.Debug.WriteLine("putRequest() running..");
                putRequest(parameters.ToString());
            }
            else if (getMethod().ToLower().Equals("updatepicture"))
            {
                System.Diagnostics.Debug.WriteLine("updateprofile() running..");
                UpdateProfile(imageStream, data);
            }
            else if (getMethod().ToLower().Equals("sendpicture"))
            {
                System.Diagnostics.Debug.WriteLine("sendpicture() running..");
                sendImage(imageStream, data);
            }
            else if (getMethod().ToLower().Equals("delete"))
            {
                System.Diagnostics.Debug.WriteLine("delete() running..");
                deleteRequest(parameters.ToString());
            }
            System.Diagnostics.Debug.WriteLine(REQUEST_URL);

            return this;
        }

        public void setCallbacks(IBaseAPIInterface callback)
        {
            this.callback = callback;
        }



        public async Task<JObject> postRequest(string json)
        {
            REQUEST_URL = API_URL + "" + getURLTail();
            var uri = new Uri(REQUEST_URL);
            JObject result = null;
            if (Constants.client != null)
            {
                client.DefaultRequestHeaders.Add("client", Constants.client);
                client.DefaultRequestHeaders.Add("uid", Constants.uid);
                client.DefaultRequestHeaders.Add("access-token", Constants.accessToken);
            }
            if (Constants.isDebug)
            {
                callback.OnSuccess(result, this);
                return null;
            }
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Debug.WriteLine("URL:" + content + " URL : " + REQUEST_URL);
                HttpResponseMessage response = null;
                response = await client.PostAsync(uri, content);
                var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JObject.Parse(stringResult);
                Debug.WriteLine("RESULT:" + result);


                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //if (result["status"].Value<string>() == "500")
                    //{
                    //    callback.OnError("Can't connect to server", this);
                    //}
                    //if (result[""]["errors"].Value<string>() != null)
                    //{
                    //    callback.OnError(result["message"][0].ToString(), this);
                    //}
                    //else
                    if (Constants.OnRegister)
                    {
                        callback.OnSuccess(result, this);
                    }
                    else
                    {
                        callback.OnError(response.ReasonPhrase, this);
                    }
                }
                else
                {   
                    if(Constants.onforgotPassword)
                    {
                        if((bool)result["success"])
                        {
                            callback.OnSuccess(result, this);
                        }else
                        {
                            callback.OnError(result["message"].ToString(), this);
                        }
                    }
                    if (Constants.OnLogin)
                    {   
                        Constants.uid = response.Headers.GetValues("uid").First();
                        Constants.client = response.Headers.GetValues("client").First();
                        Constants.accessToken = response.Headers.GetValues("access-token").First();
                        callback.OnSuccess(result, this);
                    }
                    else
                    {
                        if (!Constants.onforgotPassword)
                        {
                            if (result["status"].Value<string>() == "300")
                            //Callback
                            {
                                callback.OnError(result["message"].ToString(), this);
                            }
                            else if (result["status"].Value<string>() == "422")
                            {
                                callback.OnError(response.ReasonPhrase, this);
                            }
                            else
                            {
                                if (Constants.OnRegister)
                                {
                                    Constants.uid = response.Headers.GetValues("uid").First();
                                    Constants.client = response.Headers.GetValues("client").First();
                                    Constants.accessToken = response.Headers.GetValues("access-token").First();
                                }
                                callback.OnSuccess(result, this);
                            }
                        }
                    }
                }

                return result;

            }
            catch (Exception e)
            {

                Type exceptionType = e.GetType();
                //if (exceptionType == typeof(TaskCanceledException))
                //{
                //  TaskCanceledException tce = (TaskCanceledException)e;
                //  if (tce.CancellationToken == Cacher.ge.Token)
                //  {
                //      callback.OnError(ex.Message.ToString(), this);
                //      return result;
                //  }
                //  else
                //  {
                //      callback.OnError(ex.Message.ToString(), this);
                //      return result;
                //  }
                //}
                //else
                if (exceptionType == typeof(WebException))
                {
                    WebException ex = (WebException)e;
                    var respons = (HttpWebResponse)ex.Response;
                    var dataStream = respons.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    var serverMessage = reader.ReadToEnd();
                    result = JObject.Parse(serverMessage);

                    callback.OnError(result.ToString(), this);
                    return result;

                }
                else if (exceptionType == typeof(HttpRequestException))
                {
                    HttpRequestException ex = (HttpRequestException)e;
                    callback.OnError(ex.Message, this);
                    return result;

                }
                else
                {
                    //Exception ex = (WebException)e;
                    //var respons = (HttpWebResponse)ex.Response;
                    //var dataStream = respons.GetResponseStream();
                    //StreamReader reader = new StreamReader(dataStream);
                    //var serverMessage = reader.ReadToEnd();
                    //result = JObject.Parse(serverMessage);

                    callback.OnError(e.Message, this);
                    return result;
                }
                //var respons = (HttpWebResponse)ex.Response;
                //var dataStream = respons.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //var serverMessage = reader.ReadToEnd();
                //result = JObject.Parse(serverMessage);


            }


        }

        private async Task<JObject> putRequest(string json)
        {
            REQUEST_URL = API_URL + "" + getURLTail();
            var uri = new Uri(REQUEST_URL);
            client.DefaultRequestHeaders.Add("client", Constants.client);
            client.DefaultRequestHeaders.Add("uid", Constants.uid);
            client.DefaultRequestHeaders.Add("access-token", Constants.accessToken);

            JObject result = null;
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                response = await client.PutAsync(uri, content);
                var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JObject.Parse(stringResult);

                Debug.WriteLine("Result", "" + result);

                if (response.StatusCode != HttpStatusCode.OK)
                {

                    if (result["error"].Value<string>() != null)
                    {
                        callback.OnError(result["error"].ToString(), this);
                    }
                    else

                        callback.OnError(response.ReasonPhrase, this);
                }
                else
                {


                    //Callback
                    callback.OnSuccess(result, this);
                }

                return result;

            }
            catch (WebException ex)
            {
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;
            }
        }

        public async Task<JObject> sendImage(List<StreamContent> images = null, List<StringContent> listOfData = null)
        {

            REQUEST_URL = API_URL + "" + getURLTail();
            var uri = new Uri(REQUEST_URL);
            JObject result = null;
            try
            {
                using (var multipart = new MultipartFormDataContent())
                {
                    if (images != null)
                        foreach (var image in images)
                            multipart.Add(image);
                    if (listOfData != null)
                        foreach (var data in listOfData)
                            multipart.Add(data);
                    HttpResponseMessage response = null;
                    response = await client.PostAsync(uri, multipart);
                    string stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (images != null)
                        foreach (var image in images)
                            try { image.Dispose(); } catch (Exception ex) { }
                    if (listOfData != null)
                        foreach (var data in listOfData)
                            try { data.Dispose(); } catch (Exception ex) { }
                    Debug.WriteLine(stringResult);
                    result = JObject.Parse(stringResult);


                    if (response.StatusCode != HttpStatusCode.OK)
                    {

                        if (result["error"].Value<string>() != null)
                        {
                            callback.OnError(result["error"].ToString(), this);
                        }
                        else

                            callback.OnError(response.ReasonPhrase, this);
                    }
                    else
                    {


                        //Callback
                        callback.OnSuccess(result, this);
                    }
                    return result;


                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception" + ex.Message);
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Another exception " + ex.Message);
                throw new Exception("basdfs ");
            }
        }

        public async Task<JObject> UpdateProfile(List<StreamContent> images = null, List<StringContent> listOfData = null)
        {
            REQUEST_URL = API_URL + "" + getURLTail();
            var uri = new Uri(REQUEST_URL);
            JObject result = null;
            try
            {
                using (var multipart = new MultipartFormDataContent())
                {
                    if (images != null)
                        foreach (var image in images)
                            multipart.Add(image);
                    if (listOfData != null)
                        foreach (var data in listOfData)
                            multipart.Add(data);
                    var response = await client.PutAsync(uri, multipart);
                    var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (images != null)
                        foreach (var image in images)
                            try { image.Dispose(); } catch (Exception ex) { }
                    if (listOfData != null)
                        foreach (var data in listOfData)
                            try { data.Dispose(); } catch (Exception ex) { }
                    return JObject.Parse(stringResult);
                }

            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception" + ex.Message);
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Another exception " + ex.Message);
                throw new Exception("basdfs ");
            }
        }

        private async Task<JObject> deleteRequest(string json)
        {
            var uri = new Uri(REQUEST_URL);
            JObject result = null;
            if (Constants.isDebug)
            {
                callback.OnSuccess(result, this);
                return null;
            }
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;
                response = await client.DeleteAsync(uri);
                var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JObject.Parse(stringResult);
                callback.OnSuccess(result, this);
                Debug.WriteLine(result.ToString());
                return result;

            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception" + ex.Message);
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Another exception " + ex.Message);
                throw new Exception("basdfs ");
            }
        }
        private async Task<JObject> getRequest()
        {       
            client.DefaultRequestHeaders.Add("client", Constants.client);
            client.DefaultRequestHeaders.Add("uid", Constants.uid);
            client.DefaultRequestHeaders.Add("access-token", Constants.accessToken);
            //Debug.WriteLine("1");
            var uri = new Uri(REQUEST_URL);
            JObject result = null;
            //Debug.WriteLine("2");
            if (Constants.isDebug)
            {
                //Debug.WriteLine("3");
                callback.OnSuccess(result, this);
                return null;
            }
            try
            {
                //Debug.WriteLine("4");
                HttpResponseMessage response = null;
                response = await client.GetAsync(uri);
                var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JObject.Parse(stringResult);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    callback.OnSuccess(result, this);
                }
                else
                {
                    callback.OnError(result["error"].ToString(), this);
                }
                //Debug.WriteLine(result.ToString());
                return result;

            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception" + ex.Message);
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Another exception " + ex.Message);
                //callback.OnError(result[""]);
                return result;
                throw new Exception("basdfs");
            }
        }

        public async Task<JObject> PagegetRequest(string url)
        {
            var uri = new Uri(url);
            JObject result = null;
            if (Constants.isDebug)
            {
                callback.OnSuccess(result, this);
                return null;
            }
            try
            {
                HttpResponseMessage response = null;
                response = await client.GetAsync(uri);
                var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = JObject.Parse(stringResult);
                callback.OnSuccess(result, this);
                Debug.WriteLine(result.ToString());
                return result;

            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception" + ex.Message);
                var respons = (HttpWebResponse)ex.Response;
                var dataStream = respons.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var serverMessage = reader.ReadToEnd();
                result = JObject.Parse(serverMessage);
                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Another exception " + ex.Message);
                throw new Exception("basdfs ");
            }
        }

        public void OnSuccess(JObject response, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

        public void OnError(string errMsg, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }
    }
}
