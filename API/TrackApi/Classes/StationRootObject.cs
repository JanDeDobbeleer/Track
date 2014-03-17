using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackApi.Classes
{
    public class StationRootObject
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty(PropertyName = "station")]
        public List<Station> Station { get; set; }

        public StationRootObject()
        {
            Station = new List<Station>();
        }
    }
}
