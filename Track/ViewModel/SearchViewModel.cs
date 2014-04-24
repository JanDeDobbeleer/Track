using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Tools;
using Track.Annotations;
using Track.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class SearchViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
        private INavigationService _navigationService;
        public RelayCommand ConnectionViewCommand { get; private set; }
        private readonly Helper _helper;

        public const string StationsPropertyName = "Stations";
        private ObservableCollection<string> _stations;
        public ObservableCollection<string> Stations
        {
            get
            {
                return _stations;
            }

            set
            {
                if (_stations == value)
                {
                    return;
                }

                _stations = value;
                OnPropertyChanged(StationsPropertyName);
            }
        }

        public const string DatePropertyName = "Date";
        private string _date = DateTime.Now.ToString("MM/dd/yyyy");
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged(DatePropertyName);
            }
        }

        public const string TimePropertyName = "Time";
        private string _time = DateTime.Now.ToShortTimeString();
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged(TimePropertyName);
            }
        }

        public const string FromPropertyName = "From";
        private string _from;
        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
                OnPropertyChanged(FromPropertyName);
            }
        }

        public const string ToPropertyName = "To";
        private string _to;
        public string To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
                OnPropertyChanged(ToPropertyName);
            }
        }
        #endregion

        public SearchViewModel(INavigationService navigationService)
        {
            _helper = new Helper();
            _navigationService = navigationService;
            Stations = new ObservableCollection<string>();
            ConnectionViewCommand = new RelayCommand(() =>
            {
                if (CheckForValidValues())
                    RailService.GetInstance().GetConnections(Date, Time, From, To);
            });
        }

        private bool CheckForValidValues()
        {
            if (string.IsNullOrWhiteSpace(From) || string.IsNullOrWhiteSpace(To))
            {
                Message.ShowToast(AppResources.MessageValidStationName);
                return false;
            }
            if (!Stations.Contains(From.Trim()) || !Stations.Contains(To.Trim()))
            {
                Message.ShowToast(AppResources.MessageValidStationName);
                return false;
            }
            return true;
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
