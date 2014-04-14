using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using Cimbalino.Phone.Toolkit.Services;
using Localization.Resources;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.Api
{
    public class RailService
    {
        #region Constructor
        private static RailService _sInstance;
        private static readonly object SInstanceSync = new object();

        protected RailService()
        {
        }

        /// <summary>
        /// Returns an instance (a singleton)
        /// </summary>
        /// <returns>a singleton</returns>
        /// <remarks>
        /// This is an implementation of the singelton design pattern.
        /// </remarks>
        public static RailService GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (_sInstance == null)
            {
                lock (SInstanceSync)
                {
                    if (_sInstance == null)
                    {
                        _sInstance = new RailService();
                    }
                }
            }
            return _sInstance;
        }
        #endregion

        public async Task<List<Station>> GetLocations(KeyValuePair<String, String> valuePair)
        {
            var requestFromInternet = true;
            var stationCache = string.Empty;
            var stationCacheExists = await ServiceLocator.Current.GetInstance<IAsyncStorageService>().FileExistsAsync(Constants.LOCATIONSSTORE);

            if(stationCacheExists)
                stationCache = await ServiceLocator.Current.GetInstance<IAsyncStorageService>().ReadAllTextAsync(Constants.LOCATIONSSTORE);

            var locationsList = new List<Station>();

            if (!string.IsNullOrEmpty(stationCache))
            {
                try
                {
                    var cache = JsonConvert.DeserializeObject<StorageCache>(stationCache);
                    if ((DateTime.Now - cache.CacheDate).Days < 8)
                    {
                        locationsList = cache.CacheData;
                        requestFromInternet = false;
                    }
                }
                catch (Exception)
                {
                    //Something went wrong with the cache! So get new data from the internet to try to force a new cache
                    requestFromInternet = true;
                }
            }

            if (!requestFromInternet) 
                return locationsList;
            try
            {
                locationsList = await Client.GetInstance().GetLocations(valuePair);
                var cache = new StorageCache { CacheDate = DateTime.Now, CacheData = locationsList };
                await ServiceLocator.Current.GetInstance<IAsyncStorageService>().WriteAllTextAsync(Constants.LOCATIONSSTORE, JsonConvert.SerializeObject(cache));
            }
            catch (Exception)
            {
                //TODO: Do something with exception
            }
            return locationsList;
        }

        public async Task<List<Departure>> GetLiveBoard(Station station)
        {
            if(!station.Id.Contains(":"))
            {
                return 
                    await 
                        Client.GetInstance()
                            .GetLiveBoard(new[]
                            {
                                new KeyValuePair<string, string>(Arguments.Id.ToString().ToLower(), station.Id), 
                                new KeyValuePair<string, string>(Arguments.Lang.ToString().ToLower(), AppResources.ClientLang)
                            } );
            }
            var time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(station.Id.Split(':')[0]), int.Parse(station.Id.Split(':')[1]), 0);
            return
                await
                    Client.GetInstance()
                        .GetLiveBoard(new[]
                        {
                            new KeyValuePair<string, string>(Arguments.Station.ToString(), station.Name.ToUpper()),
                            new KeyValuePair<string, string>(Arguments.Lang.ToString().ToLower(),AppResources.ClientLang),
                            new KeyValuePair<string, string>(Arguments.Time.ToString().ToLower(),time.ToString("o"))
                        });
        }

        public async Task<List<Stop>> GetVehicle(string vehicle)
        {
            return await Client.GetInstance().GetVehicle(new KeyValuePair<string, string>("id", "BE.NMBS." + vehicle));
        }
    }
}
