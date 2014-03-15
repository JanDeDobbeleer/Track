using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;
using TrackApi.Tools;

namespace Track.ViewModel
{
    public class StationListViewmodel : ViewModelBase
    {
        public ObservableCollection<Station> Stations { get; private set; }
        private ReverseGeocodeQuery _reverseGeocodeQuery = null;

        public const string CurrentPositionPropertyName = "CurrentPosition";
        private GeoCoordinate _currentPosition;
        public GeoCoordinate CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            private set
            {
                if (_currentPosition == value)
                    return;

                _currentPosition = value;
                RaisePropertyChanged(CurrentPositionPropertyName);
            }
        }

        public const string CurrentPositionAsTextPropertyName = "CurrentPositionAsText";
        private string _currentPositionAsText;
        public string CurrentPositionAsText
        {
            get
            {
                return _currentPositionAsText;
            }
            private set
            {
                if (_currentPositionAsText == value)
                    return;

                _currentPositionAsText = value;
                RaisePropertyChanged(CurrentPositionAsTextPropertyName);
            }
        }

        #region eventhandling
        public event InfoFinishedEventHandler GetInfoFinished;

        protected void OnGetInfoFinished(FinshedEventArgs args)
        {
            if (GetInfoFinished != null)
            {
                GetInfoFinished(this, args);
            }
        }
        #endregion

        public void LoadStations()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                global::Tools.Tools.SetProgressIndicator(true);
                SystemTray.ProgressIndicator.Text = AppResources.ProgressLoadingStations;
            });
            var client = new Client();
            client.GetInfoFinished += client_GetInfoFinished;
            client.GetInfo(client.Stations, new KeyValuePair<String, String>(Arguments.Lang.ToString(), AppResources.ClientLang));
        }

        private async void client_GetInfoFinished(object sender, GetInfoCompletedArgs args)
        {
            try
            {
                var stationlist = JsonConvert.DeserializeObject<StationRootObject>(args.Json);
                Stations = new ObservableCollection<Station>(stationlist.Station);
                var geo = await GetCurrentPosition();
                foreach (var station in Stations)
                {
                    station.DistanceToCurrentPhonePosition = Geocoding.CalculateDistanceFrom(geo, station.GeoCoordinate);
                }
                Messenger.Default.Send(new NotificationMessage("StationsLoaded"));
                OnGetInfoFinished(new FinshedEventArgs() { InfoLocation = InfoLocation.Stations, Success = true });
            }
            catch (Exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => Tools.Tools.SetProgressIndicator(false));
                //TODO: handle this properly
            }
        }

        private async Task<GeoCoordinate> GetCurrentPosition()
        {
            Geoposition geoposition = null;

            var geolocator = new Geolocator
            {
                DesiredAccuracy = PositionAccuracy.High
            };
            try
            {
                geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
            }
            catch (Exception e)
            {
                //TODO: Handle exception!
#if(DEBUG)
                Debug.WriteLine("MainViewmodel - GetCurrentPosition: " + e.Message);
#endif
            }
            CurrentPosition = geoposition.Coordinate.ToGeoCoordinate();
            Deployment.Current.Dispatcher.BeginInvoke(HandleReverseGeoCodeQuery);
            return CurrentPosition;
        }

        private void HandleReverseGeoCodeQuery()
        {
            if (ReferenceEquals(_reverseGeocodeQuery, null) || !_reverseGeocodeQuery.IsBusy)
            {
                _reverseGeocodeQuery = new ReverseGeocodeQuery();
                _reverseGeocodeQuery.GeoCoordinate = CurrentPosition;
                _reverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
                _reverseGeocodeQuery.QueryAsync();
            }
        }

        private void ReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MapAddress address = e.Result[0].Information.Address;
                    StringBuilder addressDisplay = new StringBuilder();
                    addressDisplay.Append(address.Street);
                    addressDisplay.Append(string.IsNullOrEmpty(address.HouseNumber) ? string.Empty : string.Concat(" ", address.HouseNumber));
                    addressDisplay.Append(string.IsNullOrEmpty(address.PostalCode) ? string.Empty : string.Concat(", ", address.PostalCode));
                    addressDisplay.Append(string.IsNullOrEmpty(address.City) ? string.Empty : string.Concat(" ", address.City));
                    addressDisplay.Append(string.IsNullOrEmpty(address.Country) ? string.Empty : string.Concat(", ", address.Country));
                    CurrentPositionAsText = addressDisplay.ToString();
                }
            }
        }
    }
}
