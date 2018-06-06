using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using X.Helpers;
using X.iOS.Tools;

[assembly: Xamarin.Forms.Dependency(typeof(iOSLocationSettingsService))]
namespace X.iOS.Tools
{
    public class iOSLocationSettingsService : ILocationSettingsService
    {
        TaskCompletionSource<String> completionSource;

        public Task<String> OpenLocationSettings()
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString), new NSDictionary(), (bool obj) => {

                completionSource.SetResult("Okay");
            });

            completionSource = new TaskCompletionSource<String>();

            return completionSource.Task;
        }

    }
}
