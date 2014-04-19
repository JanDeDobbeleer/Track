using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localization.Resources;
using Microsoft.Practices.ServiceLocation;
using Tools;
using Track.Annotations;
using Track.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class StationOverviewViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private INavigationService _navigationService;
        private readonly Helper _helper;

        public RelayCommand DirectionsCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand<Departure> VehicleOverViewCommand { get; private set; }

        #region properties
        public const string StationPropertyName = "Station";
        private Station _station;
        public Station Station
        {
            get
            {
                return _station;
            }
            set
            {
                _station = value;
                GetLiveBoard(value);
                OnPropertyChanged(StationPropertyName);
            }
        }

        public const string DeparturesPropertyName = "Departures";
        private ObservableCollection<Departure> _departures = new ObservableCollection<Departure>();
        public ObservableCollection<Departure> Departures
        {
            get
            {
                return _departures;
            }
            set
            {
                if (_departures == value)
                    return;

                _departures = value;
                RaisePropertyChanged(DeparturesPropertyName);
            }
        }

        public const string LoadingPropertyName = "LoadingDepartures";
        private bool _loadingDepartures = false;
        public bool LoadingDepartures
        {
            get
            {
                return _loadingDepartures;
            }
            private set
            {
                if (_loadingDepartures == value)
                    return;

                _loadingDepartures = value;
                OnPropertyChanged(LoadingPropertyName);
            }
        }
        #endregion

        private async void GetLiveBoard(Station station)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Departures.Clear();
                    LoadingDepartures = true;
                });
                var list = await RailService.GetInstance().GetLiveBoard(station);
                if (list == null)
                {
                    Message.ShowToast(AppResources.MessageStationInfoError);
                    return;
                }
                _helper.AssignList(Departures, list);
                Deployment.Current.Dispatcher.BeginInvoke(() => LoadingDepartures = false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public StationOverviewViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _helper = new Helper();
            DirectionsCommand = new RelayCommand(() => _helper.OpenMaps(Station));
            RefreshCommand = new RelayCommand(() => Task.WaitAll(Task.Factory.StartNew(() => GetLiveBoard(Station))));
            VehicleOverViewCommand = new RelayCommand<Departure>((departure) =>
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<VehicleOverviewViewModel>().Vehicle = departure.Vehicle);
                _navigationService.NavigateTo(ViewModelLocator.VehicleOverviewPageUri);
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
