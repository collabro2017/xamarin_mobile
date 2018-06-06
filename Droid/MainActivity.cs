using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
//using PushNotification.Plugin;
using X.Helpers;
using Android.Graphics;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using CarouselView.FormsPlugin.Android;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android;

namespace X.Droid
{   
    [Activity(Label = "Scuttle", Icon = "@drawable/ic_launcher", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation , LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {   
        public static Context AppContext;

        protected override void OnCreate(Bundle bundle)
        {   
            AppCenter.Start("2298673e-db16-41c0-aa75-086a6713e52f", typeof(Analytics), typeof(Crashes));
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Websockets.Droid.WebsocketConnection.Link();

            base.OnCreate(bundle);
            //pushnotification
            global::Xamarin.Forms.Forms.Init(this, bundle);
            //Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            CarouselViewRenderer.Init();
             
            // CrossPushNotification.Initialize<CrossPushNotificationListener>("KEY watsoever");
            //CrossPushNotification.Current.Register();

            // get screen width and height
            //Point size = new Point();
            Android.Util.DisplayMetrics size = new Android.Util.DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(size);
            App.ScreenWidth = size.WidthPixels / Resources.DisplayMetrics.Density;
            App.ScreenHeight = size.HeightPixels / Resources.DisplayMetrics.Density;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected async override void OnNewIntent(Intent intent)
		{   
            await Task.Delay(60000);
            App.localNotif.ShareNow();
            App.androidFetching = false;
            base.OnNewIntent(intent);
		}
		//public static void StartPushService()
		//{
		//    AppContext.StartService(new Intent(AppContext, typeof(PushNotificationService)));

		//    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
		//    {

		//        PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(PushNotificationService)), 0);
		//        AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
		//        alarm.Cancel(pintent);
		//    }
		//}

		//public static void StopPushService()
		//{
		//    AppContext.StopService(new Intent(AppContext, typeof(PushNotificationService)));
		//    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
		//    {
		//        PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(PushNotificationService)), 0);
		//        AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
		//        alarm.Cancel(pintent);
		//    }
		//}
	}
}
