using System;
using Newtonsoft.Json;
using TrackApi.Tools;

namespace TrackApi.Classes
{
    public class Departure
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "delay")]
        [JsonConverter(typeof(DelayConverter))]
        public string Delay { get; set; }
        [JsonProperty(PropertyName = "station")]
        [JsonConverter(typeof(StationNameConverter))]
        public string Station { get; set; }
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Stationinfo { get; set; }
        [JsonProperty(PropertyName = "time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public String Time { get; set; }
        [JsonProperty(PropertyName = "vehicle")]
        [JsonConverter(typeof(VehicleNameConverter))]
        public string Vehicle { get; set; }
        [JsonProperty(PropertyName = "platform")]
        [JsonConverter(typeof(PlatformConverter))]
        public string Platform { get; set; }
        [JsonProperty(PropertyName = "platforminfo")]
        public PlatformInfo PlatformInfo { get; set; }
        [JsonProperty(PropertyName = "left")]
        public string Left { get; set; }

        public string VehicleName()
        {
            return "BE.NMBS." + Vehicle;
        }
    }
}
