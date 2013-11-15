using System.Device.Location;

namespace TrackApi.Classes
{
    public class Station
    {
        public string Id { get; set; }
        public string LocationX { get; set; }
        public string LocationY { get; set; }
        public string Name { get; set; }

        private GeoCoordinate _GeoCoordinate;
        public GeoCoordinate GeoCoordinate
        {
            get
            {
                if (ReferenceEquals(_GeoCoordinate, null))
                    _GeoCoordinate = new GeoCoordinate(double.Parse(LocationX), double.Parse(LocationY));

                return _GeoCoordinate;
            }
        }
    }
}
