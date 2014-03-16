using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using Track.Common;
using Track.ViewModel;
using TrackApi.Classes;

namespace Track.Controls
{
    public partial class StationList : UserControl
    {
        public ObservableCollection<Station> Stations { get; private set; }

        public StationList()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (message) =>
            {
                if (message.Notification.Equals("StationsLoaded", StringComparison.OrdinalIgnoreCase))
                    Deployment.Current.Dispatcher.BeginInvoke(AssignList);
            });
        }

        private void AssignList()
        {
            Stations = ServiceLocator.Current.GetInstance<MainpageViewModel>().Locations;
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                LongListSelector.ItemsSource = null;
                var dataSource = AlphaKeyGroup<Station>.CreateGroups(Stations.ToList(),
                    System.Threading.Thread.CurrentThread.CurrentUICulture,
                    s => s.Name, true);
                LongListSelector.ItemsSource = dataSource;
                Tools.Tools.SetProgressIndicator(false);
            });
        }
    }
}
