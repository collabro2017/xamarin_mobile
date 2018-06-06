using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Newtonsoft.Json.Linq;
//using Plugin.Connectivity;
using X.API;
using X.API.Base;
using X.Models;
using Xamarin.Forms.Maps;

namespace X
{
    public class CustomMap : Map, IBaseAPIInterface
    {
        public ObservableCollection<PinModel> CustomPin { get; set; }
        public bool OnSelect { get; set; }
        public bool OnSearch { get; set; }
        public interface Iplaces
        {
            void didGetPlaces(JObject res);
            void showinfo(string id);
            void DidHideInfoview();
        }

        public Iplaces placeCallBack { get; set; }


        public void showInfoBox(string id)
        {
            placeCallBack.showinfo(id);
        }

        public void getplace(double lat, double lng, int zoom)
        {
            FetchPlacesAPI api = new FetchPlacesAPI(lat.ToString(),lng.ToString(), zoom);
            api.setCallbacks(this);
            api.getResponse();        
        }

        public void hideInfoView()
        {
            if (placeCallBack != null)
            {
                placeCallBack.DidHideInfoview();
            }
        }

        public void OnSuccess(JObject response, BaseAPI caller)
        {
            if (placeCallBack != null)
            {
                placeCallBack.didGetPlaces(response);
            }
        }

        public void OnError(string errMsg, BaseAPI caller)
        {
            
        }

        public void OnErrorCode(int errorCode, BaseAPI caller)
        {
            throw new NotImplementedException();
        }
    }
}
