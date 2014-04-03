using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Track.Common;
using TrackApi.Classes;

namespace Track.Controls
{
    public partial class StationList : UserControl
    {
        public ObservableCollection<Station> Stations { get; private set; }

        public StationList()
        {
            InitializeComponent();
            Messenger.Default.Register<List<Station>>(this, (list) =>
            {
                if(list.Count > 0)
                    AssignList(list);
            });
        }

        public void AssignList(IEnumerable<Station> stations)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                LongListSelector.ItemsSource = null;
                var dataSource = AlphaKeyGroup<Station>.CreateGroups(stations.ToList(),
                    System.Threading.Thread.CurrentThread.CurrentUICulture,
                    s => s.Name, true);
                LongListSelector.ItemsSource = dataSource;
            });
        }
    }
}
