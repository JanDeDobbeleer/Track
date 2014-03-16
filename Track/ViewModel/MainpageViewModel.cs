using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Phone.Maps.Services;
using Microsoft.Practices.ServiceLocation;
using Track.Annotations;
using Track.Api;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public delegate void InfoFinishedEventHandler(object sender, FinshedEventArgs args);

    public class MainpageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
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
                OnPropertyChanged(CurrentPositionPropertyName);
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

        public const string LocationsPropertyName = "Locations";
        private ObservableCollection<Station> _locations = new ObservableCollection<Station>();
        public ObservableCollection<Station> Locations
        {
            get
            {
                return _locations;
            }

            set
            {
                if (_locations == value)
                {
                    return;
                }

                _locations = value;
                OnPropertyChanged(LocationsPropertyName);
            }
        }

        public const string NearbyPropertyName = "Nearby";
        private ObservableCollection<Station> _nearby = new ObservableCollection<Station>();
        public ObservableCollection<Station> Nearby
        {
            get
            {
                return _nearby;
            }

            set
            {
                if (_nearby == value)
                {
                    return;
                }

                _nearby = value;
                OnPropertyChanged(NearbyPropertyName);
            }
        }
        #endregion

        public MainpageViewModel()
        {
            Messenger.Default.Register<NotificationMessage>(this, async (message) =>
            {
                if (message.Notification.Equals("MainPageLoaded", StringComparison.OrdinalIgnoreCase))
                    await GetCurrentPosition();
            });
        }

        private async Task GetCurrentPosition()
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
            await Task.Run(() => GetLocations(CurrentPosition));
        }

        private async Task GetLocations(GeoCoordinate currentPhonePosition)
        {
            var list = await RailService.GetInstance().GetLocations(new KeyValuePair<String, String>(Arguments.Lang.ToString(), AppResources.ClientLang));
            Locations.Clear();
            Locations = null;
            foreach (var station in list)
            {
                Locations.Add(station);
            }
            foreach (var station in Locations)
            {
                station.DistanceToCurrentPhonePosition = Geocoding.CalculateDistanceFrom(currentPhonePosition, station.GeoCoordinate);
            }
            var top4Locations = Locations.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(4);
            //clear nearby locations
            Nearby.Clear();
            foreach (var top4Location in top4Locations)
            {
                Nearby.Add(top4Location);
            }
            Messenger.Default.Send(new NotificationMessage("StationsLoaded"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
