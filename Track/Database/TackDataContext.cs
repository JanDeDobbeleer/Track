using System.Data.Linq;
using TrackApi.Classes;

namespace Track.Database
{
    public class TrackDataContext: DataContext
    {
        public TrackDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<Station> Stations;
        public Table<Favorite> Favorites;
    }
}
