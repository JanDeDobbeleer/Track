using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Devices.Geolocation;
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Practices.ServiceLocation;
using Tools;
using Tools.Properties;
using Track.Api;
using Track.Common;
using Track.Controls;
using Track.Database;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class MainpageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region commands
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand<Station> DirectionsCommand { get; private set; }
        public RelayCommand<Station> StationOverviewCommand { get; private set; }
        public RelayCommand<Station> FavoriteCommand { get; private set; }
        public RelayCommand<Favorite> FavoriteNavigationCommand { get; private set; }
        public RelayCommand<Favorite> FavoriteRemoveCommand { get; private set; }
        #endregion

        #region properties
        private INavigationService _navigationService;
        private readonly Helper _helper;
        private MessagePrompt _messagePrompt;
        private const string LocationConsent = "LocationConsent";

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
        private ObservableCollection<Disruption> _disruptions  = new ObservableCollection<Disruption>();
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

        public const string AllClearVisibilityPropertyName = "AllClearVisibility";
        private bool _allClearVisibility = false;
        public bool AllClearVisibility
        {
            get
            {
                return _allClearVisibility;
            }
            private set
            {
                if (_allClearVisibility == value)
                    return;

                _allClearVisibility = value;
                OnPropertyChanged(AllClearVisibilityPropertyName);
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

        public const string FavoritePropertyName = "Favorites";
        private ObservableCollection<Favorite> _favorites = new ObservableCollection<Favorite>();
        public ObservableCollection<Favorite> Favorites
        {
            get
            {
                return _favorites;
            }

            set
            {
                if (_favorites == value)
                {
                    return;
                }

                _favorites = value;
                RaisePropertyChanged(FavoritePropertyName);
            }
        }

        public const string EmptyFavoritesVisibilityPropertyName = "EmptyFavorites";
        private bool _emptyFavorites;

        public bool EmptyFavorites
        {
            get
            {
                return _emptyFavorites;
            }
            private set
            {
                if (_emptyFavorites == value)
                    return;

                _emptyFavorites = value;
                OnPropertyChanged(EmptyFavoritesVisibilityPropertyName);
            }
        }

        public const string FavoritesTrianglePropertyName = "FavoriteTriangle";
        private bool _favoriteTriangle;

        public bool FavoriteTriangle
        {
            get
            {
                return _favoriteTriangle;
            }
            private set
            {
                if (_favoriteTriangle == value)
                    return;

                _favoriteTriangle = value;
                OnPropertyChanged(FavoritesTrianglePropertyName);
            }
        }
        #endregion

        public MainpageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _helper = new Helper();
            Favorites = ServiceLocator.Current.GetInstance<TrackDatabase>().Favorites;
            //set commands to work
            DirectionsCommand = new RelayCommand<Station>((station) => _helper.OpenMaps(station));
            RefreshCommand = new RelayCommand(() =>
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains(LocationConsent) || !(bool)IsolatedStorageSettings.ApplicationSettings[LocationConsent])
                {
                    _messagePrompt = new MessagePrompt {Body = new MessagePromptBody(), IsCancelVisible = true, Background = (SolidColorBrush)Application.Current.Resources["TrackColorBrush"], Foreground = new SolidColorBrush(Colors.White)};
                    _messagePrompt.Completed += OnMessagePromptCompleted;
                    _messagePrompt.Show();
                }
                else
                {
                    Refresh();
                }
                CheckForEmptyFavorites();
                ClearItems();
                Task.WaitAll(Task.Factory.StartNew(() => GetDisruptions()));
            });
            StationOverviewCommand = new RelayCommand<Station>(station =>
            {
                station.TimeStamp = DateTime.Now;
                Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<StationOverviewViewModel>().Station = station);
                _navigationService.NavigateTo(ViewModelLocator.StationOverviewPageUri);
            });
            SearchCommand = new RelayCommand(()=> _navigationService.NavigateTo(ViewModelLocator.SearchPageUri));
            FavoriteCommand = new RelayCommand<Station>(station =>
            {
                ServiceLocator.Current.GetInstance<TrackDatabase>().AddFavorite(station.ToFavorite(), true);
                CheckForEmptyFavorites();
            });
            FavoriteNavigationCommand = new RelayCommand<Favorite>(favorite => favorite.Navigate());
            FavoriteRemoveCommand = new RelayCommand<Favorite>(favorite =>
            {
                ServiceLocator.Current.GetInstance<TrackDatabase>().RemoveFavorite(favorite);
                CheckForEmptyFavorites();
            });
            AboutCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.AboutUri));
        }

        private void CheckForEmptyFavorites()
        {
            if (Favorites.Any())
            {
                FavoriteTriangle = true;
                EmptyFavorites = false;
            }
            else
            {
                FavoriteTriangle = false;
                EmptyFavorites = true;
            }
        }

        private async void Refresh()
        {
            while (_navigationService.CanGoBack)
            {
                _navigationService.RemoveBackEntry();
            }
            await GetCurrentPosition();
        }

        private void OnMessagePromptCompleted(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok)
            {
                IsolatedStorageSettings.ApplicationSettings[LocationConsent] = true;
                Refresh();
            }
            else
                IsolatedStorageSettings.ApplicationSettings[LocationConsent] = false;
            _messagePrompt.Completed -= OnMessagePromptCompleted;
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
                AllClearVisibility = false;
            });
        }

        private async Task GetDisruptions()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingDisruptions = true);
            Deployment.Current.Dispatcher.BeginInvoke(() => AllClearVisibility = false);
            var list = await Rss.GetInstance().GetDisruptions(AppResources.ClientLang);
            if (!list.Disruptions.Any())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => AllClearVisibility = true);
            }
            else
            {
                var tasks = Enumerable.Range(0, list.Disruptions.Count).Select(i =>
                Task.Run(() =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => Disruptions.Add(list.Disruptions[i]));
                }));
                await Task.WhenAll(tasks);
            }
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingDisruptions = false);
        }

        private async Task GetCurrentPosition()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingLocations = true);
            Geoposition geoposition = null;
            var list = await RailService.GetInstance().GetLocations(new KeyValuePair<String, String>(Arguments.Lang.ToString(), AppResources.ClientLang));
            if (list.Count == 0)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => LoadingLocations = false);
                return;
            }
            _helper.AssignList(ServiceLocator.Current.GetInstance<SearchViewModel>().Stations, list.Select(x => x.Name));
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
            await Task.Run(() => GetLocations(CurrentPosition, list));
            Deployment.Current.Dispatcher.BeginInvoke(() => { LocationLoaded = true; });
            Messenger.Default.Send(new NotificationMessage("StationsLoaded"));
            Deployment.Current.Dispatcher.BeginInvoke(() => LoadingLocations = false);
        }

        private async Task GetLocations(GeoCoordinate currentPhonePosition, List<Station> list)
        {
            try
            {
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
                _helper.AssignList(Nearby, list.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(6).ToList());
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
