using System;
using System.Collections.Generic;
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
        #region eventHandling
        public event GetInfoFinishedEventHandler GetInfoFinished;

        protected void OnGetInfoFinished(GetInfoCompletedArgs args)
        {
            if (GetInfoFinished != null)
            {
                GetInfoFinished(this, args);
            }
        }
        #endregion

        public String Stations
        {
            get { return "stations/?format=json"; }
        }

        public String Connections
        {
            get { return "connections/?format=json"; }
        }

        public String LiveBoard
        {
            get { return "liveboard/?format=json"; }
        }

        public String Vehicle
        {
            get { return "vehicle/?format=json"; }
        }

        private RestClient _restClient;
        private string _baseUrl = "http://http://api.irail.be/";

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
            var request = new RestRequest
            {
                Resource = Stations + ConvertValuePairToQueryString(new[]{valuePair})
            };
            var ro = await _restClient.ExecuteAsync<StationRootObject>(request);
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