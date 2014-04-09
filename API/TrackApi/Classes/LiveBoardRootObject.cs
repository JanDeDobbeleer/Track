namespace TrackApi.Classes
{
    public class LiveBoardRootObject
    {
        public string version { get; set; }
        public string timestamp { get; set; }
        public string station { get; set; }
        public Station stationinfo { get; set; }
        public Departures departures { get; set; }
    }
}
