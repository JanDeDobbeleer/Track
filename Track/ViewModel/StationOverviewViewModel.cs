using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using Track.Annotations;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class StationOverviewViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
        private INavigationService _navigationService;

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
                if (_station == value)
                    return;

                _station = value;
                OnPropertyChanged(StationPropertyName);
            }
        }
        #endregion

        public StationOverviewViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
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
