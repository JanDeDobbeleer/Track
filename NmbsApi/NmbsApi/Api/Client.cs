using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.Text;

namespace NmbsApi.Api
{
    public class Client
    {
        private const String Stations = "http://api.irail.be/stations/?lang=NL";
        private const String Connections = "http://http://api.irail.be/connections/";
        private const String LiveBoard = "http://http://api.irail.be/liveboard/";
        private const String Vehicle = "http://http://api.irail.be/vehicle/";

        public String GetInfo(String Location, KeyValuePair<String, String> valuePair)
        {
            if (NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                return false;
            var client = new WebClient();
            client.DownloadStringCompleted += ClientOnDownloadStringCompleted;
            client.DownloadStringAsync(new Uri("https://mobilevikings.com/api/active_users", uriKind: UriKind.RelativeOrAbsolute));
            return string.Empty;
        }
    }
}
