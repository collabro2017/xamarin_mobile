using System;
using System.Collections.Generic;
using Plugin.Geolocator.Abstractions;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace X
{
    public class CacheSettings
    {

        private static ISettings AppSettings =>
    CrossSettings.Current;

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get => AppSettings.GetValueOrDefault(nameof(GeneralSettings), string.Empty);
            
            set => AppSettings.AddOrUpdateValue(nameof(GeneralSettings), value);

        }


        //private const string KEY_SYNC = "xSync";
        //private static readonly string KEY_SYNC_DEF = "";

        public static string EmojiSync
        {
            get => AppSettings.GetValueOrDefault(nameof(EmojiSync),SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(EmojiSync), value);
        }

        public static string currentUser
        {
            get => AppSettings.GetValueOrDefault(nameof(currentUser), SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(currentUser), value);
        }

        public static string accessToken
        {
            get => AppSettings.GetValueOrDefault(nameof(accessToken), SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(accessToken), value);
        }
        public static string client
        {
            get => AppSettings.GetValueOrDefault(nameof(client), SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(client), value);
        }
        public static string uid
        {
            get => AppSettings.GetValueOrDefault(nameof(uid), SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(uid), value);
        }

        public static string nearestID
        {
            get => AppSettings.GetValueOrDefault(nameof(nearestID), SettingsDefault);
            set => AppSettings.AddOrUpdateValue(nameof(nearestID), value);
        }
         
        /// <summary>
        /// Gets or sets the user location.
        /// Returns new Position(0,0) if no location previously stored
        /// </summary>
        /// <value>The user location.</value>
        public static Position userLocation
        {
            get
            {
                var lat = AppSettings.GetValueOrDefault($"{nameof(userLocation)}Lat", default(double));
                var lon = AppSettings.GetValueOrDefault($"{nameof(userLocation)}Lon", default(double));
                return new Position(lat, lon);
            }
            set
            {
                AppSettings.AddOrUpdateValue($"{nameof(userLocation)}Lat", value.Latitude);
                AppSettings.AddOrUpdateValue($"{nameof(userLocation)}Lon", value.Latitude);
            }
        }
    }
}
