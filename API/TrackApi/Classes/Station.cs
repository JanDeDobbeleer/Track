using System;
using System.Device.Location;
using Newtonsoft.Json;

namespace TrackApi.Classes
{
    public class Station
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "locationX")]
        public string LocationX { get; set; }
        [JsonProperty(PropertyName = "locationY")]
        public string LocationY { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "standardname")]
        public string Standardname { get; set; }

        //create a geocordinate based on the X and Y
        private GeoCoordinate _geoCoordinate;
        public GeoCoordinate GeoCoordinate
        {
            get
            {
                if (ReferenceEquals(_geoCoordinate, null))
                {
                    _geoCoordinate = new GeoCoordinate(double.Parse(LocationY.Replace('.', ',')), double.Parse(LocationX.Replace('.', ',')));
                }
                return _geoCoordinate;
            }
        }

        public double DistanceToCurrentPhonePosition { get; set; }
    }
}
