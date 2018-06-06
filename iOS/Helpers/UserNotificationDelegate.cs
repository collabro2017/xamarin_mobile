using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;
using X.Helpers;
using X.iOS.Helpers;
using X.Pages;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserNotificationDelegate))]
namespace X.iOS.Helpers
{
    public class UserNotificationDelegate : UNUserNotificationCenterDelegate,ILocalNotification
    {

        //public UserNotificationDelegate()
        //{
        //var content = new UNMutableNotificationContent();
        //content.Title = "Scuttle";
        //content.Subtitle = " ";
        //content.Body = "Hey! Are you at "+App.RestoName;
        //content.Badge = 1;

        //var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);

        //var requestID = "sampleRequest";
        //var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

        //UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
        //{
        //    if (err != null)
        //    {
        //        // Do something with error...
        //    }
        //});

        //WillPresentNotification(UNUserNotificationCenter.Current, content, (UNNotificationPresentationOptions obj) => 
        //{

        //});
        //}
        #region ask if the user is in a restaurant
        public void sendLocalNotif(string name)
        {
            if (name.Equals("reminder"))
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {

                    //var actionID = "yes2";
                    //var title = "Yes";
                    //var action = UNNotificationAction.FromIdentifier(actionID, title, UNNotificationActionOptions.None);

                    //var actionID2 = "no2";
                    //var title2 = "No";
                    //var action2 = UNNotificationAction.FromIdentifier(actionID2, title2, UNNotificationActionOptions.None);

                    //// Create category
                    //var categoryID = "message";
                    //var actions = new UNNotificationAction[] { action, action2 };
                    //var intentIDs = new string[] { };
                    //var categoryOptions = new UNNotificationCategoryOptions[] { };
                    //var category = UNNotificationCategory.FromIdentifier(categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

                    //// Register category
                    //var categories = new UNNotificationCategory[] { category };
                    //UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

                    
                    var content = new UNMutableNotificationContent();
                    content.Title = "Scuttle";
                    content.Subtitle = " ";
                    content.Body = "Are you ready to share your experience?";
                    content.Badge = 1;
                    //content.CategoryIdentifier = "message";

                    var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(2, false);
                    var requestID = "sampleRequest";
                    var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

                    UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                    {
                        if (err != null)
                        {
                            // Do something with error...
                        }
                    });
                }
                else
                {
                    // create the notification
                    var notification = new UILocalNotification();
                    // set the fire date (the date time in which it will fire)
                    notification.FireDate = NSDate.FromTimeIntervalSinceNow(2);
                    // configure the alert
                    notification.AlertTitle = "Reminder2";
                    notification.AlertAction = "Scuttle";
                    notification.AlertBody = "Are you ready to share your experience?";

                    // modify the badge
                    notification.ApplicationIconBadgeNumber = 1;

                    // set the sound to be the default sound
                    notification.SoundName = UILocalNotification.DefaultSoundName;

                    // schedule it
                    UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                }
            }
            else
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    // Create action
                    var actionID = "yes";
                    var title = "Yes. Share my experience!";
                    var action = UNNotificationAction.FromIdentifier(actionID, title, UNNotificationActionOptions.Foreground);

                    var actionID3 = "RemindMeLater";
                    var title3 = "Yes. Remind me later to share";
                    var action3 = UNNotificationAction.FromIdentifier(actionID3, title3, UNNotificationActionOptions.None);
                    var actionID2 = "no";
                    var title2 = "Not this time";
                    var action2 = UNNotificationAction.FromIdentifier(actionID2, title2, UNNotificationActionOptions.None);

                    // Create category
                    var categoryID = "message";
                    var actions = new UNNotificationAction[] { action, action3,action2 };
                    var intentIDs = new string[] { };
                    var categoryOptions = new UNNotificationCategoryOptions[] { };
                    var category = UNNotificationCategory.FromIdentifier(categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

                    // Register category
                    var categories = new UNNotificationCategory[] { category };
                    UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

                    var content = new UNMutableNotificationContent();
                    content.Title = "Scuttle";
                    content.Subtitle = " ";
                    content.Body = "Hey! Are you at " + name + "?";
                    content.Badge = 1;
                    content.CategoryIdentifier = "message";

                    var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(2, false);

                    var requestID = "sampleRequest";
                    var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

                    UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                    {
                        if (err != null)
                        {
                        // Do something with error...
                    }
                    });
                }
                else
                {
                    // create the notification
                    var notification = new UILocalNotification();

                    // set the fire date (the date time in which it will fire)
                    notification.FireDate = NSDate.FromTimeIntervalSinceNow(5);

                    // configure the alert
                    notification.AlertTitle = "LocationCheck";
                    notification.AlertAction = "Scuttle";
                    notification.AlertBody = "Hey! Are you at " + name + "?";

                    // modify the badge
                    notification.ApplicationIconBadgeNumber = 1;

                    // set the sound to be the default sound
                    notification.SoundName = UILocalNotification.DefaultSoundName;

                    // schedule it
                    UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                }
            }
        }
        #endregion

        #region ask when can the user share
        public void whenCanUShare()
        {
            // Create action
            var actionID = "ten";
            var title = "10 minutes";
            var action = UNNotificationAction.FromIdentifier(actionID, title, UNNotificationActionOptions.Foreground);

            var actionID2 = "fifteen";
            var title2 = "15 minutes";
            var action2 = UNNotificationAction.FromIdentifier(actionID2, title2, UNNotificationActionOptions.None);

            var actionID3 = "thirty";
            var title3 = "30 minutes";
            var action3 = UNNotificationAction.FromIdentifier(actionID3, title3, UNNotificationActionOptions.None);

            var actionID4 = "leave";
            var title4 = "When I leave";
            var action4 = UNNotificationAction.FromIdentifier(actionID4, title4, UNNotificationActionOptions.None);

            // Create category
            var categoryID = "remindlater";
            var actions = new UNNotificationAction[] { action, action2, action3, action4 };
            var intentIDs = new string[] { };
            var categoryOptions = new UNNotificationCategoryOptions[] { };
            var category = UNNotificationCategory.FromIdentifier(categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

            // Register category
            var categories = new UNNotificationCategory[] { category };
            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

            var content = new UNMutableNotificationContent();
            content.Title = "Scuttle";
            content.Subtitle = " ";
            content.Body = "When can you share?";
            content.Badge = 1;
            content.CategoryIdentifier = "remindlater";

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(2, false);

            var requestID = "sampleRequest";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    // Do something with error...
                }
            });
        }
        #endregion

        #region Override Methods
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification
            Console.WriteLine("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
        #endregion

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // Take action based on Action ID
            switch (response.ActionIdentifier)
            {
                case "yes":

                    App.localNotif.recieveFromLocal("sendRating");

                    break;
                case "RemindMeLater": 
                    App.localNotif.recieveFromLocal("30");
                    break;
                case "no":
                    // Do something
                    break;
                //case "ten":
                //    break;
                //case "fifteen":
                //    App.localNotif.recieveFromLocal("15");
                //    break;
                //case "thirty":
                //    App.localNotif.recieveFromLocal("30");
                //    break;
                //case "leave":
                    //App.localNotif.recieveFromLocal("Leave");
                    //// Do something
                    //break;
                //case "yes2" :
                //    App.localNotif.recieveFromLocal("sendRating");
                //    break;
                //case "no2":
                    //whenCanUShare();
                    //break;
                default:
                    // Take action based on identifier
                    if (response.IsDefaultAction)
                    {
                        // Handle default action...
                    }
                    else if (response.IsDismissAction)
                    {
                        // Handle dismiss action
                    }
                    break;
            }

            // Inform caller it has been handled
            completionHandler();
        }

        public void recieveFromLocal(string response)
        {
        }
        public void ShareNow()
        {
        }

        public void Checker()
        {
        }
    }
}
    
 
