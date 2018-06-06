using Foundation;
using UIKit;
using X.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(Settings))]
public class Settings : ISettingsService
{
    public bool OpenSettings()
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
        {
            // For iOS 8 and 9, we can navigate automatically to the settings
            NSUrl url = new NSUrl(UIKit.UIApplication.OpenSettingsUrlString);
            bool result = UIApplication.SharedApplication.OpenUrl(url);
        }
        else
        {
            // Below iOS 8, the user has to navigate manually to the settings
            UIAlertView uiAlert = new UIAlertView(
                "Location Services Required",
                "Authorisation to use your location is required to use this feature of the app. To use this feature please go to the settings app and enable location services",
                null,
                "Ok");
            uiAlert.Show();
        }
        return false;
    }
}