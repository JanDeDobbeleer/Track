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
using Track.Annotations;
using Track.Api;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
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

        public const string LocationLoadedPropertyName = "LocationLoaded";
        private bool _locationLoaded = false;
        public bool LocationLoaded
        {
            get
            {
                return _locationLoaded;
            }
            private set
            {
                if (_locationLoaded == value)
                    return;

                _locationLoaded = value;
                OnPropertyChanged(LocationLoadedPropertyName);
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

        public const string LoadingPropertyName = "Loading";
        private bool _loading = false;
        public bool Loading
        {
            get
            {
                return _loading;
            }
            private set
            {
                if (_loading == value)
                    return;

                _loading = value;
                OnPropertyChanged(LoadingPropertyName);
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

        public async Task GetCurrentPosition()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => Loading = true);
            //Deployment.Current.Dispatcher.BeginInvoke(() => Tools.Tools.SetProgressIndicator(true, AppResources.ProgressAquiringLocation));
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
            //TODO: check if you need this, if not needed anywhere by release, remove it
            //HandleReverseGeoCodeQuery();
            await Task.Run(() => GetLocations(CurrentPosition));
            Deployment.Current.Dispatcher.BeginInvoke(() => { LocationLoaded = true; });
            Messenger.Default.Send(new NotificationMessage("StationsLoaded"));
            Deployment.Current.Dispatcher.BeginInvoke(() => Loading = false);
        }

        private async Task GetLocations(GeoCoordinate currentPhonePosition)
        {
            var list = await RailService.GetInstance().GetLocations(new KeyValuePair<String, String>(Arguments.Lang.ToString(), AppResources.ClientLang));
            //parallelize this for optimization
            var tasks = Enumerable.Range(0, list.Count).Select(i =>
              Task.Run(() =>
              {
                  list[i].DistanceToCurrentPhonePosition = Geocoding.CalculateDistanceFrom(currentPhonePosition, list[i].GeoCoordinate);
              }));
            await Task.WhenAll(tasks);
            //send the list to the StationList control
            Messenger.Default.Send(list);
            //assign the visible pins on the map, limited to 10 to improve speed
            await Task.Run(() => AssignList(Locations,list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(10).ToList()));
            //assign the nearby list
            await Task.Run(() => AssignList(Nearby, list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(3).ToList()));
        }

        private async Task AssignList(ICollection<Station> input, IReadOnlyList<Station> stations)
        {
            //parallelize this for optimization
            var tasks = Enumerable.Range(0, stations.Count).Select(i =>
              Task.Run(() =>
              {
                  Deployment.Current.Dispatcher.BeginInvoke(() => input.Add(stations[i]));
              }));
            await Task.WhenAll(tasks);
        }

        private void HandleReverseGeoCodeQuery()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (ReferenceEquals(_reverseGeocodeQuery, null) || !_reverseGeocodeQuery.IsBusy)
                {
                    _reverseGeocodeQuery = new ReverseGeocodeQuery();
                    _reverseGeocodeQuery.GeoCoordinate = CurrentPosition;
                    _reverseGeocodeQuery.QueryCompleted += ReverseGeocodeQuery_QueryCompleted;
                    _reverseGeocodeQuery.QueryAsync();
                }
            });
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
