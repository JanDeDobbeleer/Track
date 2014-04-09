using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using Track.Annotations;
using Track.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class StationOverviewViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region properties
        private INavigationService _navigationService;
        private readonly Helper _helper;

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
                GetLiveBoard(_station);
                OnPropertyChanged(StationPropertyName);
            }
        }

        public const string DeparturesPropertyName = "Station";
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
                OnPropertyChanged(DeparturesPropertyName);
            }
        }
        #endregion

        private async void GetLiveBoard(Station station)
        {
            var list = await RailService.GetInstance().GetLiveBoard(station);
            _helper.AssignList(Departures, list);
        }

        public StationOverviewViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _helper = new Helper();
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
