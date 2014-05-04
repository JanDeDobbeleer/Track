using System.Collections.Generic;
using Newtonsoft.Json;

namespace TrackApi.Classes
{
    public class Stops
    {
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }
        [JsonProperty(PropertyName = "stop")]
        public List<Stop> Stop { get; set; }

        public Stops()
        {
            Stop = new List<Stop>();
        }
    }
}
