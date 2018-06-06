 

using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
namespace X.iOS
{
    public class BackgroundWorkerHelperIOS
    {
        nint _taskId;
        CancellationTokenSource _cts;
        private static BackgroundWorkerHelperIOS longRunningTaskExample;
        public static void WireUpLongRunningTask (object owner)
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage> (owner, "StartLongRunningTaskMessage", async message => {
                longRunningTaskExample = new BackgroundWorkerHelperIOS ();
                await longRunningTaskExample.Start ();
            });
            MessagingCenter.Subscribe<StopLongRunningTaskMessage> (owner, "StopLongRunningTaskMessage", message => {
                longRunningTaskExample.Stop ();
            });
        }
        public async Task Start ()
        {
            _cts = new CancellationTokenSource ();
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask ("LongRunningTask", OnExpiration);
            try {
                //INVOKE THE SHARED CODE
                await BackgroundWorkerHelper.BackgroundTask (_cts.Token);
            } catch (OperationCanceledException) {
            } finally {
                if (_cts.IsCancellationRequested) {
                    //var message = new CancelledMessage ();
                    //Device.BeginInvokeOnMainThread (
                    //  () => MessagingCenter.Send (message, "CancelledMessage")
                    //);
                }
            }
            UIApplication.SharedApplication.EndBackgroundTask (_taskId);
        }
        public void Stop ()
        {
            _cts.Cancel ();
        }
        void OnExpiration ()
        {
            _cts.Cancel ();
        }
    }
}