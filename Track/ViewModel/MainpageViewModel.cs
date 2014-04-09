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
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Phone.Info;
using Microsoft.Practices.ServiceLocation;
using Track.Annotations;
using Track.Api;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class MainpageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region commands
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand<Station> DirectionsCommand { get; private set; }
        public RelayCommand<Station> StationOverviewCommand { get; private set; }
        #endregion

        #region properties
        private INavigationService _navigationService;
        private readonly Helper _helper;

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

        public MainpageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _helper = new Helper();
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
                try
                {
                    var manufacturer= string.Empty;
                    object temp;
                    if (DeviceExtendedProperties.TryGetValue("DeviceManufacturer", out temp))
                        manufacturer = temp.ToString();
                    if (manufacturer.Equals("NOKIA"))
                    {
                        try
                        {
                            _helper.OpenHereMaps(station);
                        }
                        catch (Exception)
                        {
                            _helper.OpenDefaultmaps(station);
                        }
                    }
                    else
                    {
                        _helper.OpenDefaultmaps(station);
                    }
                }
                catch (Exception)
                {
                    //TODO: handle this, guide the user to doznload a maps app?
                }
            });
            RefreshCommand = new RelayCommand(async () =>
            {
                ClearItems();   
                Task.WaitAll(Task.Factory.StartNew(() => GetDisruptions()));
                await GetCurrentPosition();
            });
            StationOverviewCommand = new RelayCommand<Station>(station =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<StationOverviewViewModel>().Station = station);
                _navigationService.NavigateTo(ViewModelLocator.StationOverviewPageUri);
            });
        }

        private void ClearItems()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Locations.Clear();
                Disruptions.Clear();
                Nearby.Clear();
                CurrentPosition = new GeoCoordinate();
                LocationLoaded = false;
                LoadingDisruptions = false; 
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
                _helper.AssignList(Locations,list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(10).ToList());
                //assign the nearby list
                _helper.AssignList(Nearby, list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(3).ToList());
            }
            catch (Exception e)
            {
                //TODO: Handle exception!
#if(DEBUG)
                Debug.WriteLine("MainViewmodel - GetLocation: " + e.Message);
#endif
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
