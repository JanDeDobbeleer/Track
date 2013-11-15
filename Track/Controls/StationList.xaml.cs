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
                    LongListSelector.ItemsSource = null;
                    List<AlphaKeyGroup<Station>> DataSource = AlphaKeyGroup<Station>.CreateGroups(((ViewModelLocator)Application.Current.Resources["Locator"]).StationListModel.Stations.ToList(),
                            System.Threading.Thread.CurrentThread.CurrentUICulture,
                            (Station s) => { return s.Name; }, true);
                    LongListSelector.ItemsSource = DataSource;
                    Tools.Tools.SetProgressIndicator(false);
                    break;
            }
        }
    }
}
