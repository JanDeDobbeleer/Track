using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Track.Common;
using Track.ViewModel;
using TrackApi.Classes;

namespace Track.Controls
{
    public partial class StationList : UserControl
    {
        public StationList()
        {
            InitializeComponent();
            ((ViewModelLocator)Application.Current.Resources["Locator"]).StationListModel.GetInfoFinished += StationListViewmodel_GetInfoFinished;
            ((ViewModelLocator)Application.Current.Resources["Locator"]).StationListModel.LoadStations();
        }

        void StationListViewmodel_GetInfoFinished(object sender, FinshedEventArgs args)
        {
            switch (args.InfoLocation)
            {
                case InfoLocation.Stations:
                    Deployment.Current.Dispatcher.BeginInvoke(delegate
                    {
                        LongListSelector.ItemsSource = null;
                        var dataSource = AlphaKeyGroup<Station>.CreateGroups(((ViewModelLocator)Application.Current.Resources["Locator"]).StationListModel.Stations.ToList(),
                            System.Threading.Thread.CurrentThread.CurrentUICulture,
                            s => s.Name, true);
                        LongListSelector.ItemsSource = dataSource;
                        Tools.Tools.SetProgressIndicator(false);
                    });
                    break;
            }
        }
    }
}
