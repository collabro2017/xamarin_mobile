using System;
using System.Collections.Generic;
using X.API;
using Xamarin.Forms;

namespace X
{
    public class Constants
    {
        public static bool isDebug = false;
        public static bool OnForeground ;
        //public static string SERVER_URL = "http://198.58.101.247:58194";
        public static string SERVER_URL = "http://198.58.101.247:57552";
        //public static string SERVER_URL = "http://192.168.1.200:2000";
        public static string MAP_API = "AIzaSyCgdmsIf4nqwhAHu4K206uB5A_jVsC4oZI";
        public static bool OnSearch = false;
        public static bool OnInit = false;
        public static int ChatServerPort = 2000;
        public static string accessToken;
        public static string client;
        public static string uid;
        public static bool OnRegister = false;
        public static bool SettingsInit = false;
        public static bool OnLogin = false;
        public static bool is_android;
        public static bool fromnotif;
        public static bool canCheck;
        public static bool onforgotPassword;
        public static List<int> CurrentCategoryOrder;
        public static User current_user;
        public static int animatedcount = 0;
        public static String ChatServerUrl = "http://198.58.101.247";
        public static string RestoName;
        public static float androidMapRadius = 0;
        public static bool canSendNotification = false;
        public static bool NotificationAllowed = false;
        public Constants()
        {
        }
         
    }
}
