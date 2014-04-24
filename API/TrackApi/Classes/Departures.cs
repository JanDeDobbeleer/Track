using System.Collections.Generic;

namespace TrackApi.Classes
{
    public class Departures
    {
        public string number { get; set; }
        public List<Departure> departure { get; set; }

        public Departures()
        {
            departure = new List<Departure>();
        }
    }
}
