using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views.Animations;
using Android.Widget;
using X;
using X.Droid.CustomRenderer;
using X.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using static Android.Gms.Maps.GoogleMap;
using static Android.Graphics.BitmapFactory;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace X.Droid.CustomRenderer
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        ObservableCollection<PinModel> customPins;
        ObservableCollection<CancellationTokenSource> tokens = new ObservableCollection<CancellationTokenSource>();
        private DateTime mapDraggedTime;
        private bool CanFetch = false;
        Dictionary<PinModel,Marker> marketList = new Dictionary<PinModel,Marker>();
        CustomMap formsMap;
        public CustomMapRenderer(Context context) : base()
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
                NativeMap.MarkerClick -= OnMarkerClicked;
                NativeMap.CameraChange -= Map_CameraChange;
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPin;

                var observAble = formsMap.Pins as INotifyCollectionChanged;
                if (observAble != null)
                {
                    observAble.CollectionChanged += OnCustomPinsCollectionChanged;
                }
                Control.GetMapAsync(this);
            }
        }

		protected override MarkerOptions CreateMarker(Pin pin)
		{
            PinModel p = pin as PinModel;
            if (p != null)
                return CreateMarkerOptions(p);

            return base.CreateMarker(pin);
        }

		private void CancelPinAnimation() {
            System.Diagnostics.Debug.WriteLine("Cancelling all pin animations");
            foreach (var t in tokens)
            {
                t.Cancel();
            }
            tokens.Clear();
        }

        private void OnCustomPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {   
            System.Diagnostics.Debug.WriteLine("On custom pins collection changed");
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //CancelPinAnimation();
                foreach (PinModel pin in e.NewItems)
                {
                    var options = CreateMarkerOptions(pin);
                    Marker marker = NativeMap.AddMarker(options);
                    var cpin = pin as PinModel;
                    if (!marketList.ContainsKey(cpin))
                    {
                        marketList.Add(cpin, marker);
                    }
                    if (cpin.Emojis.Count > 0)
                    {
                        CancellationTokenSource token = new CancellationTokenSource();
                        tokens.Add(token);
                        AnimatePinStreaming(cpin, marker, token);
                        Console.WriteLine(tokens.Count);
                    }
                }
            }
            else if( e.Action == NotifyCollectionChangedAction.Remove)
            {
                //CancelPinAnimation();
            }
        }





        MarkerOptions CreateMarkerOptions(PinModel pin) {

            //var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            //Android.Views.View view;

            //view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);

            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Id);
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(createBitmapFromLayoutWithText(pin, 0, 0)));
            return marker;
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.UiSettings.ZoomControlsEnabled = false;
            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.MarkerClick += OnMarkerClicked;
            NativeMap.SetInfoWindowAdapter(this);
            NativeMap.CameraChange += Map_CameraChange;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (DateTime.Now.Subtract(TimeSpan.FromSeconds(1)) >= mapDraggedTime && !CanFetch && !customMap.OnSelect && Constants.OnInit)
                {
                    NativeMap.Clear();
                    customMap.getplace(lat, lng,0);
                    CanFetch = true;
                    marketList.Clear();

                    CancelPinAnimation();
                }
                return true;
            });
            //Device.StartTimer(TimeSpan.FromSeconds(20), () =>
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        foreach (var item in marketList)
            //        {
            //            //System.Diagnostics.Debug.WriteLine("ctr = {0}", ctr++);
            //            //Marker marker = (Marker)item.Value;
            //            //marker.Remove();
            //            //marker.Visible = false;
            //            //item.Value.Dispose();
            //            //var marker = new MarkerOptions();
            //            //marker.SetPosition(new LatLng(item.Value.Position.Latitude, item.Value.Position.Longitude));
            //            //marker.SetTitle(item.Key.Label);
            //            //marker.SetSnippet(item.Key.Address);
            //            //marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.heart_eyes));
            //            //NativeMap.AddMarker(marker);
            //        }
            //    });
            //    return false;
            //});
        }
        int ctr = 0;
        CustomMap customMap
        {
            get { return Element as CustomMap; }
        }
        Double lat, lng;
        private async void Map_CameraChange(object sender, CameraChangeEventArgs e)
        {
            if (!customMap.OnSearch && !customMap.OnSelect)
            {
                var s = sender as GoogleMap;
                CanFetch = false;
                mapDraggedTime = DateTime.Now;

                var zoom = e.Position.Zoom;
                if (((CustomMap)this.Element) != null && NativeMap != null)
                {

                    var projection = NativeMap.Projection.VisibleRegion;
                    var far_right_lat = Math.Round(projection.FarLeft.Latitude, 5).ToString();

                    lat = projection.LatLngBounds.Center.Latitude;

                    lng = projection.LatLngBounds.Center.Longitude;

                    var centerLatLong = projection.LatLngBounds.Center;
                }
            }
            if (!customMap.OnSelect)
            { 
                customMap.hideInfoView();
            }
        }


        public Bitmap DrawPin(Pin p, float step, int currentEmojiIndex, float x) 
        {
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true,
                InScaled = true,
                InMutable = true, 
            };

            Bitmap b = BitmapFactory.DecodeResource(Resources, Resource.Id.pin,options);

            options.InJustDecodeBounds = false;
            try
            {
                if (customMap.OnSelect)
                {
                    var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
                    Android.Widget.RelativeLayout view = new Android.Widget.RelativeLayout(Context);
                    inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, view, true);

                    view.Measure(MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified),
                              MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified));
                    view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
                    Bitmap bitmap = Bitmap.CreateBitmap(view.MeasuredWidth,
                                                        view.MeasuredHeight,
                                                        Bitmap.Config.Alpha8);
                    Canvas c = new Canvas(bitmap);
                    view.Draw(c);
                    c.Dispose();
                    return bitmap;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Step: {0}", step);
                    b = Bitmap.CreateBitmap(75, 280, Bitmap.Config.Argb8888);
                    Canvas canvas = new Canvas(b);
                    Rect bounds = new Rect();


                    Paint paint = new Paint(PaintFlags.AntiAlias);
                    paint.TextSize = (float)(50 * App.scale);
                    float startY = (float)(170 * App.scale);

                    string emoji = "";
                    var pinModel = p as PinModel;
                    if (pinModel != null && pinModel.Emojis.Count > 0)
                    {
                        emoji = pinModel.Emojis[currentEmojiIndex];
                    }
                    var rand = new Random();

                    canvas.DrawText(emoji, x, startY * (1 - step), paint);

                    canvas.Dispose();
                    paint.Dispose(); 
                    //int numEmojis = (int)Math.Ceiling( step / 0.25f );
                    //System.Diagnostics.Debug.WriteLine("Number of emojis to show: {0}", numEmojis);
                    //for (int n = 0; n < numEmojis; n++) {
                    //    string emoji = ":)";
                    //    var pinModel = p as PinModel;
                    //    if (pinModel != null && pinModel.Emojis.Count > 0)
                    //    {
                    //        emoji = pinModel.Emojis[numEmojis - n - 1];
                    //    }
                    //    float y = (300 - 35) * (1 - n * (step - 0.25f));
                    //        System.Diagnostics.Debug.WriteLine("y = {0}", y);
                    //    canvas.DrawText(emoji, 17, y, paint);
                    //}
                }

            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                if (b != null) {
                    b.Recycle();
                    b.Dispose();
                    b = null;
                }
            }

            return b;
        
        }


        public Bitmap createBitmapFromLayoutWithText(Pin pin, float steps, int currentEmojiIndex)
        {   
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            Android.Widget.RelativeLayout view = new Android.Widget.RelativeLayout(Context);
            inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, view, true);

            if (customMap.OnSelect)
            {
                view.Measure(MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified),
                          MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified));
                view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
                Bitmap bitmap = Bitmap.CreateBitmap(view.MeasuredWidth,
                                                    view.MeasuredHeight,
                                                    Bitmap.Config.Argb8888);
                Canvas c = new Canvas(bitmap);
                view.Draw(c);
                c.Dispose();
                return bitmap;


            }
            else
            {
                //var emoji1 = view.FindViewById<TextView>(Resource.Id.Emoji);
                //var cpin = pin as PinModel;
                    //switch (currentEmojiIndex)
                    //{
                    //    case (0):
                    //    if (cpin.Emojis.Count > 0)
                    //    {
                    //        emoji1.Text = cpin.Emojis[0];
                    //        emoji1.TextSize = 25;
                    //        emoji1.TranslationY = -steps;
                    //    }
                    //        break;

                    //    case (1):
                    //    if (cpin.Emojis.Count > 0)
                    //    {
                    //        emoji1.Text = cpin.Emojis[1];
                    //        emoji1.TextSize = 25;
                    //        emoji1.TranslationY = -steps;
                    //    }
                    //        break;

                    //    case (2):
                    //    if (cpin.Emojis.Count > 1)
                    //    {
                    //        emoji1.Text = cpin.Emojis[2];
                    //        emoji1.TextSize = 25;
                    //        emoji1.TranslationY = -steps;
                    //    }
                    //        break;
                    //    case (3):
                    //    if (cpin.Emojis.Count > 2)
                    //    {
                    //        emoji1.Text = cpin.Emojis[3];
                    //        emoji1.TextSize = 25;
                    //        emoji1.TranslationY = -steps;
                    //    }
                    //        break;
                    //}
                view.Measure(MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified),
                             MeasureSpec.MakeMeasureSpec(0, Android.Views.MeasureSpecMode.Unspecified));
                view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
                Bitmap bitmap = Bitmap.CreateBitmap(view.MeasuredWidth,
                                                    view.MeasuredHeight,
                                                    Bitmap.Config.Argb8888);
                Canvas c = new Canvas(bitmap);
                view.Draw(c);
                c.Dispose();
                view.Dispose();
                return bitmap;

            }
        }

        async Task AnimatePinStreaming(PinModel p, Marker marker, CancellationTokenSource cancelToken)
        {
            if (p.Id == null)
                return;
             
            Console.WriteLine("AnimatePinStreamRunning...");
            int currentEmojiIndex = 0;
            int animationCount = 0;
            while (!cancelToken.IsCancellationRequested)
            {
              
                float duration = 1500; // milliseconds
                float fps = 60;
                float step = duration / fps;
                if(currentEmojiIndex>3)
                {
                    currentEmojiIndex = 0;
                   
                }

                var x = 0f;
                var rand = new Random();

                x=  rand.Next(10, 15);

                for (float i = 0; i < 1; i += 1.0f/step)
                {
                    //var markerImage = createBitmapFromLayoutWithText(p, i, currentEmojiIndex);
                    var markerImage = DrawPin(p, i, currentEmojiIndex,x);

                    if (markerImage == null)
                    {
                        System.Diagnostics.Debug.WriteLine("marker image is null");
                        break;
                    }
                    
                    //p.Label = i.ToString();
                    marker.SetIcon(BitmapDescriptorFactory.FromBitmap(markerImage));
                    await Task.Delay((int)step);
                    markerImage.Dispose();
                    markerImage = null;
                }
                animationCount++;
                currentEmojiIndex++;
            }
            System.Diagnostics.Debug.WriteLine("AnimatePinStreaming canceled");
        }
         
        public string getEmojiByUnicode(int unicode)
        {
            return new System.String(Java.Lang.Character.ToChars(unicode));
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return imageBitmap;
        }
        public static Bitmap getBitmapFromView(Android.Views.View view)
        {
            Bitmap returnedBitmap = Bitmap.CreateBitmap(10, 10, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(returnedBitmap);
            Drawable bgDrawable = view.Background;
            if (bgDrawable != null)
                bgDrawable.Draw(canvas);
            else
                canvas.DrawColor(Android.Graphics.Color.Black);
            view.Draw(canvas);
            return returnedBitmap;
        }


        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }   


        void OnMarkerClicked(object sender, GoogleMap.MarkerClickEventArgs e)
        {

            CancelPinAnimation();

            customMap.OnSelect = true;
            var pintoadd = new PinModel();
            var customView = e.Marker;
            System.Diagnostics.Debug.WriteLine("Position of marker: {0}, {1}", customView.Position.Latitude, customView.Position.Longitude);

            foreach (X.Models.PinModel pin in customMap.CustomPin)
            {
                System.Diagnostics.Debug.WriteLine("Custom pin position: {0}, {1}", pin.Position.Latitude, pin.Position.Longitude);
                System.Diagnostics.Debug.WriteLine("\tdLat = {0}", Math.Abs(pin.Position.Latitude - customView.Position.Latitude));
                System.Diagnostics.Debug.WriteLine("\tdLon = {0}", Math.Abs(pin.Position.Longitude - customView.Position.Longitude));
                if(customView.Snippet.Equals(pin.Id)){
                    pintoadd = pin;
                    break;
                }
            }
            VisibleRegion visibleRegion = NativeMap.Projection.VisibleRegion;
             
            NativeMap.Clear();
            marketList.Clear();

            //var dMeters = 0.004 * NativeMap.Projection.VisibleRegion.;
            //var dLat = dMeters / 111.11;
            var bounds = NativeMap.Projection.VisibleRegion.LatLngBounds;
            var dLat = 0.01 * (bounds.Northeast.Latitude - bounds.Southwest.Latitude);

            LatLng latlng = new LatLng(pintoadd.Position.Latitude-dLat, pintoadd.Position.Longitude);
            NativeMap.MoveCamera(CameraUpdateFactory.NewLatLng(latlng));
            //pintoadd.Emojis.Clear();
            NativeMap.AddMarker(CreateMarker(pintoadd));
            customMap.showInfoBox(pintoadd.Id);

        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }


        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        PinModel GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in formsMap.CustomPin)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }
}

