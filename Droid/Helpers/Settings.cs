using Android.Content;
using Android.Locations;
using X.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(Settings))]
public class Settings: ISettingsService
{
    public bool OpenSettings()
    {
        Xamarin.Forms.Forms.Context.StartActivity(new Android.Content.Intent(Android.Provider.Settings.ActionLocat​ionSourceSettings));

        LocationManager locationManager = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);

        if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == true)
        {
            return true;
        }
        return false;
    }
}