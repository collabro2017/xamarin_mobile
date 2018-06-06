using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarouselView.FormsPlugin.iOS;
using Foundation;
using Google.Maps;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PushNotification.Plugin;
using UIKit;
using UserNotifications;
using X.Helpers;
using X.iOS.Helpers;
using Xamarin.Forms;


namespace X.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {   
        string MapsApiKey = Constants.MAP_API;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {       
            Websockets.Ios.WebsocketConnection.Link();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Request Permissions
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>{ });
                UNUserNotificationCenter.Current.Delegate = new UserNotificationDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                app.RegisterUserNotificationSettings(notificationSettings);
            }
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);


            //AppCenter.Start("db95f5e3-ae95-4cf6-bb83-e7e244949bee", typeof(Analytics), typeof(Crashes));
            AppCenter.Start("db95f5e3-ae95-4cf6-bb83-e7e244949bee", typeof(Analytics), typeof(Crashes));
            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
            global::Xamarin.Forms.Forms.Init();
            CarouselViewRenderer.Init();

            MapServices.ProvideAPIKey(MapsApiKey);

            LoadApplication(new App());
            CrossPushNotification.Initialize<CrossPushNotificationListener>();
            CrossPushNotification.Current.Register();
            Xamarin.FormsMaps.Init();
            Xamarin.FormsGoogleMaps.Init(Constants.MAP_API);

            // get screen width and height
           

            System.Diagnostics.Debug.WriteLine("Apple screen size: {0}", UIScreen.MainScreen.Bounds);

            return base.FinishedLaunching(app, options);
        
        }

       
        //iOS 10 below notification
        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.ActionSheet);

            if (notification.AlertTitle.Equals("LocationCheck"))
            {
                okayAlertController.AddAction(UIAlertAction.Create("Yes. Share my experience!", UIAlertActionStyle.Default, alertAction => ShareRating()));
                okayAlertController.AddAction(UIAlertAction.Create("Yes. Remind me later to share", UIAlertActionStyle.Default, alertAction => ThirtyMinutes()));
                okayAlertController.AddAction(UIAlertAction.Create("Not this time", UIAlertActionStyle.Default, alertAction => NoAction()));
            }
            else if(notification.AlertTitle.Equals("Reminder2"))
            {
                
                okayAlertController.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, alertAction => ShareRating()));
                //okayAlertController.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Default, alertAction => YesAction()));

            }else if(notification.AlertTitle.Equals("ShareSchedule"))
            {
                
            }

            var window = UIApplication.SharedApplication.KeyWindow;
            window.RootViewController.PresentViewController(okayAlertController, true, null);
            
            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
       
        public void ShareRating()
        {
            if (App.localNotif != null)
            {
                App.localNotif.recieveFromLocal("sendRating");
            }
        }

        public void ThirtyMinutes()
        {
            if (App.localNotif != null)
            {
                App.localNotif.recieveFromLocal("30");
            }
        }
       
        public void YesAction()
        {
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(2);

            // configure the alert
            notification.AlertTitle = "ShareSchedule";
            notification.AlertAction = "Scuttle";
            notification.AlertBody = "When can you share?";

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void NoAction()
        {
            Console.WriteLine("No Clicked");

        }

        //BackgroundTask
        //public async override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    int X= 0;
        //    bool checker = true;
        //    // Check for new data, and display it
        //    Console.WriteLine("TestPerformFetch");
        //    //DependencyService.Get<ILocalNotification>().sendLocalNotif("Appstone");
        //    while (checker)
        //    {
        //        await Task.Delay(2000);
        //        Console.WriteLine(checker);
        //        if (Constants.canCheck)
        //        {
        //            App.localNotif.Checker();

        //        }
        //    }
        //    // Inform system of fetch results
        //    completionHandler(UIBackgroundFetchResult.NewData);
        //}


        public override void DidEnterBackground(UIApplication application)
        {
            nint taskID = UIApplication.SharedApplication.BeginBackgroundTask(() => { });
            new Task( async () => {
                while (Constants.canCheck)
                {
                    await Task.Delay(60000);
                    App.localNotif.Checker();
                }
                UIApplication.SharedApplication.EndBackgroundTask(taskID);
            }).Start();
        }
        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            application.RegisterForRemoteNotifications();
        }

        //      Uncomment if using remote background notifications. To support this background mode, enable the Remote notifications option from the Background modes section of iOS project properties. (You can also enable this support by including the UIBackgroundModes key with the remote-notification value in your app�s Info.plist file.)
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            //App.Current.MainPage.DisplayAlert("Accountapal", userInfo.ToString(), "OK");
            //App.setBadge(int.Parse(userInfo["notif_count"].ToString()));
            if (CrossPushNotification.Current is IPushNotificationHandler)
            {
                ((IPushNotificationHandler)CrossPushNotification.Current).OnMessageReceived(userInfo);
              }
        }
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            //App.setBadge(int.Parse(userInfo["notif_count"].ToString()));
            if (CrossPushNotification.Current is IPushNotificationHandler)
            {
                ((IPushNotificationHandler)CrossPushNotification.Current).OnMessageReceived(userInfo);

            }
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            if (CrossPushNotification.Current is IPushNotificationHandler)
            {
                ((IPushNotificationHandler)CrossPushNotification.Current).OnErrorReceived(error);
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            if (CrossPushNotification.Current is IPushNotificationHandler)
            {
                ((IPushNotificationHandler)CrossPushNotification.Current).OnRegisteredSuccess(deviceToken);
            }
        }
    }
}
