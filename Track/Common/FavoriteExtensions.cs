using Track.Database;
using TrackApi.Classes;

namespace Track.Common
{
    public static class FavoriteExtensions
    {
        public static Favorite ToFavorite(this Station station)
        {
            var fav = new Favorite
            {
                Name = station.Name, 
                QueryElement = station.Id,
                Type = Type.Station
            };
            return fav;
        }

        public static Favorite ToFavorite(this VehicleInfo vehicle)
        {
            var fav = new Favorite
            {
                Name = vehicle.name,
                QueryElement = vehicle.name,
                Type = Type.Vehicle
            };
            return fav;
        }
    }
}
