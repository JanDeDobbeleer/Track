using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Tasks;
using Track.Annotations;
using Track.Api;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;
using Nokia.Phone.HereLaunchers;

namespace Track.ViewModel
{
    public class MainpageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region commands
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand<Station> DirectionsCommand { get; private set; }
        #endregion

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
                RaisePropertyChanged(LocationsPropertyName);
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
                RaisePropertyChanged(NearbyPropertyName);
            }
        }

        public const string DisruptionsPropertyName = "Disruptions";
        private ObservableCollection<Disruption> _disruptions = new ObservableCollection<Disruption>();
        public ObservableCollection<Disruption> Disruptions
        {
            get
            {
                return _disruptions;
            }

            set
            {
                if (_disruptions == value)
                {
                    return;
                }

                _disruptions = value;
                OnPropertyChanged(DisruptionsPropertyName);
            }
        }

        public const string LoadingPropertyName = "LoadingLocations";
        private bool _loadingLocations = false;
        public bool LoadingLocations
        {
            get
            {
                return _loadingLocations;
            }
            private set
            {
                if (_loadingLocations == value)
                    return;

                _loadingLocations = value;
                OnPropertyChanged(LoadingPropertyName);
            }
        }

        public const string LoadingDisruptionsPropertyName = "LoadingDisruptions";
        private bool _loadingDisruptions = false;
        public bool LoadingDisruptions
        {
            get
            {
                return _loadingDisruptions;
            }
            private set
            {
                if (_loadingDisruptions == value)
                    return;

                _loadingDisruptions = value;
                OnPropertyChanged(LoadingDisruptionsPropertyName);
            }
        }
        #endregion

        public MainpageViewModel()
        {
            Messenger.Default.Register<NotificationMessage>(this, async (message) =>
            {
                if (!message.Notification.Equals("MainPageLoaded", StringComparison.OrdinalIgnoreCase)) 
                    return;
                Task.WaitAll(Task.Factory.StartNew(() => GetDisruptions()));
                await GetCurrentPosition();
            });
            //set commands to work
            DirectionsCommand = new RelayCommand<Station>((station) =>
            {
                var manufacturer= string.Empty;
                object temp;
                if (Microsoft.Phone.Info.DeviceExtendedProperties.TryGetValue("DeviceManufacturer", out temp))
                    manufacturer = temp.ToString();
                if (manufacturer.Equals("NOKIA"))
                {
                    var routeTo = new DirectionsRouteDestinationTask
                    {
                        Destination = station.GeoCoordinate,
                        Mode = RouteMode.Unknown
                    };
                    routeTo.Show();
                }
                else
                {
                    var bingMapsDirectionsTask = new BingMapsDirectionsTask();
                    var mapLocation = new LabeledMapLocation(string.Format(AppResources.NavigationStation, station.Name), station.GeoCoordinate);
                    bingMapsDirectionsTask.End = mapLocation;
                    // If bingMapsDirectionsTask.Start is not set, the user's current location is used as the start point.
                    bingMapsDirectionsTask.Show();
                }
            });
            RefreshCommand = new RelayCommand(async () =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(Locations.Clear);
                Deployment.Current.Dispatcher.BeginInvoke(()=> { CurrentPosition = new GeoCoordinate(); });
                Deployment.Current.Dispatcher.BeginInvoke(()=> { LocationLoaded = false; });
                Deployment.Current.Dispatcher.BeginInvoke(()=> { LoadingDisruptions = false; });
                Deployment.Current.Dispatcher.BeginInvoke(Disruptions.Clear);
                Deployment.Current.Dispatcher.BeginInvoke(Nearby.Clear);
                Task.WaitAll(Task.Factory.StartNew(() => GetDisruptions()));
                await GetCurrentPosition();
            });
        }

        private async Task GetDisruptions()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingDisruptions = true);
            var list = await Rss.GetInstance().GetDisruptions(AppResources.ClientLang);
            var tasks = Enumerable.Range(0, list.Disruptions.Count).Select(i =>
              Task.Run(() =>
              {
                  Deployment.Current.Dispatcher.BeginInvoke(() => Disruptions.Add(list.Disruptions[i]));
              }));
            await Task.WhenAll(tasks);
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingDisruptions = false);
        }

        private async Task GetCurrentPosition()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingLocations = true);
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
            await Task.Run(() => GetLocations(CurrentPosition));
            Deployment.Current.Dispatcher.BeginInvoke(() => { LocationLoaded = true; });
            Messenger.Default.Send(new NotificationMessage("StationsLoaded"));
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingLocations = false);
        }

        private async Task GetLocations(GeoCoordinate currentPhonePosition)
        {
            try
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
                AssignList(Locations,list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(10).ToList());
                //assign the nearby list
                AssignList(Nearby, list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(3).ToList());
            }
            catch (Exception e)
            {
                //TODO: Handle exception!
#if(DEBUG)
                Debug.WriteLine("MainViewmodel - GetLocation: " + e.Message);
#endif
            }
        }

        private void AssignList(ICollection<Station> input, IEnumerable<Station> stations)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var station in stations)
                {
                    input.Add(station);
                }
            });
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
