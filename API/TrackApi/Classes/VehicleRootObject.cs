using Newtonsoft.Json;

namespace TrackApi.Classes
{
    public class VehicleRootObject
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty(PropertyName = "vehicle")]
        public string Vehicle { get; set; }
        [JsonProperty(PropertyName = "vehicleinfo")]
        public VehicleInfo Vehicleinfo { get; set; }
        [JsonProperty(PropertyName = "stops")]
        public Stops Stops { get; set; }
    }
}
