using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    public class VehicleOverviewViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
        private INavigationService _navigationService;
        private readonly Helper _helper;

        public RelayCommand<Stop> StationOverViewCommand { get; private set; }

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
                GetVehicleInfo();
                OnPropertyChanged(VehiclePropertyName);
            }
        }

        public const string StopsPropertyName = "Stops";
        private ObservableCollection<Stop> _stops = new ObservableCollection<Stop>();
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
            StationOverViewCommand = new RelayCommand<Stop>((stop) =>
            {
                stop.Stationinfo.Id = stop.Time;
                Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<StationOverviewViewModel>().Station = stop.Stationinfo);
                _navigationService.NavigateTo(ViewModelLocator.StationOverviewPageUri);
            });
        }

        private async void GetVehicleInfo()
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Stops.Clear();
                    Loading = true;
                });
                var list = await RailService.GetInstance().GetVehicle(Vehicle);
                if (list == null)
                {
                    Message.ShowToast(AppResources.MessageVehicleInfoError);
                    return;
                }
                _helper.AssignList(Stops, list);
                Deployment.Current.Dispatcher.BeginInvoke(() => Loading = false);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
