using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using Localization.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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

        private void Map_OnLoaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Pushpin_Tap(object sender, GestureEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}