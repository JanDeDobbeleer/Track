using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Practices.ServiceLocation;
using Track.ViewModel;

namespace Track.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                if (message.Notification.Equals("StationsLoaded", StringComparison.OrdinalIgnoreCase))
                    Deployment.Current.Dispatcher.BeginInvoke(AdjustMapView);
            });
        }

        private void AdjustMapView()
        {
            //Rectangle around the 6 most nearby
            var nearbyLocations = ServiceLocator.Current.GetInstance<MainpageViewModel>().Locations.OrderBy(item => item.DistanceToCurrentPhonePosition).Take(5).ToList();
            var geoCoordinates = (from station in nearbyLocations select station.GeoCoordinate).ToList();
            //Add the current phone position to be sure it's also visible when changing the view zoom level
            geoCoordinates.Add(ServiceLocator.Current.GetInstance<MainpageViewModel>().CurrentPosition);
            var locationRectangle = LocationRectangle.CreateBoundingRectangle(geoCoordinates);
            Map.SetView(locationRectangle);
        }

        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "0690e3e3-9f4b-4aa4-854a-759d938675ba";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "CLdqXlhvtu2QchQbHTNCdg";
        }
    }
}