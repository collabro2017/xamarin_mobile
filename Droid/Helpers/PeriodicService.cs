using System;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;

namespace X.Droid.Helpers
{
    [Service]
    public class PeriodicService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // From shared code or in your PCL

            Console.WriteLine("ANDROID BACKGROUND RUNNING");
            return StartCommandResult.NotSticky;
        }


    }
}
