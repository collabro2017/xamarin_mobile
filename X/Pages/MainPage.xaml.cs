using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using X.API;
using X.API.Base;
using X.Helpers;
using X.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static X.CustomMap;

namespace X.Pages
{
    public partial class MainPage : ContentPage, IBaseAPIInterface, Iplaces, ILocalNotification
    {

        Plugin.Geolocator.Abstractions.Position currentLocation;
        CancellationTokenSource cancel;
        CancellationTokenSource locationCancelation = new CancellationTokenSource();
        CancellationTokenSource animatecancel = new CancellationTokenSource();
        Locations SelectedPinInfo;
        JToken latest_ratings;
        PinModel nearestPin = new PinModel();
        ObservableCollection<Locations> searchlist = new ObservableCollection<Locations>();
        ObjectPool<Image> imgPool;
        public List<OnCooldownEmoji> OnCoolDown = new List<OnCooldownEmoji>();
        PermissionStatus locationPermission = new PermissionStatus();
        Double current_lat = 0, current_lng = 0;
        JObject SearchedPlaces;
        ActionCableClient client = new ActionCableClient("ws://198.58.101.247:57552/cable", "PlacesChannel");
        DateTime lastFoodEmojiSent;
        DateTime lastServiceEmojiSent;
        DateTime lastCleanlinessEmojiSent;
        DateTime lastVibeEmojiSent;
        DateTime endTimer;
        DateTime searchTimeStamp;
        DateTime notifcationcountDown;
        bool notyetsentnotif;
        bool canSendEmoji = false;
        bool animateTaskRunning = false;
        bool onSearch;
        bool Opened = false;
        bool timerInit;
        bool sendingEmoji = false;
        bool bgtapped = false;
        int emojirating_id;

        public MainPage()
        {

            InitializeComponent();
            Constants.onforgotPassword = false;
            Constants.OnLogin = false;
            Constants.OnRegister = false;
            App.localNotif = this;
            searchbar.WidthRequest = 250 * App.scale;

            if (ConnectedToInternet())
            {

                if (locationPermission == PermissionStatus.Granted)
                {
                    Constants.OnInit = false;
                }

                notyetsentnotif = true;
                scalesizes();
                askpermissions();
                if (Constants.CurrentCategoryOrder == null)
                {
                    FetchUserAPI api = new FetchUserAPI();
                    api.setCallbacks(this);
                    api.getResponse();
                }
                else
                {
                    FetchEmojis();
                }
                info_view.IsVisible = false;
                timerInit = true;

                //startLocationUpdates();
                //setUpsocket();
                //info_view.TranslationY = 100;
                imgPool = new ObjectPool<Image>();
                App.currentPage = this;
                client.ConnectAsync();
                client.IgnorePings = true;
                client.MessageReceived += NewRatingRecieved;



                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (DateTime.Now.Subtract(TimeSpan.FromSeconds(2)) >= searchTimeStamp && onSearch)
                    {
                        if (SearchedPlaces["restaurants"].Value<JArray>().Count > 0 && SearchedPlaces != null)
                        {
                            if (Constants.is_android)
                            {
                                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(SearchedPlaces["restaurants"][0]["lat"].Value<double>(), SearchedPlaces["restaurants"][0]["lng"].Value<double>()), Distance.FromMeters(20)));
                            }else
                            {
                                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(SearchedPlaces["restaurants"][0]["lat"].Value<double>(), SearchedPlaces["restaurants"][0]["lng"].Value<double>()), map.VisibleRegion.Radius));

                            }
                            map.Pins.Clear();
                            map.CustomPin.Clear();
                            var ctr = 0;

                            foreach (var Places in SearchedPlaces["restaurants"])
                            {
                                var emojilist = new List<string>();
                                foreach (var ratings in Places["latest_ratings"])
                                {
                                    if (ctr < 4)
                                    {
                                        var rating = ratings["rating"];
                                        //Debug.WriteLine(Constants.SERVER_URL + rating["images"]["medium"].Value<String>());
                                        emojilist.Add(rating["image"].Value<String>());
                                    }
                                    ctr++;
                                }
                                PinModel pin = new PinModel();
                                if (Places["latest_ratings"] != null)
                                {

                                    pin = new PinModel
                                    {
                                        Type = Xamarin.Forms.Maps.PinType.Place,
                                        Position = new Xamarin.Forms.Maps.Position(Double.Parse(Places["lat"].ToString()), Double.Parse(Places["lng"].ToString())),
                                        Label = Places["name"].ToString(),
                                        Address = "323e",
                                        Id = Places["place_id"].ToString(),
                                        Url = "http://xamarin.com/about/",
                                        pin = "pin",
                                        Emojis = emojilist,

                                    };
                                }
                                map.Pins.Add(pin);
                                map.CustomPin.Add(pin);
                                //Debug.WriteLine(map.CustomPin.Count + " " + map.Pins.Count);
                            }
                            var lat = SearchedPlaces["restaurants"][0]["lat"].Value<Double>().ToString("0.000000");
                            var lng = SearchedPlaces["restaurants"][0]["lng"].Value<Double>().ToString("0.000000");
                        }
                        latest_pins = SearchedPlaces;
                        onSearch = false;
                    }
                    if (notifcationcountDown <= DateTime.Now && Constants.canSendNotification && Constants.NotificationAllowed)
                    {
                        DependencyService.Get<ILocalNotification>().sendLocalNotif("reminder");
                        Constants.canSendNotification = false;
                    }
                    return true;
                });
            }
            else
            {
                info_view.IsVisible = false;
                DisplayAlert("Scuttle", "No internet connection", "ok");
            }
            searchbar.TextChanged += async (sender, e) =>
            {
                if (e.NewTextValue == "")
                {
                    //noResults.IsVisible = false;
                    listView.IsVisible = false;
                    lvBorder.IsVisible = false;
                    onSearch = false;
                    map.OnSearch = false;
                }
                else
                {
                    if (ConnectedToInternet())
                    {
                        searchTimeStamp = DateTime.Now;
                        //noResults.IsVisible = false;

                        listView.IsRefreshing = true;
                        var query = e.NewTextValue;
                        //var url = "https://maps.googleapis.com/maps/api/place/textsearch/json?key=" + Constants.MAP_API + "&query=" + query;

                        var url = Constants.SERVER_URL + "/api/v1/restaurants?lat=" + current_lat + "&lng=" + current_lng + "&q=" + query;
                        using (var client = new HttpClient())
                        {
                            //client.BaseAddress = new Uri("http://localhost:55587/");
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            HttpResponseMessage response = await client.GetAsync(url);
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            //var Loc = JsonConvert.DeserializeObject<Locations>(jsonResponse);
                            // parsing json
                            SearchedPlaces = JObject.Parse(jsonResponse);
                            var places = SearchedPlaces["restaurants"];
                            searchlist = new ObservableCollection<Locations>();
                            foreach (var place in places)
                            {
                                var Name = place["name"].ToString();
                                string address = place["address"].ToString();
                                double latitude = (double)place["lat"];//.ToObject<double>();
                                double longitude = (double)place["lng"];//.ToObject<double>();
                                //System.Diagnostics.Debug.WriteLine("addr: {0}\tlatitude: {1}\tlatitude: {2}", address, latitude, longitude);
                                var NameAndAddess = new FormattedString();
                                NameAndAddess.Spans.Add(new Span { Text = Name + " ", ForegroundColor = Color.Black, FontFamily = "Helvetica-Light", FontSize = 13 });
                                NameAndAddess.Spans.Add(new Span { Text = address, ForegroundColor = Color.FromHex("#a1a1a1"), FontSize = 13, FontFamily = "Helvetica-Light" });
                                searchlist.Add(new Locations { ID = place["place_id"].ToString(), name = Name, NameAndAddress = NameAndAddess, lat = latitude, lng = longitude, address = address, rating = place["rating"].ToString() });
                              
                            }
                            listView.ItemsSource = searchlist;
                            if (searchlist.Count > 0)
                            {
                                listView.IsVisible = true;
                                lvBorder.IsVisible = true;

                                lvBorder.HeightRequest = 40 * searchlist.Count;
                                lvBorder.VerticalOptions = LayoutOptions.Start;
                                listView.VerticalOptions = LayoutOptions.Start;
                                listView.HeightRequest = 50 * searchlist.Count;
                                //noResults.IsVisible = false;
                            }
                            else
                            {
                                //noResults.IsVisible = true;
                                listView.IsVisible = false;
                                lvBorder.IsVisible = false;
                                lvBorder.HeightRequest = 0;
                                lvBorder.VerticalOptions = LayoutOptions.Start;
                                listView.VerticalOptions = LayoutOptions.Start;
                                listView.HeightRequest = 0;
                            }
                            onSearch = true;
                            map.OnSearch = true;
                            listView.IsRefreshing = false;
                        }
                    }
                    else
                    {
                        DisplayAlert("Scuttle", "No internet connection.", "OK");
                        searchbar.Text = "";
                    }
                }
            };

            searchbar.Focused += (sender, e) =>
            {
                //if (map.OnSearch && searchlist.Count > 0)
                //listView.IsVisible = true;
                //lvBorder.IsVisible = true;
                searchbar.HorizontalOptions = LayoutOptions.StartAndExpand;
                searchbar.Placeholder = "";
                searchbar.TextGravityAlignment = TransparentEntry.TextGravity.START_CENTER;
            };
            searchbar.Unfocused += (sender, e) => { if (searchbar.Text == "") { searchbar.Placeholder = "🔎 Sear�"; searchbar.TextGravityAlignment = TransparentEntry.TextGravity.CENTER; searchbar.HorizontalOptions = LayoutOptions.CenterAndExpand;  } listView.IsVisible = false; lvBorder.IsVisible = false; };
            //map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.GoogleMaps.Position(position.Result.Latitude, position.Result.Longitude),
            //Distance.FromMiles(1))) ;  
            CarouselView.PositionSelected += (object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e) =>
            {
                //cvindicator.cvmoved(int.Parse(e.SelectedPosition.ToString()));
                if (latest_ratings != null)
                {
                    if (animateTaskRunning)
                    {
                        animatecancel.Cancel();
                    }else
                    {
                        animatecancel = new CancellationTokenSource();
                    }
                    animateSpecificRatings(animatecancel.Token);
                } 
            };
            GotolocationAsync();

            //DependencyService.Get<ILocalNotification>().sendLocalNotif("Appstone");
            searchbar.Focus();
            searchbar.Unfocus();

        }



        async void NewRatingRecieved(object s, ActionCableEventArgs e)
        {
            if (!Constants.is_android)
            {
                JObject result = JObject.Parse(e.Message);
                await Task.Delay(2000);
                if (emojirating_id != result["user_rating_id"].Value<int>())
                {
                    if (CarouselView.Position + 1 == result["rating"]["rating_category_id"].Value<int>())
                    {
                        var r = new Random();
                        var emoji = new RatingObject()
                        {
                            //Image = result["rating"]["image"].ToString(),
                        };
                        emoji.Reset();
                        ReactionsContainer.Children.Add(emoji);
                        emoji.AnimateLiveStream(6000);
                        await Task.Delay(750 + (int)(300 * r.NextDouble()));
                    }
                }else
                {
                    FetchPlaceRatingsAPI api = new FetchPlaceRatingsAPI(SelectedPinInfo.ID);
                    api.setCallbacks(this);
                    api.getResponse();

                }
            }        
        }
        public bool ConnectedToInternet()
        {
            if (!CrossConnectivity.IsSupported)
                return true;

            return CrossConnectivity.Current.IsConnected;
        }
        //void Handle_Scrolled(object sender, CarouselView.FormsPlugin.Abstractions.ScrolledEventArgs e)
        //{
        //    Debug.WriteLine("Scrolled to: " + e.NewValue );
        //    if(e.NewValue > 97)
        //    {   
        //        if(e.Direction == CarouselView.FormsP)
        //        CarouselView.Position += 1;
        //    }
        //}
        void FetchEmojis()
        {
            FetchEmojisAPI api = new FetchEmojisAPI();
            api.setCallbacks(this);
            api.getResponse();
        }
        void scalesizes()
        {
            //if (Device.RuntimePlatform.Equals(Device.Android))
            //{
            //    info_name.FontSize = 15;
            //    info_rating.FontSize -= 3;
            //    info_address.FontSize -= 3;
            //    info_distance.FontSize -= 3;
            //    rating_star.WidthRequest -= 5;

            //}
            var scale = App.scale;
            searchbar.FontSize *= scale;
            //info_getDirection.FontSize *= scale;
            info_name.FontSize *= scale;
            rating_star.WidthRequest *= scale;
            showbtn.WidthRequest = 275 * scale;
            info_rating.FontSize *= scale;

            //info_view.Padding = 10 * scale;
        }
        async void askpermissions()
        {
            locationPermission = await checkLocationPermission();
            Constants.OnInit = true;
            GotolocationAsync();

        }
        static async Task<PermissionStatus> checkLocationPermission()
        {

            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {

                if (CrossGeolocator.Current.IsGeolocationEnabled)
                {

                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        //await DisplayAlert("Need location", "Gunna need that location", "Okay");
                    }
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    status = results[Permission.Location];
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Enable Location", "Please enable location settings.", "Okay");
                    bool locationOn = DependencyService.Get<ISettingsService>().OpenSettings();
                    if (locationOn)
                    {
                    }
                }

            }


            if (status == PermissionStatus.Granted)
            {
                if (CrossGeolocator.Current.IsGeolocationEnabled)
                {
                    var browser = new WebView();
                    //var results = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(2));
                    //browser.Source = "https://forecast.io/?mobile=1#/f/" + "Lat: " + results.Latitude + " Long: " + results.Longitude;

                    //System.Diagnostics.Debug.WriteLine("Results: {0} - {1}" , results.Latitude,results.Longitude);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Enable Location", "Please enable location settings.", "Okay");
                    bool locationOn= DependencyService.Get<ISettingsService>().OpenSettings();
                    if(locationOn)
                    {
                    }
                }

            }
            else if (status == PermissionStatus.Disabled)
            {
                await Application.Current.MainPage.DisplayAlert("Enable Location", "Please enable location settings.", "Okay");
                var locSettings = DependencyService.Get<ILocationSettingsService>();
                if (locSettings != null)
                {

                    await locSettings.OpenLocationSettings();
                    //await checkLocationPermission();
                }
            }
            else if (status == PermissionStatus.Denied)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    //await DisplayAlert("Need location", "Gunna need that location", "Okay");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                status = results[Permission.Location];
            }
            else if (status != PermissionStatus.Unknown)
            {
                //await DisplayAlert("Location Denied", "Please enable location services for PP.", "Okay");
            }

            return status;
        }
        async void searchTapped(object s, EventArgs e)
        {
            await Task.Delay(100);
            searchbar.Focus();
        }

        void SelectedPlace(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            SelectedPinInfo = e.SelectedItem as Locations;
            map.OnSearch = true;
            var pin = new PinModel
            {
                Type = Xamarin.Forms.Maps.PinType.Place,
                Position = new Xamarin.Forms.Maps.Position(SelectedPinInfo.lat, SelectedPinInfo.lng),
                Label = SelectedPinInfo.name,
                Address = SelectedPinInfo.address,
                Id = SelectedPinInfo.ID,
                Url = "http://xamarin.com/about/",
                pin = "pin",
                Emojis = new List<string>()
                {
                    "https://cdn.shopify.com/s/files/1/0185/5092/products/persons-0041_large.png?v=1369543932",
                    "https://cdn.shopify.com/s/files/1/0185/5092/products/persons-0024.png?v=1369543702",
                    "https://cdn.shopify.com/s/files/1/0185/5092/products/persons-0035.png?v=1369543848",
                    "https://cdn.shopify.com/s/files/1/0185/5092/products/persons-0036.png?v=1369543508",
                }

            };
            map.Pins.Add(pin);
            map.CustomPin.Add(pin);
            if (Constants.is_android)
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(SelectedPinInfo.lat, SelectedPinInfo.lng), Distance.FromMeters(200)));
            }else
            {
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(SelectedPinInfo.lat, SelectedPinInfo.lng), map.VisibleRegion.Radius));
            }listView.IsVisible = false;
            lvBorder.IsVisible = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //map.MyLocationEnabled = true;
            if (Constants.is_android)
            {

            }
            else
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(map.VisibleRegion.Center.Latitude + 0.00001, map.VisibleRegion.Center.Longitude), map.VisibleRegion.Radius));

            }
        }
        public async void SetUpEmojis()
        {

            var ratings = await App.Database.GetItemsAsync();
            var rFood = new List<Emojis>();
            var rCleanliness = new List<Emojis>();
            var rCustomerService = new List<Emojis>();
            var rVibe = new List<Emojis>();
            foreach (var rating in ratings)
                switch (int.Parse(rating.rating_category_id))
                {
                    case (1): rFood.Add(rating); break;
                    case (2): rCustomerService.Add(rating); break;
                    case (3): rCleanliness.Add(rating); break;
                    case (4): rVibe.Add(rating); break;
                }


            var EmojisCat = new ObservableCollection<EmojiCategory>();
            foreach (var cat in Constants.CurrentCategoryOrder)
            {
                switch (cat)
                {
                    case (1):
                        var cat1 = new EmojiCategory
                        {
                            emoji = "poop",
                            name = "What did you think about the food?",
                            ratingEmoji = rFood,
                        };
                        EmojisCat.Add(cat1);
                        break;
                    case (2):
                        var cat2 = new EmojiCategory
                        {
                            emoji = "cool",
                            name = "What did you think about the service?",
                            ratingEmoji = rCustomerService,

                        };
                        EmojisCat.Add(cat2);
                        break;
                    case (3):
                        var cat3 = new EmojiCategory
                        {
                            emoji = "pin",
                            name = "What did you think about the cleanliness?",
                            ratingEmoji = rCleanliness,
                        };
                        EmojisCat.Add(cat3);
                        break;
                    case (4):
                        var cat4 = new EmojiCategory
                        {
                            emoji = "heart_eyes",
                            name = "What did you think about the vibe?",
                            ratingEmoji = rVibe,

                        };
                        EmojisCat.Add(cat4);
                        break;
                }
            }


            CarouselView.ItemsSource = EmojisCat;
        }
        async void OpenSettings(object e, EventArgs s)
        {   
            var fade = (Image)e;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
            map.Pins.Clear();
            map.CustomPin.Clear();
            await Navigation.PushModalAsync(new SettingsPage());
        }
        void OpenGoogleMaps(Object e, EventArgs s)
        {
            var locator = CrossGeolocator.Current;
            var position = locator.GetLastKnownLocationAsync();
            double lat, lng;

            if (locationPermission == PermissionStatus.Granted)
            {
                lat = position.Result.Latitude;
                lng = position.Result.Longitude;
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var uri = new Uri("http://maps.apple.com/?saddr=" + lat + ",+" + lng + "&daddr=" + SelectedPinInfo.lat + ",+" + SelectedPinInfo.lng + "&directionsmode=Drive");
                    Device.OpenUri(uri);
                }
                else
                {
                    var uri = new Uri("http://maps.google.com/?saddr=" + lat + ",+" + lng + "&daddr=" + SelectedPinInfo.lat + ",+" + SelectedPinInfo.lng + "&directionsmode=Drive");

                    Device.OpenUri(uri);
                }

            }
            else
            {
                lat = map.VisibleRegion.Center.Latitude;
                lng = map.VisibleRegion.Center.Longitude;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    var uri = new Uri("http://maps.apple.com/?daddr=" + SelectedPinInfo.lat + ",+" + SelectedPinInfo.lng + "&directionsmode=Drive");
                    Device.OpenUri(uri);
                }
                else
                {
                    var uri = new Uri("http://maps.google.com/?saddr=&daddr=" + SelectedPinInfo.lat + ",+" + SelectedPinInfo.lng + "&directionsmode=Drive");

                    Device.OpenUri(uri);
                }
            }
        }
        async void backgroundTapped(Object e, EventArgs s)
        {
            if (!sendingEmoji )
            {
                ssloading.IsVisible = true;
                animatecancel.Cancel();
                animateTaskRunning = false;
                map.OnSelect = false;
                searchView.IsVisible = true;
                map.Pins.Clear();
                map.CustomPin.Clear();
                await info_view.TranslateTo(0, 0, 250, Easing.CubicIn);
                menu_icon.IsVisible = true;
                //search_button.IsVisible = true;
                info_view.IsVisible = false;
                live_bg.IsVisible = false;
                Opened = false;
                ReactionsContainer.Children.Clear();
                cvLoading.IsVisible = false;
                endTimer = DateTime.Now;

                if (!map.OnSearch)
                {
                    if (Constants.is_android)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(SelectedPinInfo.lat + 0.00001, SelectedPinInfo.lng), Distance.FromMeters(200)));

                    }
                    else
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(map.VisibleRegion.Center.Latitude + 0.00001, map.VisibleRegion.Center.Longitude), map.VisibleRegion.Radius));

                    }
                }
                else
                {
                   
                    ssloading.IsVisible = false;
                }
            }

        }
        double x= 0, y = 0;
        async void showRatings(Object s, EventArgs e)
        {
            //hidelive ratings 
            //ReactionsContainer.IsVisible = false;
           
            if (!Opened)
            {
                if (Device.RuntimePlatform.Equals(Device.Android))
                {
                    await info_view.TranslateTo(0, -1 * (info_view.Height - 75 * App.scale), 250, Easing.CubicIn);
                    y = info_view.TranslationY;

                }
                else
                {
                    await info_view.TranslateTo(0, -1 * (info_view.Height - 80 * App.scale), 250, Easing.CubicIn);
                    y = info_view.TranslationY;

                }
                searchView.IsVisible = false;
                menu_icon.IsVisible = false;
                //search_button.IsVisible = false;
                Opened = true;
                map.OnSelect = true;
                live_bg.IsVisible = true;
               
                    client.Perform("follow", SelectedPinInfo.ID);
                    FetchPlaceRatingsAPI api = new FetchPlaceRatingsAPI(SelectedPinInfo.ID);
                    api.setCallbacks(this);
                    api.getResponse();
                
                //client.Connected += (sender, ee) => 
                //{
                //    // we are connected to actioncable server
                //};

                cvLoading.IsVisible = true;
                animatecancel = new CancellationTokenSource();
            }
            else
            {
                await info_view.TranslateTo(0, 0, 250, Easing.CubicIn);
                searchView.IsVisible = true;
                menu_icon.IsVisible = true;
                //search_button.IsVisible = true;
                live_bg.IsVisible = false;
                Opened = false;
                map.OnSelect = false;
                cvLoading.IsVisible = false;
                endTimer = DateTime.Now;


            }

        }
        void showratingPan(object s, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    //info_view.TranslationX =  Math.Max(Math.Min(0, x + e.TotalX), -Math.Abs(info_view.Width - App.ScreenWidth));
                    info_view.TranslationY = Math.Max(Math.Min(0, y + e.TotalY), -Math.Abs(info_view.Height - App.ScreenHeight));
                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    //x = info_view.TranslationX;
                    y = info_view.TranslationY;
                    if (y < -50)
                    {
                        showRatings(null, null);
                    }
                    else
                    {
                        showRatings(null, null);
                    }
                    Debug.WriteLine(y);
                    break;
            }
        }
        async Task GotolocationAsync()
        {

            if (locationPermission == PermissionStatus.Granted)
            {
                var position = await forceGetLocation();

                if (map.CustomPin == null)
                    map.CustomPin = new ObservableCollection<PinModel>();
                else
                    map.CustomPin.Clear();

                //map.CustomPin = new List<PinModel> { };
                current_lat = position.Latitude;
                current_lng = position.Longitude;
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude), Distance.FromMeters(200)));

            }
            else
            {
                map.CustomPin = new ObservableCollection<PinModel>();
                current_lat = 0;
                current_lng = 0;
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(current_lat, current_lng), Distance.FromKilometers(1000)));

            }

            map.placeCallBack = this;
        }

        private async Task<Plugin.Geolocator.Abstractions.Position> forceGetLocation()
        {
            Plugin.Geolocator.Abstractions.Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                //Get the library stored location if any
                position = await locator.GetLastKnownLocationAsync();

                //Now if still no location try getting the actual gps location within 3 seconds time
                if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds(3), null, true);

                //Update the app's cached location - can access this anywhere.
                CacheSettings.userLocation = position;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            //If all of the above fails this is the last resort get the app's stored location if any. If none then it will return new Position(0,0)
            return position ?? CacheSettings.userLocation;
        }
       
          void LogoutTapped(object sender, EventArgs e)
        {   
             
            Navigation.PopModalAsync();
        }
        async void clear_database()
        {
            List<Emojis> newEmojis = new List<Emojis>();
            newEmojis = await App.Database.GetItemsAsync();

            foreach (var emo in newEmojis)
            {
                await App.Database.DeleteItemAsync(emo);
            }
        }

        
        public async void OnSuccess(JObject response, BaseAPI caller)
        {

            if (caller as FetchEmojisAPI != null)
            {
                if (!Cacher.getLastSync().Equals(response["last_updated"].ToString()))
                {
                    clear_database();
                    List<Emojis> newEmojis = new List<Emojis>();

                    Cacher.LastSync(response["last_updated"].ToString());
                    foreach (var e in response["ratings"])
                    {
                        var emoji = new Emojis
                        {
                            emoji_ID = int.Parse(e["id"].ToString()),
                            category = e["category"].ToString(),
                            name = e["name"].ToString(),
                            rating_category_id = e["rating_category_id"].ToString(),
                            image = e["image"].ToString(),
                            status = e["order"].Value<int>(),
                            position = e["position"].Value<int>()
                        };
                        newEmojis.Add(emoji);
                        await App.Database.SaveItemAsync(emoji);
                    }
                    var saved = 1;

                    //foreach (var emo in newEmojis)
                    //{

                    //    saved++;
                    //}
                    await Task.Delay(500);
                    SetUpEmojis();
                }
                else
                {
                    SetUpEmojis();
                }


            }
            else if (caller as SendRatingAPI != null)
            {
                emojirating_id = response["user_rating"]["user_rating_id"].Value<int>();
            }
            else if (caller as FetchPlaceRatingsAPI != null)
            {
                latest_ratings = response["restaurant"]["latest_ratings"];
                foreach (var rating in latest_ratings)
                {
                    ratings.Clear();
                    var ct = new CancellationTokenSource();
                    animateSpecificRatings(ct.Token);
                }
                if (animateTaskRunning)
                {
                    animatecancel.Cancel();
                }
                else
                {
                    animatecancel = new CancellationTokenSource();
                }
                cvLoading.IsVisible = false;
                animateSpecificRatings(animatecancel.Token);

            }
            else if (caller as FetchUserAPI != null)
            {
                Constants.current_user = JsonConvert.DeserializeObject<User>(response.ToString(), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                Constants.CurrentCategoryOrder = new List<int>();
                Cacher.SaveCurrentUser(Constants.current_user.data.id, Constants.accessToken, Constants.client, Constants.uid);
                foreach (var orderNum in response["user"]["user_setting"]["ratings_order"])
                {
                    Constants.CurrentCategoryOrder.Add(orderNum.Value<int>());
                }
                
                Constants.NotificationAllowed = response["user"]["user_setting"]["location_notification"].Value<bool>() ? true:false;
                FetchEmojis();

            }else if (caller as FetchPlacesAPI != null)
            {
                double nearest = 250;
                double distanceInMetre = 0;
                int itemcounter = 0;
                var locator = CrossGeolocator.Current;
                var position = locator.GetLastKnownLocationAsync();

                foreach (var resto in latest_pins["restaurants"])
                {
                    itemcounter++;
                    Plugin.Geolocator.Abstractions.Position pos = new Plugin.Geolocator.Abstractions.Position();
                    pos.Latitude = double.Parse(resto["lat"].ToString());
                    pos.Longitude = double.Parse(resto["lng"].ToString());
                    distanceInMetre = GetDistance(position.Result, pos) * 1000;
                    //Debug.WriteLine(resto["name"]);
                    //Debug.WriteLine("Distance {0}", distanceInMetre);
                    if (distanceInMetre < 15)
                    {
                        if (nearest > distanceInMetre)
                        {
                            nearest = distanceInMetre;
                            nearestPin = new PinModel()
                            {
                                Type = Xamarin.Forms.Maps.PinType.Place,
                                Position = new Xamarin.Forms.Maps.Position(Double.Parse(resto["lat"].ToString()), Double.Parse(resto["lng"].ToString())),
                                Label = resto["name"].ToString(),
                                Address = "323e",
                                Id = resto["place_id"].ToString(),
                                Url = "http://xamarin.com/about/",
                                pin = "pin",
                            };
                            Debug.WriteLine(nearestPin.Label);
                        }
                        //Debug.WriteLine(resto["name"]);
                    }
                    if(nearestPin.Id !=null)
                    {
                        if (itemcounter == 11 && nearestPin.Id.Length > 0 && Constants.NotificationAllowed)
                        {
                            DependencyService.Get<ILocalNotification>().sendLocalNotif(nearestPin.Label);
                        }
                    }
                }
            }
        }

        public void OnError(string errMsg, BaseAPI caller)
        {
            DisplayAlert("Scuttle", "Sending rating failed.", "OK");
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }

        public JObject latest_pins = new JObject();


        /// <summary>
        /// Calculates distance between points in kilometers.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="p1">P1.</param>
        /// <param name="p2">P2.</param>
        public double GetDistance(Plugin.Geolocator.Abstractions.Position p1, Plugin.Geolocator.Abstractions.Position p2)
        {
            // convert to radians
            double phi_1, phi_2, lambda_1, lambda_2;
            phi_1 = p1.Latitude * Math.PI / 180;
            phi_2 = p2.Latitude * Math.PI / 180;

            lambda_1 = p1.Longitude * Math.PI / 180;
            lambda_2 = p2.Longitude * Math.PI / 180;

            return 6371 * Math.Acos(Math.Sin(phi_1) * Math.Sin(phi_2) + Math.Cos(phi_1) * Math.Cos(phi_2) * Math.Cos(lambda_2 - lambda_1));
        }

        public void resetTimer()
        {
            lastFoodEmojiSent = DateTime.Now;
            lastVibeEmojiSent = DateTime.Now;
            lastServiceEmojiSent = DateTime.Now;
            lastCleanlinessEmojiSent = DateTime.Now;
        }

        public void showinfo(string id)
        {
            if (id != null)
            {
                var ctr = 0;
                Plugin.Geolocator.Abstractions.Position Sposition = new Plugin.Geolocator.Abstractions.Position();
                Plugin.Geolocator.Abstractions.Position Dposition = new Plugin.Geolocator.Abstractions.Position();
                var info_found = false;
                foreach (var pin in latest_pins["restaurants"])
                {
                     if (pin["place_id"].ToString().Equals(id))
                    {
                        info_name.Text = pin["name"].ToString();
                        info_address.Text = pin["address"].ToString();
                        string r;
                        if (pin["rating"].ToString().Equals("-1") || pin["rating"].ToString().Equals(""))
                        {
                            r = "0";
                        }
                        else
                        {
                            r = pin["rating"].ToString();
                        }
                        info_rating.Text = r;
                        info_view.IsVisible = true;

                        Dposition.Latitude = pin["lat"].Value<Double>();
                        Dposition.Longitude = pin["lng"].Value<Double>();
                        Sposition.Latitude = current_lat;
                        Sposition.Longitude = current_lng;
                        info_distance.Text = "Distance: " + (GetDistance(Dposition, Sposition) * 0.621371).ToString("0.00") + "mi";
                        SelectedPinInfo = new Locations();
                        SelectedPinInfo.lat = pin["lat"].Value<Double>();
                        SelectedPinInfo.lng = pin["lng"].Value<Double>();
                        SelectedPinInfo.ID = pin["place_id"].Value<String>();
                        //while (ctr < int.Parse(pin["rating"].ToString()))
                        //{
                        //    Image rate = new Image()
                        //    {
                        //        Source="star_yellow",
                        //        HeightRequest=20,
                        //        Aspect= Aspect.Fill
                        //    };
                        //    ratingContainer.Children.Add(rate);
                        //    ctr++;
                        //}
                        //if(ctr<5)
                        //{
                        //    Image rate = new Image()
                        //    {
                        //        Source = "star_gray",
                        //        HeightRequest = 20,`
                        //        Aspect = Aspect.Fill
                        //    };
                        //    ratingContainer.Children.Add(rate);
                        //    ctr++;
                        //}
                        info_found = true;
                    }
                    else if (map.OnSearch && !info_found)
                    {
                        info_name.Text = SelectedPinInfo.name;
                        info_address.Text = SelectedPinInfo.address;
                        info_rating.Text = SelectedPinInfo.rating;
                        info_view.IsVisible = true;
                    }

                }
                live_bg.IsVisible = true;
            }
            else
            {
                Debug.WriteLine("ID is null");
            }
        }
        public async void didGetPlaces(JObject response)
        {
            ssloading.IsVisible = false;
            if (!map.OnSearch)
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    map.Pins.Clear();
                    map.CustomPin.Clear();
                }
                cancel = new CancellationTokenSource();
                latest_pins = response;
                await Task.Factory.StartNew(async () =>
                {
                    foreach (var Places in response["restaurants"])
                    {
                        var ctr = 0;

                        var emojilist = new List<string>();
                        foreach (var ratings in Places["latest_ratings"])
                        {
                            if (ctr < 4)
                            {
                                var rating = ratings["rating"];
                                //Debug.WriteLine(Constants.SERVER_URL + rating["images"]["medium"].Value<String>());
                                emojilist.Add(rating["image"].Value<String>());
                            }
                            ctr++;
                        }
                        PinModel pin = new PinModel();
                        if (Places["latest_ratings"] != null)
                        {
                            pin = new PinModel
                            {
                                Type = Xamarin.Forms.Maps.PinType.Place,
                                Position = new Xamarin.Forms.Maps.Position(Double.Parse(Places["lat"].ToString()), Double.Parse(Places["lng"].ToString())),
                                Label = Places["name"].ToString(),
                                Address = "323e",
                                Id = Places["place_id"].ToString(),
                                Url = "http://xamarin.com/about/",
                                pin = "pin",
                                Emojis = emojilist,

                            };
                        }
                        else
                        {

                            pin = new PinModel
                            {
                                Type = Xamarin.Forms.Maps.PinType.Place,
                                Position = new Xamarin.Forms.Maps.Position(Double.Parse(Places["lat"].ToString()), Double.Parse(Places["lng"].ToString())),
                                Label = Places["name"].ToString(),
                                Address = "323e",
                                Id = Places["place_id"].ToString(),
                                Url = "http://xamarin.com/about/",
                                pin = "pin",
                                Emojis = new List<string>()
                            {
                                "cool",
                                "cool",
                                "cool",
                                "cool",
                            }
                            };
                        }
                        await Task.Delay(300);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (!map.Pins.Contains(pin) && !map.OnSelect)
                            {
                                if (map.Pins.Count > 9)
                                {
                                    map.CustomPin.RemoveAt(0);
                                    map.Pins.RemoveAt(0);
                                }
                                map.Pins.Add(pin);
                                map.CustomPin.Add(pin);
                                //Debug.WriteLine(map.CustomPin.Count + " " + map.Pins.Count);
                            }
                        });
                    }
                }, cancel.Token);
            }
            if (timerInit)
            {
                Constants.canCheck = true;
                timer(60000);
            }
        }

        public async void MyLocationClicked(object sender, EventArgs e)
        {   

            var fade = (Image)sender;
            await fade.FadeTo(0, 200);
            await Task.Delay(100);
            await fade.FadeTo(1, 200);
            if (locationPermission == PermissionStatus.Granted)
            {
                if (CrossGeolocator.Current.IsGeolocationEnabled)
                {
                    var locator = CrossGeolocator.Current;
                    var position = locator.GetLastKnownLocationAsync();

                    if (position.Result != null)
                    {
                        current_lat = position.Result.Latitude;
                        current_lng = position.Result.Longitude;
                    }
                    else
                    {
                        askpermissions();
                        current_lat = 0.0;
                        current_lng = 0.0;
                    }

                    var pos = new Xamarin.Forms.Maps.Position(current_lat, current_lng);

                    if (Constants.is_android)
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(200)));
                    }
                    else
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, map.VisibleRegion.Radius));
                    }
                }else
                {
                    askpermissions();
                }
            }
            else
            {
                askpermissions();
            }
        }

        public async void DidTapEmoji(Label emoji, double x, double Y, int emoji_ID)
        {
            sendingEmoji = true;

                var selectedEmoji = new OnCooldownEmoji();
                if (OnCoolDown.Count > 0)
                {
                    foreach (var CD in OnCoolDown)
                    {

                        if (CD.id == emoji_ID && CD.pin.ID == SelectedPinInfo.ID)
                        {
                            selectedEmoji = CD;
                            if (selectedEmoji.dateClicked.AddMinutes(2) < DateTime.Now)
                            {
                                selectedEmoji.onCooldown = false;
                                CD.onCooldown = false;
                                CD.dateClicked = DateTime.Now;

                            }
                            else
                            {
                                selectedEmoji.onCooldown = true;
                                CD.onCooldown = true;
                            }
                        }
                    }
                }
                else
                {
                    selectedEmoji = new OnCooldownEmoji() { id = emoji_ID, onCooldown = false, dateClicked = DateTime.Now, added = false };
                }
                if (!selectedEmoji.onCooldown)
                {
                    if (!selectedEmoji.added)
                    {
                        var oncd = new OnCooldownEmoji()
                        {
                            id = emoji_ID,
                            onCooldown = true,
                            dateClicked = DateTime.Now,
                            added = true,
                            pin = SelectedPinInfo
                        };
                        OnCoolDown.Add(oncd);

                    }
                    switch (CarouselView.Position + 1)
                    {
                        case (1): lastFoodEmojiSent = DateTime.Now; break;
                        case (2): lastServiceEmojiSent = DateTime.Now; break;
                        case (3): lastCleanlinessEmojiSent = DateTime.Now; break;
                        case (4): lastVibeEmojiSent = DateTime.Now; break;
                    }

                    SendRatingAPI api = new SendRatingAPI(SelectedPinInfo.ID, emoji_ID);
                    api.setCallbacks(this);
                    api.getResponse();

                    var AnimateEmoji = new Label()
                    {
                        Text = emoji.Text,
                        IsVisible = false,
                        FontSize = 25 * App.scale,
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        TextColor = Color.Black
                    };

                    if (AnimateEmoji.Text.Equals("5m") || AnimateEmoji.Text.Equals("10m") || AnimateEmoji.Text.Equals("15m+"))
                    {
                        AnimateEmoji.FontSize = 15 * App.scale;
                        AnimateEmoji.WidthRequest = 45 * App.scale;
                    }
                    if (Constants.is_android)
                    {
                        AnimateEmoji.WidthRequest = 40 * App.scale;

                    }
                    //AnimateEmoji =imgPool.Get();
                    //AnimateEmoji.Source = emoji.Source;
                    //AnimateEmoji.IsVisible = false;
                    MainConent.Children.Add(AnimateEmoji, new Rectangle(0, 0, .1, .1), AbsoluteLayoutFlags.All);
                    AnimateEmoji.TranslateTo(0, 0, 200, Easing.Linear);
                    //Debug.WriteLine("{0} was tapped:X = {1}, Y = {2}", emoji.Source.ToString(),x,Y);
                    await AnimateEmoji.TranslateTo(x - 5, Y - 265, 200, Easing.Linear);
                    AnimateEmoji.FadeTo(.2, 4000, Easing.SinIn);
                    await Task.Delay(200);
                    AnimateEmoji.IsVisible = true;
                    //AnimateEmoji.FadeTo(.2, 4000, Easing.SinIn);
                    Easing e = Easing.CubicOut;
                    double startX = x - 5;
                    double endX = -50;
                    double deltaX = endX - startX;
                    Random r = new Random();
                    double height = 0;
                    double startY = 0;
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        height = 0.03 * App.ScreenHeight;
                        startY = 0.03 * App.ScreenHeight;
                    }
                    else
                    {
                        height = 0.05 * App.ScreenHeight;
                        startY = 0.05 * App.ScreenHeight;
                    }
                    int sign = r.NextDouble() < 0.5 ? -1 : 1;
                    double phase = r.NextDouble() * height;
                    await AnimateEmoji.TranslateTo(x, (uint)(startY + sign * height * Math.Sin(2 * Math.PI * (0 / 2000) + phase)), 1000, Easing.Linear);
                    for (double t = 0; t < 2000; t += 10)
                    {
                        var eX = e.Ease(t / 2000);
                        AnimateEmoji.TranslationX = startX + deltaX * eX;
                        AnimateEmoji.TranslationY = startY + sign * height * Math.Sin(2 * Math.PI * (t / 2000) + phase);
                        //Debug.WriteLine("X:{0}, Y:{1}, StartX{2} and Y{3} ", AnimateEmoji.TranslationX,AnimateEmoji.TranslationY,startX,startY);
                        await Task.Delay(20);
                    }
                    sendingEmoji = false;

                }
           
            
        }
        public void DidHideInfoview()
        {
            info_view.IsVisible = false;
        }
       
        private async Task timer(int delay)
        {
            int Delay = delay;
            double nearest = 250;
            double distanceInMetre = 0;

            timerInit = false;
            locationCancelation = new CancellationTokenSource();
            while (!locationCancelation.IsCancellationRequested)
            {
                int itemcounter = 0;
                var locator = CrossGeolocator.Current;
                var position = locator.GetLastKnownLocationAsync();
                Debug.WriteLine("timer running");
                foreach (var resto in latest_pins["restaurants"])
                {
                    itemcounter++;
                    Plugin.Geolocator.Abstractions.Position pos = new Plugin.Geolocator.Abstractions.Position();
                    pos.Latitude = double.Parse(resto["lat"].ToString());
                    pos.Longitude = double.Parse(resto["lng"].ToString());
                    //Debug.WriteLine(resto["name"]);
                    distanceInMetre = GetDistance(position.Result, pos) * 1000;
                    //Debug.WriteLine("Distance {0}", distanceInMetre);

                    if (distanceInMetre < 15)
                    {
                        if (nearest > distanceInMetre)
                        {
                            nearestPin = new PinModel();
                            nearest = distanceInMetre;
                            nearestPin.Type = Xamarin.Forms.Maps.PinType.Place;
                            nearestPin.Position = new Xamarin.Forms.Maps.Position(Double.Parse(resto["lat"].ToString()), Double.Parse(resto["lng"].ToString()));
                            nearestPin.Label = resto["name"].ToString();
                            nearestPin.Address = "323e";
                            nearestPin.Id = resto["place_id"].ToString();
                            nearestPin.Url = "http://xamarin.com/about/";
                            nearestPin.pin = "pin";
                            Cacher.saveNearestID(nearestPin.Id);
                            Debug.WriteLine(nearestPin.Label);
                            Debug.WriteLine(nearestPin.Id);

                        }
                        //Debug.WriteLine(resto["name"]);
                    }
                    if (itemcounter == 11 && nearestPin.Id.Length > 0 && notyetsentnotif && Constants.NotificationAllowed)
                    {

                        DependencyService.Get<ILocalNotification>().sendLocalNotif(nearestPin.Label);
                        Delay = 60000;
                        notyetsentnotif = false;
                    }
                }
                await Task.Delay(Delay);
            }
        }
        List<Emojis> ratings = new List<Emojis>();
        bool animationRunning;
        async Task animateSpecificRatings(CancellationToken ct)
        {   
            
            if (!ct.IsCancellationRequested)
            {//await Task.Factory.StartNew(async () =>
             //{
                try
                {
                    animateTaskRunning = true;
                    ReactionsContainer.Children.Clear();

                    //emoji.Reset();
                    foreach (var latest in latest_ratings)
                    {
                        ratings.Add(new Emojis { category = latest["rating"]["rating_category_id"].Value<string>(), emoji_ID = latest["rating"]["id"].Value<int>(), ID = 0, image = latest["rating"]["image"].ToString() });
                    }
                    Random r = new Random();
                    endTimer = DateTime.Now.AddHours(1);
                    int ratingcounter = 0;


                    if (ratings.Any())
                    {
                        // the "param" property exists
                        while (DateTime.Now < endTimer)
                        {
                            ratingcounter = 0;

                            foreach (var rating in ratings)
                            {
                                if (Constants.CurrentCategoryOrder[CarouselView.Position] == int.Parse(rating.category))
                                { ratingcounter++; }
                            }
                            if (ratingcounter > 0)
                            {
                                if (!animateTaskRunning)
                                {
                                    break;
                                }
                                foreach (var rating in ratings)
                                {
                                    if (!animateTaskRunning)
                                    {
                                        break;
                                    }
                                    if (map.OnSelect)
                                    {
                                        if (Constants.CurrentCategoryOrder[CarouselView.Position] == int.Parse(rating.category))
                                        {

                                            if (Constants.is_android)
                                            {

                                                ReactionsContainer.TranslationX = 330 * App.scale;
                                                RatingObject emoji = new RatingObject()
                                                {
                                                    Image = rating.image,
                                                    Size = 20 * App.scale,

                                                };
                                                if (rating.image.Equals("5m") || rating.image.Equals("10m") || rating.image.ToString().Equals("15m+"))
                                                {
                                                    emoji.Size = 15 * App.scale;
                                                }
                                                //emoji.WidthRequest = 20 * App.scale;
                                                emoji.Reset();
                                                ReactionsContainer.Children.Add(emoji);
                                                emoji.AnimateLiveStream(6000);

                                                await Task.Delay(750 + (int)(300 * r.NextDouble()));
                                            }
                                            else
                                            {
                                                ReactionsContainer.TranslationX = 330 * App.scale;
                                                RatingObject emoji = new RatingObject()
                                                {
                                                    Image = rating.image,
                                                    Size = 25 * App.scale,
                                                };
                                                emoji.WidthRequest = 30 * App.scale;

                                                if (rating.image.Equals("5m") || rating.image.Equals("10m") || rating.image.Equals("15m+"))
                                                {
                                                    emoji.Size = 15 * App.scale;
                                                    emoji.WidthRequest = 40 * App.scale;
                                                }
                                                emoji.Reset();
                                                ReactionsContainer.Children.Add(emoji);
                                                emoji.AnimateLiveStream(6000);
                                                await Task.Delay(750 + (int)(300 * r.NextDouble()));
                                            }

                                        }

                                    }
                                    if (ReactionsContainer.Children.Count > 10)
                                    {
                                        ReactionsContainer.Children.RemoveAt(0);
                                        Debug.WriteLine("ReactionsContainer Children: {0}", ReactionsContainer.Children.Count);
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        animateTaskRunning = false;
                    }
                    else
                    {
                        cvLoading.IsVisible = false;
                    }
                    //}, animatecancel.Token);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void sendLocalNotif(string name)
        {

        }
        public async void ShareNow()
        {


            //Android notification reciever
            if (Constants.fromnotif)
            {

                var x = await DisplayActionSheet("Hey are you at Appstone?",null,null, "Yes. Share my experience!", "Yes. Remind me later to share", "Not this time");
                switch(x)
                {
                    case ("Yes. Share my experience!"): recieveFromLocal("sendRating"); break;
                    case ("Yes. Remind me later to share"): recieveFromLocal("10"); break;
                }
                Constants.fromnotif = false;

            }

        }

        public void recieveFromLocal(string response)
        {

            bool open = true; 
                 if (response.Equals("sendRating"))
                {   
                    //while (open)
                    //{
                        //if (latest_pins.Count > 0)
                        //{
                            showinfo(Cacher.getNearestID());
                            showRatings(null, null);
                            open = false;
                        //}
                //}
                }
                else
                {
                  Constants.canSendNotification = true;
                notifcationcountDown = DateTime.Now.AddSeconds(double.Parse(response));
                }
            
        }

        public void Checker()
        {
            if (locationPermission == PermissionStatus.Granted)
            {
                var locator = CrossGeolocator.Current;
                var position = locator.GetLastKnownLocationAsync();
                FetchPlacesAPI api = new FetchPlacesAPI(position.Result.Latitude.ToString(), position.Result.Longitude.ToString(), 0);
                api.setCallbacks(this);
                api.getResponse();
                Debug.WriteLine("checker running");
            }else
            {
                Debug.WriteLine("cant access location");
            }
            //DependencyService.Get<ILocalNotification>().sendLocalNotif("SHAMLA");
        }
    }
}

public class ObjectPool<T> where T : new()
{
    private readonly ConcurrentBag<T> items = new ConcurrentBag<T>();
    private int counter = 0;
    private int MAX = 10;

    public void Release(T item)
    {
        if (counter < MAX)
        {
            items.Add(item);
            counter++;
            Debug.WriteLine("Counter: {0}", counter);
        }
    }
    public T Get()
    {
        T item;
        if (items.TryTake(out item))
        {
            counter--;
            Debug.WriteLine("Counter: {0}", counter);
            return item;
        }
        else
        {
            T obj = new T();
            items.Add(obj);
            counter++;
            Debug.WriteLine("Counter: {0}", counter);
            return obj;
        }

    }
}

