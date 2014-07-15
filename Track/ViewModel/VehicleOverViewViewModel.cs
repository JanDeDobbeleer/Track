using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Tools.Properties;
using Track.Api;
using Track.Database;
using TrackApi.Classes;
using Type = Track.Database.Type;

namespace Track.ViewModel
{
    public class VehicleOverviewViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
        private INavigationService _navigationService;
        private readonly Helper _helper;

        public RelayCommand<Stop> StationOverViewCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand FavoriteCommand { get; private set; }
        public RelayCommand HomeCommand { get; private set; }

        public const string VehiclePropertyName = "Vehicle";
        private string _vehicle;
        public string Vehicle
        {
            get
            {
                return _vehicle;
            }
            set
            {
                _vehicle = value;
                OnPropertyChanged(VehiclePropertyName);
            }
        }

        public const string StopsPropertyName = "Stops";
        private ObservableCollection<Stop> _stops;
        public ObservableCollection<Stop> Stops
        {
            get
            {
                return _stops;
            }
            set
            {
                if (_stops == value)
                    return;
                _stops = value;
                RaisePropertyChanged(StopsPropertyName);
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

        public VehicleOverviewViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _helper = new Helper();
            Stops = new ObservableCollection<Stop>();
            StationOverViewCommand = new RelayCommand<Stop>((stop) =>
            {
                stop.Stationinfo.TimeStamp = stop.Time;
                Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<StationOverviewViewModel>().Station = stop.Stationinfo);
                _navigationService.NavigateTo(ViewModelLocator.StationOverviewPageUri);
            });
            RefreshCommand = new RelayCommand(GetVehicleInfo);
            FavoriteCommand = new RelayCommand(() =>
            {
                var fav = new Favorite
                {
                    Name = Vehicle,
                    Type = Type.Vehicle,
                    QueryElement = Vehicle,
                    Detail =
                        Stops.ElementAt(0).Station + " - " + Stops.ElementAt(0).TimeStamp + " • " + Stops.Last().Station + " - " +
                        Stops.Last().TimeStamp
                };
                ServiceLocator.Current.GetInstance<TrackDatabase>().AddFavorite(fav);
            });
            HomeCommand = new RelayCommand(() => _navigationService.NavigateTo(ViewModelLocator.HomePageUri));
        }

        private async void GetVehicleInfo()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Stops.Clear();
                Loading = true;
            });
            var list = await RailService.GetInstance().GetVehicle(Vehicle);
            if (list.Count == 0)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => Loading = false);
                return;
            }
            _helper.AssignList(Stops, list);
            Deployment.Current.Dispatcher.BeginInvoke(() => Loading = false);
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
