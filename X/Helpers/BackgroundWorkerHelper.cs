using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace X
{
    public class StartLongRunningTaskMessage { }
    public class StopLongRunningTaskMessage { }
    public class BackgroundWorkerHelper
    {
        public static Action action;
        public static void startTask (Action action)
        {
            BackgroundWorkerHelper.action = action;
            StartBackgroundTask ();
        }
        public static void StartBackgroundTask ()
        {
            var message = new StartLongRunningTaskMessage ();
            MessagingCenter.Send (message, "StartLongRunningTaskMessage");
        }
        public static void StopBackgroundTask ()
        {
            var message = new StopLongRunningTaskMessage ();
            MessagingCenter.Send (message, "StopLongRunningTaskMessage");
        }
        public static async Task BackgroundTask (CancellationToken token)
        {
            await Task.Run (async () => {
                action.Invoke ();
            },token);
        }
    }
}
