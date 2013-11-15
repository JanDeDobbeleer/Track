using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Settings;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Track.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly AppSettings _settings = ((ISettings)Application.Current).Settings;
        
        public MainPage()
        {
            try
            {
                InitializeComponent();
                Pivot.Title = _settings.Title;
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

        private void Map_OnLoaded_OnLoaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Pushpin_Tap(object sender, GestureEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}