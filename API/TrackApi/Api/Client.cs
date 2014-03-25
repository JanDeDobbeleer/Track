using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PortableRest;
using TrackApi.Classes;
using TrackApi.Tools;

namespace TrackApi.Api
{
    public delegate void GetInfoFinishedEventHandler(object sender, GetInfoCompletedArgs args);

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
            get { return "/liveboard/?format=json"; }
        }

        public String Vehicle
        {
            get { return "/vehicle/?format=json"; }
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

        public async Task<List<Station>> GetLocations(KeyValuePair<String, String> valuePair)
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
            catch (HttpRequestException re)
            {
                //TODO: show toast that indicates the call was not succesful
            }
            catch (Exception e)
            {
                //TODO: allow user to send an error log
            }
            return ro.Station;
        }

        private string ConvertValuePairToQueryString(IEnumerable<KeyValuePair<string, string>> valuePair)
        {
            var builder = new StringBuilder();
            foreach (var vp in valuePair)
            {
                builder.Append("&");
                builder.Append(vp.Key);
                builder.Append("=");
                builder.Append(vp.Value);
            }
            return builder.ToString();
        }
    }
}