using System;
using System.Collections.Generic;
using TrackApi.Classes;

namespace Track.Api
{
    internal class StorageCache
    {
        public DateTime CacheDate { get; set; }
        public List<Station> CacheData { get; set; }
    }
}
