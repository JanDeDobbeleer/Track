using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cimbalino.Phone.Toolkit.Extensions;
using TrackApi.Classes;

namespace Track.Database
{
    public class TrackDatabase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private TrackDataContext _trackDb;
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
            _trackDb = new TrackDataContext(DbConnectionString);
            LoadItems();
        }

        private void LoadItems()
        {
            var stationItemsInDb = from Station station in _trackDb.Stations
                                   select station;
            // Query the database and load all station items.
            if (stationItemsInDb.Any())
                Stations = new ObservableCollection<Station>(stationItemsInDb);

            var favoriteItemsInDb = from Favorite favorite in _trackDb.Favorites
                                    select favorite;
            // Query the database and load all station items.
            if (favoriteItemsInDb.Any())
                Favorites = new ObservableCollection<Favorite>(favoriteItemsInDb);
        }

        public void AddStations(List<Station> stations)
        {
            DeleteAllStations();
            _trackDb.Stations.InsertAllOnSubmit(stations);
            SubmitChanges();
            Stations.AddRange(stations);
        }

        private void DeleteAllStations()
        {
            try
            {
                var list = from Station station in _trackDb.Stations
                           select station;
                _trackDb.Stations.DeleteAllOnSubmit(list);
                SubmitChanges();
                Stations.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void AddFavorite(Favorite favorite)
        {
            try
            {
                _trackDb.Favorites.InsertOnSubmit(favorite);
                SubmitChanges();
                Favorites.Add(favorite);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SubmitChanges()
        {
            _trackDb.SubmitChanges();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
