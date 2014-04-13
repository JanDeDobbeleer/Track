using System;
using System.Collections.Generic;
using System.Windows;
using Localization.Resources;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
using Nokia.Phone.HereLaunchers;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class Helper
    {
        public void OpenMaps(Station station)
        {
            try
            {
                var manufacturer = string.Empty;
                object temp;
                if (DeviceExtendedProperties.TryGetValue("DeviceManufacturer", out temp))
                    manufacturer = temp.ToString();
                if (manufacturer.Equals("NOKIA"))
                {
                    try
                    {
                        OpenHereMaps(station);
                    }
                    catch (Exception)
                    {
                        OpenDefaultmaps(station);
                    }
                }
                else
                {
                    OpenDefaultmaps(station);
                }
            }
            catch (Exception)
            {
                //TODO: handle this, guide the user to doznload a maps app?
            }
        }

        private void OpenDefaultmaps(Station station)
        {
            var bingMapsDirectionsTask = new BingMapsDirectionsTask();
            var mapLocation = new LabeledMapLocation(string.Format(AppResources.NavigationStation, station.Name), station.GeoCoordinate);
            bingMapsDirectionsTask.End = mapLocation;
            // If bingMapsDirectionsTask.Start is not set, the user's current location is used as the start point.
            bingMapsDirectionsTask.Show();
        }

        private void OpenHereMaps(Station station)
        {
            var routeTo = new DirectionsRouteDestinationTask
            {
                Destination = station.GeoCoordinate,
                Mode = RouteMode.Unknown
            };
            routeTo.Show();
        }

        public void AssignList(ICollection<Station> input, IEnumerable<Station> stations)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var station in stations)
                {
                    input.Add(station);
                }
            });
        }

        public void AssignList(ICollection<Departure> input, IEnumerable<Departure> departures)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var departure in departures)
                {
                    input.Add(departure);
                }
            });
        }

        public void AssignList(ICollection<Stop> input, IEnumerable<Stop> stops)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var stop in stops)
                {
                    input.Add(stop);
                }
            });
        }
    }
}