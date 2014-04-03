using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using Track.Annotations;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class StationDetailsViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public const string CurrentPositionPropertyName = "Station";
        private Station _station;
        public Station Station
        {
            get
            {
                return _station;
            }
            private set
            {
                if (_station == value)
                    return;

                _station = value;
                OnPropertyChanged(CurrentPositionPropertyName);
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
