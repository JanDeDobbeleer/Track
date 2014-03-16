using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using TrackApi.Api;
using TrackApi.Classes;
using TrackApi.Tools;

namespace Track.Api
{
    public class RailService
    {
        #region Constructor
        private static RailService s_Instance;
        private static object s_InstanceSync = new object();

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
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new RailService();
                    }
                }
            }
            return s_Instance;
        }
        #endregion

        public async Task<List<Station>> GetLocations(KeyValuePair<String, String> valuePair)
        {
            bool requestFromInternet = true;
            string stationCache = string.Empty;
            //TODO: fix the crashing
            /*bool stationCacheExists = await ServiceLocator.Current.GetInstance<IAsyncStorageService>().FileExistsAsync(Constants.LOCATIONSSTORE);

            if(stationCacheExists)
                stationCache = await ServiceLocator.Current.GetInstance<IAsyncStorageService>().ReadAllTextAsync(Constants.LOCATIONSSTORE);*/

            var locationsList = new List<Station>();

            if (!string.IsNullOrEmpty(stationCache))
            {
                try
                {
                    StorageCache cache = JsonConvert.DeserializeObject<StorageCache>(stationCache);
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

            if (requestFromInternet)
            {
                try
                {
                    locationsList = await Client.GetInstance().GetLocations(valuePair);
                    StorageCache cache = new StorageCache() { CacheDate = DateTime.Now, CacheData = locationsList };
                    await ServiceLocator.Current.GetInstance<IAsyncStorageService>().WriteAllTextAsync(Constants.LOCATIONSSTORE, JsonConvert.SerializeObject(cache));
                }
                catch (Exception ex)
                {
                    //TODO: Do something with exception
                }
            }

            return locationsList;
        }
    }
}
