using Newtonsoft.Json;
using TrackApi.Tools;

namespace TrackApi.Classes
{
    public class Stop
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "station")]
        public string Station { get; set; }
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Stationinfo { get; set; }
        [JsonProperty(PropertyName = "time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public string Time { get; set; }
        [JsonProperty(PropertyName = "delay")]
        [JsonConverter(typeof(DelayConverter))]
        public string Delay { get; set; }
    }
}
