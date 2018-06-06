using System;
using System.Collections.Generic;
using CoreGraphics;
using X;
using X.iOS;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using X.Models;
using CoreLocation;
using Foundation;
using System.Threading.Tasks;
using X.API;
using X.Pages;
using System.Collections.ObjectModel;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace X.iOS
{
    public class CustomMapRenderer : MapRenderer
    {
        void HandleAction()
        {

        }

        UIView customPinView;
        ObservableCollection<PinModel> customPins = new ObservableCollection<PinModel>();
        private DateTime mapDraggedTime;
        private bool CanFetch = false;
        bool OnSelect;
        bool animationRunning = false;
        CustomMap formsMap;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                nativeMap.RegionChanged -= OnMapRegionChanged;
            }

            if (e.NewElement != null)
            {
                 formsMap = (CustomMap)e.NewElement;
                //var nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPin!=null?formsMap.CustomPin:new ObservableCollection<PinModel>();
                OnSelect = formsMap.OnSelect;
                //nativeMap.AddAnnotations(new MKPointAnnotation()
                //{s
                //    Title = "MyAnnotation",
                //    Coordinate = new CLLocationCoordinate2D(customPins)
                //});

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.RegionChanged += OnMapRegionChanged;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
                nativeMap.ZoomEnabled = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                    if (DateTime.Now.Subtract(TimeSpan.FromSeconds(2)) >= mapDraggedTime && !CanFetch && !customMap.OnSelect && Constants.OnInit  ) {
                        System.Diagnostics.Debug.WriteLine("[iOS] Map dragged");
                        customMap.getplace(s.Region.Center.Latitude, s.Region.Center.Longitude,0);
                        CanFetch = true;
                    }
                    return true;
                });
            }
        }

        CustomMap customMap
        {
            get { return Element as CustomMap; }
        }

        MKMapView nativeMap
        {
            get { return Control as MKMapView; }
        }

        MKMapView s= new MKMapView();

         void OnMapRegionChanged(object sender, MKMapViewChangeEventArgs e)
        {   

            if (!customMap.OnSelect && !customMap.OnSearch)
            {
                s = sender as MKMapView;
                mapDraggedTime = DateTime.Now;
                CanFetch = false;   
                //var startTime = DateTime.Now.Add(TimeSpan.FromSeconds(2));
                //Device.StartTimer(TimeSpan.FromSeconds(5), () => {
                //    return true;
                //});
            }
            if (!customMap.OnSelect)
            {
                customMap.hideInfoView();
            }

        }
        MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
                return null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            
            if (customPin == null)
            {
                Console.WriteLine("Custom pin null");
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Id,customPin.Emojis,customPin.pin);
                annotationView.Image = UIImage.FromFile(customPin.pin);
                //annotationView.CalloutOffset = new CGPoint(0, 0);
                //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("settings.png"));
                //annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).Id = customPin.Id;
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
                ((CustomMKAnnotationView)annotationView).emojis = customPin.Emojis;
                ((CustomMKAnnotationView)annotationView).pin = customPin.pin;
            }
            annotationView.CanShowCallout = false;
            customPinView = new UIView();
            Random rand = new Random();
            var images = new List<UILabel>();
           
            if (customPin.Emojis != null)
            {   
               
                int x = 0, y = 0, w = 0, h = 0, ctr = 0;

                customPinView.Frame = new CGRect(0, 0, 40, 40);
                foreach (var emoji in customPin.Emojis)
                {
                    ctr += 1;
                    switch (ctr)
                    {
                        //case (1): x = 0; y = 0; w = rand.Next(18,21); h = rand.Next(18,21); break;
                        //case (2): x = rand.Next(25,30); y = rand.Next(25,30); w = rand.Next(23,25); h = rand.Next(23,25); break;
                        //case (3): x = 0; y = rand.Next(50,60); w = 30; h = 30; break;
                        //case (4): x = rand.Next(10,15); y = rand.Next(80,90); w = 35; h = 35; break;
                        case (1): x = 0; y = 100; w = 30; h = 30; break;
                        case (2): x = 30; y = 100; w = 30; h = 30; break;
                        case (3): x = 0; y = 100; w = 30; h = 30; break;
                        case (4): x = 30; y = 100; w = 30; h = 30; break;
                    }
                    var image = new UILabel(new CGRect(x, y, w, h));
                    image.Font = UIFont.SystemFontOfSize(h);
                    image.Text = emoji;
                    image.AdjustsFontSizeToFitWidth = true;
                    //image.Image = UIImage.FromFile(emoji);
                    image.Alpha = 0;
                    images.Add(image);
                   
                    customPinView.AddSubview(image);
                    //UIView.Animate(2.5,
                    //               delay: ctr * 3,
                    //               options: UIViewAnimationOptions.Repeat | UIViewAnimationOptions.CurveEaseOut,
                    //               animation: () => {
                    //                   image.Alpha = 1;
                    //    image.Center = new CGPoint(image.Center.X, image.Center.Y - (1 + rand.NextDouble()%0.5) * 90);
                    //                }, 
                    //               completion: () => {

                    //                   //image.Alpha = 0;
                    //});
                }
                //customPinView.Frame = new CGRect(0, -100, 40, 40);
                //var image2 = new UIImageView(new CGRect(30, 30, 25, 25));
                //image2.Image = FromUrl(customPin.Emojis[1]);
                //var image3 = new UIImageView(new CGRect(0, 60, 30, 30));
                //image3.Image = FromUrl(customPin.Emojis[2]);
                //var image4 = new UIImageView(new CGRect(15, 90, 35, 35));
                //image4.Image = FromUrl(customPin.Emojis[3]);
                //customPinView.AddSubview(image2);
                //customPinView.AddSubview(image3);
                //customPinView.AddSubview(image4);
                if (!animationRunning)
                {
                    Console.WriteLine(customPin.Label);
                    AnimatePinStreaming(images);
                }
                customPinView.Center = new CGPoint(0, -(annotationView.Frame.Height + 75));
                annotationView.AddSubview(customPinView);
                if (customMap.OnSelect)
                {
                    foreach (var sub in annotationView.Subviews)
                    {
                        sub.RemoveFromSuperview();
                    }
                    images.Clear();
                }
            }
            else
            {
                Console.WriteLine("No emojis");
            }
             
            return annotationView;
        }

        void AnimatePinStreaming(List<UILabel> images)
        {
            animationRunning = true;
            if (images.Count == 0)
            {
                animationRunning = false;

                return;
            }
            float duration = .8f;
            UIViewAnimationOptions opts = UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.Repeat;
            var image1 = images[0];
            var rand = new Random();

            //var anim1 = new CoreAnimation.CAAnimation();

            UIView.AnimateKeyframes(duration * images.Count,
                                    delay: 0,
                                    options: UIViewKeyframeAnimationOptions.Repeat,
                                    animations: () =>
                                    {

                                        UIView.AddKeyframeWithRelativeStartTime(0, 0.15, () =>
                                        {
                                            image1.Alpha = 1;
                                        });
                                        UIView.AddKeyframeWithRelativeStartTime(0, 0.25, () =>
                                        {
                                            image1.Center = new CGPoint(image1.Center.X, image1.Center.Y - (1 + rand.NextDouble() % 0.5) * 60);

                                        });

                                        UIView.AddKeyframeWithRelativeStartTime(0, 0.08, () =>
                                                                {
                                                                });


                                        UIView.AddKeyframeWithRelativeStartTime(0.225, 0.25, () =>
                                        {
                                            image1.Alpha = 0;
                                        });




                                        if (images.Count > 1)
                                        {
                                            var image2 = images[1];
                                            UIView.AddKeyframeWithRelativeStartTime(0.25, 0.15, () =>
                                                                    {
                                                                        image2.Alpha = 1;
                                                                    });
                                            UIView.AddKeyframeWithRelativeStartTime(0.25, 0.25, () =>
                                            {
                                                image2.Center = new CGPoint(image2.Center.X, image2.Center.Y - (1 + rand.NextDouble() % 0.5) * 60);
                                            });

                                            UIView.AddKeyframeWithRelativeStartTime(0.475, 0.25, () =>
                                            {
                                                image2.Alpha = 0;
                                            });
                                        }

                                        if (images.Count > 2)
                                        {
                                            var image3 = images[2];
                                            UIView.AddKeyframeWithRelativeStartTime(0.5, 0.15, () =>
                                                                    {
                                                                        image3.Alpha = 1;
                                                                    });
                                            UIView.AddKeyframeWithRelativeStartTime(0.5, 0.25, () =>
                                            {
                                                image3.Center = new CGPoint(image3.Center.X, image3.Center.Y - (1 + rand.NextDouble() % 0.5) * 60);
                                            });
                                            UIView.AddKeyframeWithRelativeStartTime(0.725, 0.25, () =>
                                            {
                                                image3.Alpha = 0;
                                            });
                                        }
                                        if (images.Count > 3)
                                        {
                                            var image4 = images[3];
                                            UIView.AddKeyframeWithRelativeStartTime(0.75, 0.15, () =>
                                                                    {
                                                                        image4.Alpha = 1;
                                                                    });
                                            UIView.AddKeyframeWithRelativeStartTime(0.75, 0.25, () =>
                                            {
                                                image4.Center = new CGPoint(image4.Center.X, image4.Center.Y - (1 + rand.NextDouble() % 0.5) * 60);
                                            });
                                            UIView.AddKeyframeWithRelativeStartTime(0.75, 0.25, () =>
                                            {
                                                image4.Alpha = 0;
                                            });
                                        }
                                    }, completion: (i) =>
                                    {   
                                    });

            animationRunning = false;
        }

        static UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                if (data != null)
                {
                    return UIImage.LoadFromData(data);
                }
                else
                {
                    return UIImage.FromFile("cool");
                }
        }

        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            if (!string.IsNullOrWhiteSpace(customView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
            }
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            customMap.OnSelect = true;
            var pintoadd = new PinModel();

            var customView = e.View as CustomMKAnnotationView;
            foreach (X.Models.PinModel pin in customMap.Pins)
            {
                if (pin.Id.Equals(customView.Id))
                {
                    pintoadd = pin;
                }

            }
            //var newPin = new Xamarin.Forms.Maps.Position(pintoadd.Position.Latitude, pintoadd.Position.Longitude);
            var dMeters = 0.3 * customMap.VisibleRegion.Radius.Kilometers;
            var dLat = dMeters / 111.11;
            Console.WriteLine("delta latitude: {0}", dLat);
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(pintoadd.Position.Latitude-dLat, pintoadd.Position.Longitude), customMap.VisibleRegion.Radius));
            customPinView = new UIView();
            customMap.Pins.Clear();
            if (pintoadd.Label == null)
            {
                pintoadd.Label = "Scuttle";
            }
            customMap.Pins.Add(pintoadd);
            customMap.CustomPin.Clear();
            customMap.CustomPin.Add(pintoadd);
            customMap.showInfoBox(customView.Id);
                //if (customView.emojis != null)
                //{
                //    customPinView.Frame = new CGRect(0, -100, 40, 40);
                //    var image = new UIImageView(new CGRect(0, 0, 20, 20));
                //    image.Image = UIImage.FromFile(customView.emojis[0]);
                //    var image2 = new UIImageView(new CGRect(30, 30, 25, 25));
                //    image2.Image = UIImage.FromFile(customView.emojis[1]);
                //    var image3 = new UIImageView(new CGRect(0, 60, 30, 30));
                //    image3.Image = UIImage.FromFile(customView.emojis[2]);
                //    var image4 = new UIImageView(new CGRect(15, 90, 35, 35));
                //    image4.Image = UIImage.FromFile(customView.emojis[3]);
                //    customPinView.AddSubview(image);
                //    customPinView.AddSubview(image2);
                //    customPinView.AddSubview(image3);
                //    customPinView.AddSubview(image4);
                //    customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                //    e.View.AddSubview(customPinView);
                //}
                //else
                //{
                //    Console.WriteLine("No emojis");
                //}
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            
        }

        PinModel GetCustomPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in formsMap.CustomPin)
            {
                if (pin.Position == position)
                    return pin;
                 
            }
            return null;
        }
    }
}
