using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Geolocator;
using X.Helpers;
using X.Models;
using X.Pages;
using Xamarin.Forms;

namespace X
{
    public partial class App : Application
    {    
        public  static slqLiteHelper database;
        public static Page currentPage { get; set; }
        public static CancellationTokenSource CancellationToken { get; set; }
        public static double ScreenWidth { get; set; }
        public static double ScreenHeight { get; set; }
        public static string RestoName { get; set; }
        public static ILocalNotification localNotif;
        public static bool androidFetching { get; set; }


        public static double scale
        {
            get { return (ScreenWidth + ScreenHeight) / (320.0f + 568.0f); } //Every measure is based on iOS 320 scale
        }

        public App()
        {
            InitializeComponent();
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                Constants.is_android = true;
            }
            else
            {
                Constants.is_android = false;
            }

            if (Constants.is_android)
            {
                MainPage = new Login();
            }
            else if (Cacher.getCurrentUser().Length > 0)
            {
                Constants.accessToken = Cacher.getAccessToken();
                Constants.uid = Cacher.getUid();
                Constants.client = Cacher.getClient();
                MainPage = new MainPage();
            }
            else
            {
                MainPage = new Login();
            }
          

        }

        public void NotificationResult()
        {

        }

        public static slqLiteHelper Database
        {
            get
            {
                if (database == null)
                {
                    database = new slqLiteHelper(DependencyService.Get<IFileHelper>().GetLocalFilePath("TodoSQLite.db3"));
                }
                return database;
            }
        }
      
        protected override void OnStart()
        {
            // Handle when your app starts
             
            Constants.OnForeground = true;
        }

        protected  override void OnSleep()
        {
            androidFetching = true;
            System.Threading.Tasks.Task.Run( async () =>
            {
                while(androidFetching)
                {
                   await Task.Delay(60000);
                    localNotif.Checker();
                }
                //Add your code here.
            }).ConfigureAwait(false);
            //while (Constants.canCheck)
            //{
            //    await  Task.Delay(2000);
            //    localNotif.Checker();
            //}

            Constants.OnForeground = false;
        }

        protected override void OnResume()
        {    
            if(App.CancellationToken != null)
            {
                App.CancellationToken.Cancel();
            }
            Debug.WriteLine("App on foreground");

            Constants.OnForeground = true;
        }
    }
}
