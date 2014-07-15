using System;
using System.Collections.ObjectModel;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Track.ViewModel
{
    public class ConnectionViewModel : ViewModelBase
    {
        #region properties
        private readonly INavigationService _navigationService;
        public RelayCommand ConnectionViewCommand { get; private set; }
        public RelayCommand ShowListCommand { get; private set; }

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
                RaisePropertyChanged(StationsPropertyName);
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
                RaisePropertyChanged(TimePropertyName);
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
                RaisePropertyChanged(FromPropertyName);
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
                RaisePropertyChanged(ToPropertyName);
            }
        }

        public const string SelectedStationPropertyName = "SelectedStation";
        private string _selectedStation;
        public string SelectedStation
        {
            get
            {
                return _selectedStation;
            }
            set
            {
                _selectedStation = value;
                RaisePropertyChanged(SelectedStationPropertyName);
            }
        }

        public const string ListVisiblePropertyName = "ListVisible";
        private bool _listVisible = false;
        public bool ListVisible
        {
            get
            {
                return _listVisible;
            }
            private set
            {
                if (_listVisible == value)
                    return;

                _listVisible = value;
                RaisePropertyChanged(ListVisiblePropertyName);
            }
        }
        #endregion

        public ConnectionViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ShowListCommand = new RelayCommand(() => Deployment.Current.Dispatcher.BeginInvoke(() => ListVisible = true));
        }
    }
}
