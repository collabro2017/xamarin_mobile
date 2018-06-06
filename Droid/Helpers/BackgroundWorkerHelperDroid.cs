using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Forms;
namespace X.Droid
{
    [Service]
    class BackgroundWorkerHelperDroid : Service
    {
        CancellationTokenSource _cts;
        public static void WireUpLongRunningTask(Context owner)
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(owner, "StartLongRunningTaskMessage", message => {
                var intent = new Intent(owner, typeof(BackgroundWorkerHelperDroid));
                owner.StartService(intent);
            });
            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(owner, "StopLongRunningTaskMessage", message => {
                var intent = new Intent(owner, typeof(BackgroundWorkerHelperDroid));
                owner.StopService(intent);
            });
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => {
                try
                {
                    //INVOKE THE SHARED CODE
                    //var counter = new TaskCounter ();
                    //counter.RunCounter (_cts.Token).Wait ();
                    BackgroundWorkerHelper.BackgroundTask(_cts.Token).Wait();
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        //var message = new CancelledMessage ();
                        //Device.BeginInvokeOnMainThread (
                        //  () => MessagingCenter.Send (message, "CancelledMessage")
                        //);
                    }
                }
            }, _cts.Token);
            return StartCommandResult.Sticky;
        }
        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();
                _cts.Cancel();
            }
            base.OnDestroy();
        }
    }
}

