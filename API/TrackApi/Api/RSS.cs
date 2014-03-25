using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PortableRest;

namespace TrackApi.Api
{
    public class Rss
    {
        private RestClient _restClient;
        private const string _baseUrl = "http://www.railtime.be/website/RSS/";

        public String Disruptions
        {
            get { return "RssInfoBar_{0}.xml"; }
        }

        #region Constructor
        private static Rss s_Instance;
        private static object s_InstanceSync = new object();

        protected Rss()
        {
            _restClient = new RestClient
            {
                BaseUrl = _baseUrl
            };
        }

        public static Rss GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (s_Instance == null)
            {
                lock (s_InstanceSync)
                {
                    if (s_Instance == null)
                    {
                        s_Instance = new Rss();
                    }
                }
            }
            return s_Instance;
        }
        #endregion

        public async Task<string> GetDisruptions(string language)
        {
            string ro = string.Empty;
            try
            {
                var wc = new HttpClient {BaseAddress = new Uri(_baseUrl)};
                var message = await wc.GetAsync(string.Format(Disruptions, language));
                ro = await message.Content.ReadAsStringAsync();
                //TODO: add lovely to object here
            }
            catch (HttpRequestException re)
            {
                //TODO: show toast that indicates the call was not succesful
            }
            catch (Exception e)
            {
                //TODO: allow user to send an error log
            }
            return ro;
        }
    }
}
