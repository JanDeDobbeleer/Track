using System;
using System.Device.Location;
using Localization.Resources;
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

        public string Distance
        {
            get
            {
                if (DistanceToCurrentPhonePosition > 2)
                    return Math.Round(DistanceToCurrentPhonePosition, 2) + " " +  AppResources.Kilometers;
                if (DistanceToCurrentPhonePosition > 1)
                    return Math.Round(DistanceToCurrentPhonePosition, 2) + " " + AppResources.Kilometer;
                return Math.Round((DistanceToCurrentPhonePosition * 1000), 0) + " " + AppResources.Meters;
            }
        }
    }
}
