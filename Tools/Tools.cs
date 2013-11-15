using System;
using System.IO.IsolatedStorage;
using System.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;

namespace Tools
{
    public struct KeyValuePair
    {
        public object Key;
        public string Value;
    }

    public static class Tools
    {
        public static void SetProgressIndicator(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        public static ApplicationBarIconButton CreateButton(string uri, string text, bool enabled, EventHandler handler)
        {
            var button = new ApplicationBarIconButton { IconUri = new Uri(uri, UriKind.Relative), Text = text, IsEnabled = enabled };
            button.Click += handler;
            return button;
        }

        public static ApplicationBarMenuItem CreateMenuItem(string text, bool enabled, EventHandler handler)
        {
            var appBarMenuItem = new ApplicationBarMenuItem { Text = text, IsEnabled = enabled };
            appBarMenuItem.Click += handler;
            return appBarMenuItem;
        }

        public static bool HasInternetConnection()
        {
            return (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None);
        }

        public static void DefaultAllSettings()
        {
            //TODO: add appsettings here
        }
    }
}