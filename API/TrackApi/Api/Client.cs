using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PortableRest;
using TrackApi.Classes;

namespace TrackApi.Api
{

    public class Client
    {
        public String Stations
        {
            get { return "/stations/?format=json"; }
        }

        public String Connections
        {
            get { return "/connections/?format=json"; }
        }

        public String LiveBoard
        {
            get { return "/liveboard/?format=json&fast=true"; } 
        }

        public String Vehicle
        {
            get { return "/vehicle/?format=json&fast=true"; }
        }

        private RestClient _restClient;
        private const string _baseUrl = "http://api.irail.be";

        #region Constructor
        private static Client s_Instance;
        private static object s_InstanceSync = new object();

        protected Client()
        {
            _restClient = new RestClient
            {
                BaseUrl = _baseUrl
            };
        }

        public static Client GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new Client();
                    }
                }
            }
            return s_Instance;
        }
        #endregion

        public async Task<List<Station>> GetLocations(KeyValuePair<string, string> valuePair)
        {
            var ro = new StationRootObject();
            try
            {
                var request = new RestRequest
                {
                    Resource = Stations + ConvertValuePairToQueryString(new[] {valuePair})
                };
                ro = await _restClient.ExecuteAsync<StationRootObject>(request);
            }
            catch (HttpRequestException)
            {
                //TODO: show toast that indicates the call was not succesful
            }
            catch (Exception)
            {
                //TODO: allow user to send an error log
            }
            return ro.Station;
        }

        public async Task<List<Departure>> GetLiveBoard(KeyValuePair<string, string>[] valuePair)
        {
            var ro = new LiveBoardRootObject();
            try
            {
                var request = new RestRequest
                {
                    Resource = LiveBoard + ConvertValuePairToQueryString(valuePair)
                };
                ro = await _restClient.ExecuteAsync<LiveBoardRootObject>(request);
            }
            catch (HttpRequestException)
            {
                //TODO: show toast that indicates the call was not succesful
            }
            catch (Exception)
            {
                //TODO: allow user to send an error log
            }
            return ro.departures.departure;
        }

        public async Task<List<Stop>> GetVehicle(KeyValuePair<string, string> valuePair)
        {
            var ro = new VehicleRootObject();
            try
            {
                var request = new RestRequest
                {
                    Resource = Vehicle + ConvertValuePairToQueryString(new[] { valuePair })
                };
                ro = await _restClient.ExecuteAsync<VehicleRootObject>(request);
            }
            catch (HttpRequestException)
            {
                //TODO: show toast that indicates the call was not succesful
            }
            catch (Exception)
            {
                //TODO: allow user to send an error log
            }
            return ro.Stops.Stop;
        }

        private string ConvertValuePairToQueryString(IEnumerable<KeyValuePair<string, string>> valuePair)
        {
            var builder = new StringBuilder();
            foreach (var vp in valuePair)
            {
                builder.Append("&");
                builder.Append(vp.Key.ToLower());
                builder.Append("=");
                builder.Append(vp.Value);
            }
            return builder.ToString();
        }

        /*
         * var vehicleName = "BE.NMBS." + vehicle;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.irail.be/", UriKind.RelativeOrAbsolute);
                var temp = await client.GetStringAsync("vehicle/?format=json&fast=true" + "&id=" + vehicleName);
                var test = temp + "test";
            }
         */
    }
}