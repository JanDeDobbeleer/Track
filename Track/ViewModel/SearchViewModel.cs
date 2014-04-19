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
using Track.Annotations;
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
        private ObservableCollection<string> _stations = new ObservableCollection<string>();
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
        private string _date;
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(DatePropertyName);
            }
        }

        public const string TimePropertyName = "Time";
        private string _time;
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
            Date = DateTime.Now.ToShortDateString();
            Time = DateTime.Now.ToShortTimeString();
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
