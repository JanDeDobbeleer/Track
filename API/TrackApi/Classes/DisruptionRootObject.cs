using System;
using System.Collections.Generic;

namespace TrackApi.Classes
{
    public class DisruptionRootObject
    {
        public DateTime Timestamp { get; private set; }
        public List<Disruption> Disruptions { get; set; }

        public DisruptionRootObject()
        {
            Timestamp = DateTime.Now;
            Disruptions = new List<Disruption>();
        }
    }
}
