using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace X
{
    public static class Cacher
    {
        public static string KEY_SYNC = "xSync";
        public static string KEY_USER = "USER";
         

        public static void LastSync(String date)
        {
            CacheSettings.EmojiSync = date;
        }

        public static String getLastSync()
        {
            return CacheSettings.EmojiSync;
        }

        public static void SaveCurrentUser(String id, string access, string client, string uid)
        {
            CacheSettings.currentUser = id;
            CacheSettings.client = client;
            CacheSettings.accessToken = access;
            CacheSettings.uid = uid;
        }
        public static string getCurrentUser()
        {
            return CacheSettings.currentUser;
        }

        public static string getClient()
        {
            return CacheSettings.client;
        }

        public static string getAccessToken()
        {
            return CacheSettings.accessToken;
        }

        public static string getUid()
        {
            return CacheSettings.uid;
        }
        public static void saveNearestID(string id)
        {
            CacheSettings.nearestID = id;
        }

        public static string getNearestID()
        {
            return CacheSettings.nearestID;
        }


        //public static void cache(string key, object value)
        //{
        //  if(Application.Current.Properties.ContainsKey(key))
        //      Application.Current.Properties.Remove(key);


        //  Application.Current.Properties.Add(key, value);
        //  Application.Current.SavePropertiesAsync();
        //  Log.e("SAVED VALUE", Application.Current.Properties[key]+"....");
        //}


        //public static object getValue(string key)
        //{
        //  object value =null;
        //  try
        //  {
        //      value = Application.Current.Properties["APToken222"];
        //  }
        //  catch (Exception e)
        //  {
        //      Log.e("VALUEEXCEPTION", key +" - "+e.Message);
        //  }

        //  return value;
        //}

        //public static void logout()
        //{
        //    StoreFBAccessToken(null);
        //    //cache(KEY_CURRENT_USER_ID, "");
        //    CacheSettings.CurrentUserID = "";
        //    CacheSettings.CurrentUserAuthToken = "";
        //    CacheSettings.FbToken = "";
        //}

        //public static void StoreFBAccessToken(FbAccessToken token)
        //{
        //    if (token != null)
        //    {
        //        CacheSettings.FbToken = token.Token;
        //        CacheSettings.Permissions = StringValue(token.Permissions);
        //        CacheSettings.FBDeclinedPermissions = StringValue(token.DeclinedPermissions);
        //        CacheSettings.FBAppId = token.ApplicationId;
        //        CacheSettings.FBUserId = token.UserId;
        //        CacheSettings.TokenSource = token.AccessTokenSource;
        //        CacheSettings.TokenExpiry = token.ExpirationTime;
        //        CacheSettings.TokenLastRefresh = token.LastRefreshTime;
        //    }
        //    else
        //    {
        //        CacheSettings.FbToken = "";
        //        CacheSettings.Permissions = "";
        //        CacheSettings.FBDeclinedPermissions = "";
        //        CacheSettings.FBAppId = "";
        //        CacheSettings.FBUserId = "";
        //        CacheSettings.TokenSource = AccessTokenSource.NONE;
        //        CacheSettings.TokenExpiry = DateTime.Now;
        //        CacheSettings.TokenLastRefresh = DateTime.Now;
        //    }
        //}
        //public static FbAccessToken GetFBAccessToken()
        //{
        //    FbAccessToken token = new FbAccessToken();


        //    string tokenString = CacheSettings.FbToken;
        //    token.Token = tokenString;

        //    if (!tokenString.Equals(""))
        //    {
        //        string permissions = CacheSettings.Permissions;
        //        token.Permissions = JsonConvert.DeserializeObject<List<string>>(permissions);

        //        string declinedPermisions = CacheSettings.FBDeclinedPermissions;
        //        token.DeclinedPermissions = JsonConvert.DeserializeObject<List<string>>(declinedPermisions);

        //        token.ApplicationId = CacheSettings.FBAppId;
        //        token.UserId = CacheSettings.FBUserId;
        //        token.AccessTokenSource = CacheSettings.TokenSource;
        //        token.ExpirationTime = CacheSettings.TokenExpiry;
        //        token.LastRefreshTime = CacheSettings.TokenLastRefresh;

        //        return token;
        //    }
        //    else return null;
        //}


        static string StringValue(ICollection<string> l)
        {

            List<string> list = new List<string>();

            foreach (string s in l)
                list.Add(s);

            return JsonConvert.SerializeObject(list);
        }
    }
}
