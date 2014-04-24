using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using Localization.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using TrackApi.Api;
using TrackApi.Classes;
using TrackApi.Tools;
using Message = Tools.Message;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

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
            if (CheckForInternetAccess())
                return new List<Station>();
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
                        //TODO: check the size
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
            if (CheckForInternetAccess())
                return new List<Departure>();
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
            return
                await
                    Client.GetInstance()
                        .GetLiveBoard(new[]
                        {
                            new KeyValuePair<string, string>(Arguments.Station.ToString(), station.Name.ToUpper()),
                            new KeyValuePair<string, string>(Arguments.Lang.ToString().ToLower(),AppResources.ClientLang),
                            new KeyValuePair<string, string>(Arguments.Date.ToString().ToLower(),DateTime.Now.ToShortDateString().ConvertDateToCorrectDateStamp()),
                            new KeyValuePair<string, string>(Arguments.Time.ToString().ToLower(),station.Id.Remove(station.Id.IndexOf(':'), 1))
                        });
        }

        public async Task<List<Stop>> GetVehicle(string vehicle)
        {
            if (CheckForInternetAccess())
                return new List<Stop>();
            return await Client.GetInstance().GetVehicle(new KeyValuePair<string, string>("id", "BE.NMBS." + vehicle));
        }

        public void GetConnections(string date, string time, string from, string to)
        {
            if(CheckForInternetAccess())
                return;
            Client.GetInstance().GetConnections(new[]
            {
                new KeyValuePair<string, string>(Arguments.To.ToString().ToLower(), to),
                new KeyValuePair<string, string>(Arguments.From.ToString().ToLower(), from),
                new KeyValuePair<string, string>(Arguments.Date.ToString().ToLower(), date.AmericanDateToApiDate()),
                new KeyValuePair<string, string>(Arguments.Time.ToString().ToLower(), time.Remove(time.IndexOf(':'), 1)),
                new KeyValuePair<string, string>(Arguments.TimeSel.ToString().ToLower(), Arguments.Depart.ToString().ToLower()),
                new KeyValuePair<string, string>(Arguments.TypeOfTransport.ToString().ToLower(), Arguments.Train.ToString().ToLower())
            });
        }

        private bool CheckForInternetAccess()
        {
            if (NetworkInterface.GetIsNetworkAvailable()) 
                return true;
            Message.ShowToast(AppResources.ToastNoInternet);
            return false;
        }
    }
}
