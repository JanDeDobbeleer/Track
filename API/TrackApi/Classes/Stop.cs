using System;
using Newtonsoft.Json;
using TrackApi.Tools;

namespace TrackApi.Classes
{
    public class Stop
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "station")]
        [JsonConverter(typeof(StationNameConverter))]
        public string Station { get; set; }
        [JsonProperty(PropertyName = "stationinfo")]
        public Station Stationinfo { get; set; }
        [JsonProperty(PropertyName = "time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }
        [JsonProperty(PropertyName = "delay")]
        [JsonConverter(typeof(DelayConverter))]
        public string Delay { get; set; }

        public string TimeStamp
        {
            get { return Time.ToString("HH:mm"); }
        }
    }
}
