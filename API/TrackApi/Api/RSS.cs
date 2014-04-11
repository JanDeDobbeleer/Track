using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using TrackApi.Classes;

namespace TrackApi.Api
{
    public class Rss
    {
        private const string BaseUrl = "http://www.railtime.be/website/RSS/";

        public String Disruptions
        {
            get { return "RssInfoBar_{0}.xml"; }
        }

        #region Constructor
        private static Rss _sInstance;
        private static readonly object SInstanceSync = new object();

        protected Rss()
        {
            
        }

        public static Rss GetInstance()
        {
            // This implementation of the singleton design pattern prevents 
            // unnecessary locks (using the double if-test)
            if (_sInstance != null) 
                return _sInstance;
            lock (SInstanceSync)
            {
                if (_sInstance == null)
                {
                    _sInstance = new Rss();
                }
            }
            return _sInstance;
        }
        #endregion

        public async Task<DisruptionRootObject> GetDisruptions(string language)
        {
            var ro = new DisruptionRootObject();
            try
            {
                string temp;
                using (var wc = new HttpClient { BaseAddress = new Uri(BaseUrl) })
                {
                    var message = await wc.GetAsync(string.Format(Disruptions, language));
                    temp = await message.Content.ReadAsStringAsync();
                }
                var xml = XDocument.Parse(temp, LoadOptions.None);
                foreach (var item in xml.Descendants("item"))
                {
                    var d = new Disruption();
                    d.Decription = item.Element("description").Value.Trim();
                    d.Timestamp = DateTime.Parse(item.Element("pubDate").Value);
                    d.Title = item.Element("title").Value.Trim();
                    d.Link = item.Element("link").Value.Trim();
                }
                //TODO: remove this after debugging
                /*if (ro.Disruptions.Count == 0)
                    ro.Disruptions = TestData();*/
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

        private List<Disruption> TestData()
        {
            var list = new List<Disruption>();
            var d1 = new Disruption
            {
                Decription =
                    "11:40 (26/03/2014) Er is een wissel defect ter hoogte van station Nossegem. Hierdoor duurt de reistijd 5 tot 10 minuten langer.   "
                        .Trim(),
                Timestamp = DateTime.Parse("Wed, 26 Mar 2014 10:45:00 GMT"),
                Link = "http://www.railtime.be/website/Pages/InfobarList.aspx?l=NL".Trim(),
                Title = "Lijn 36 : Brussel-Zuid - Luik-Guillemins Vertraagd verkeer. "
            };
            var d2 = new Disruption
            {
                Decription =
                    "12:10 (26/03/2014) Er is een storing aan de seininrichting tussen station Edingen en station Aat. Verstoord verkeer.   "
                        .Trim(),
                Timestamp = DateTime.Parse("Wed, 26 Mar 2014 11:13:00 GMT"),
                Link = "http://www.railtime.be/website/Pages/InfobarList.aspx?l=NL".Trim(),
                Title = "Lijn 94 : Brussel-Zuid - Doornik Verstoord verkeer. "
            };
            list.Add(d1);
            list.Add(d2);
            return list;
        }

    }
}
