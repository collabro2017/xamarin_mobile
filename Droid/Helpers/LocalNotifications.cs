using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using X.Droid.Helpers;
using X.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalNotifications))]
namespace X.Droid.Helpers
{
    public class LocalNotifications : ILocalNotification
    {
        public void Checker()
        {
            
        }

        public void recieveFromLocal(string response)
        {
            
        }

        public async void sendLocalNotif(string name)
        {   
            Intent intent = new Intent(Forms.Context, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent =
                PendingIntent.GetActivity(Forms.Context, pendingIntentId, intent, PendingIntentFlags.UpdateCurrent);
            
            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(Forms.Context)
                .SetContentTitle("Scuttle")
                .SetContentIntent(pendingIntent)
                .SetContentText("Hey! Are you at " + name + "?")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetPriority((int)(NotificationPriority.High))
                .SetSmallIcon(Resource.Drawable.ic_launcher);
                
           
            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager = Forms.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
            Constants.fromnotif = true;

            
        }

        public void ShareNow()
        {
            
        }
    }
}
