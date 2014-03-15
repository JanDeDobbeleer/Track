using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
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
