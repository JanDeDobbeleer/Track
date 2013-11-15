using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Tools;
using Track.Common;
using TrackApi.Api;
using TrackApi.Classes;

namespace Track.ViewModel
{
    public class StationListViewmodel:ViewModelBase
    {
        public ObservableCollection<Station> Stations { get; private set; }

        #region eventhandling
        public event InfoFinishedEventHandler GetInfoFinished;

        protected void OnGetInfoFinished(FinshedEventArgs args)
        {
            if (GetInfoFinished != null)
            {
                GetInfoFinished(this, args);
            }
        }
        #endregion

        public void LoadStations()
        {
            SystemTray.ProgressIndicator.Text = "Loading stations";
            var client = new Client();
            client.GetInfoFinished += client_GetInfoFinished;
            client.GetInfo(client.Stations, new[] { new KeyValuePair() { Key = client.Lang, Value = Language.EN.ToString() } });
        }

        private void client_GetInfoFinished(object sender, GetInfoCompletedArgs args)
        {
            try
            {
                var xml = XDocument.Parse(args.Data);
                var query = from b in xml.Descendants("station")
                           select new Station()
                           {
                               Id = (String)b.Attribute("id"),
                               LocationX = (String)b.Attribute("locationX"),
                               LocationY = (String)b.Attribute("locationY"),
                               Name = (String)b.Attribute("standardname")
                           };
                Stations = new ObservableCollection<Station>(query);
                Messenger.Default.Send(new NotificationMessage("LocationsLoaded"));
#if(DEBUG)
                Debug.WriteLine("Number of stations = " + Stations.Count());
                Debug.WriteLine("Station: " + Stations.ElementAt(0).Name);
                Debug.WriteLine("LocationX: " + Stations.ElementAt(0).LocationX);
                Debug.WriteLine("LocationY: " + Stations.ElementAt(0).LocationY);
                Debug.WriteLine("Id: " + Stations.ElementAt(0).Id);
#endif
                SetNearbyLocations();
                
                OnGetInfoFinished(new FinshedEventArgs() { InfoLocation = InfoLocation.Stations, Success = true });
            }
            catch (Exception)
            {
                //TODO: handle this properly
            }
        }

        private void SetNearbyLocations()
        {
            
        }
    }
}
