using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.ServiceLocation;
using Track.Common;
using Track.ViewModel;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Track.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                if (message.Notification.Equals("StationsLoaded", StringComparison.OrdinalIgnoreCase))
                    Deployment.Current.Dispatcher.BeginInvoke(AdjustMapView);
            });
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("MainPageLoaded"));
            Loaded -= OnPageLoaded;
        }

        private void AdjustMapView()
        {
            //Rectangle around the 3 most nearby
            var nearbyLocations = ServiceLocator.Current.GetInstance<MainpageViewModel>().Nearby;
            //calculate offset for displaying the 3 loctions properly
            //Add the current phone position to be sure it's also visible when changing the view zoom level
            var geoCoordinates = (from station in nearbyLocations select station.GeoCoordinate.OffsetCoordinate(1000)).ToList();
            //TODO: check for null?
            geoCoordinates.Add(ServiceLocator.Current.GetInstance<MainpageViewModel>().CurrentPosition);
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