using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TrackApi.Classes;

namespace Track.Database
{
    public class TrackDatabase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private const string DbConnectionString = "Data Source=isostore:/Database.sdf";

        public const string StationsPropertyName = "Stations";
        private ObservableCollection<Station> _stations = new ObservableCollection<Station>();
        public ObservableCollection<Station> Stations
        {
            get { return _stations; }
            set
            {
                _stations = value;
                OnPropertyChanged(StationsPropertyName);
            }
        }

        public const string FavoritesPropertyName = "Favorites";
        private ObservableCollection<Favorite> _favorites = new ObservableCollection<Favorite>();
        public ObservableCollection<Favorite> Favorites
        {
            get { return _favorites; }
            set
            {
                _favorites = value;
                OnPropertyChanged(FavoritesPropertyName);
            }
        }

        public TrackDatabase()
        {
            LoadItems();
        }

        private void LoadItems()
        {
            using (var db = new TrackDataContext(DbConnectionString))
            {
                var stationItemsInDb = from Station station in db.Stations
                                       select station;
                // Query the database and load all station items.
                if (stationItemsInDb.Any())
                    Stations = new ObservableCollection<Station>(stationItemsInDb);

                var favoriteItemsInDb = from Favorite favorite in db.Favorites
                                       select favorite;
                // Query the database and load all station items.
                if (favoriteItemsInDb.Any())
                    Favorites = new ObservableCollection<Favorite>(favoriteItemsInDb);
            }
        }



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
