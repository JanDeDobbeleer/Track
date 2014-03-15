using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Localization.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.ServiceLocation;
using Track.ViewModel;
using TrackApi.Classes;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Track.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            try
            {
                InitializeComponent();
                Pivot.Title = AppResources.ApplicationTitle;
                Messenger.Default.Register<NotificationMessage>(this, (message) =>
                {
                    if (message.Notification.Equals("LocationsLoaded", StringComparison.OrdinalIgnoreCase))
                        Deployment.Current.Dispatcher.BeginInvoke(AdjustMapView);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            SystemTray.ProgressIndicator = new ProgressIndicator();
            Tools.Tools.SetProgressIndicator(true);
        }

        private void AdjustMapView()
        {   
            //Rectangle around the 4 most nearby
            var top4Locations = ServiceLocator.Current.GetInstance<MainpageViewModel>().Locations.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(4);

            //Add the current phone position to be sure it's also visible when changing the view zoom level
            var geoCoordinates = (from station in top4Locations select station.GeoCoordinate).ToList();
            //TODO: check for null?
            geoCoordinates.Add(ServiceLocator.Current.GetInstance<StationListViewmodel>().CurrentPosition);
            var locationRectangle = LocationRectangle.CreateBoundingRectangle(geoCoordinates);

            Map.SetView(locationRectangle);
        }

        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "0690e3e3-9f4b-4aa4-854a-759d938675ba";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "CLdqXlhvtu2QchQbHTNCdg";
        }

        private void Pushpin_Tap(object sender, GestureEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}