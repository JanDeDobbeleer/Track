using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Track.Annotations;
using Track.Common;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public delegate void InfoFinishedEventHandler(object sender, FinshedEventArgs args);

    public class MainpageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
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
        #endregion

        public MainpageViewModel()
        {
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                if (message.Notification.Equals("StationsLoaded", StringComparison.OrdinalIgnoreCase))
                    Deployment.Current.Dispatcher.BeginInvoke(AssignList);
            });
        }

        private void AssignList()
        {
            Locations.Clear();
            Locations = null;
            Locations = ServiceLocator.Current.GetInstance<StationListViewmodel>().Stations;
            //assign currentposition
            CurrentPosition = ServiceLocator.Current.GetInstance<StationListViewmodel>().CurrentPosition;
            var top4Locations = ServiceLocator.Current.GetInstance<MainpageViewModel>().Locations.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(4);
            //clear nearby locations
            Nearby.Clear();
            foreach (var top4Location in top4Locations)
            {
                Nearby.Add(top4Location);
            }
            Messenger.Default.Send(new NotificationMessage("LocationsLoaded"));
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
