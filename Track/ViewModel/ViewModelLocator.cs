/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Track"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Track.Database;

namespace Track.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static readonly Uri StationOverviewPageUri = new Uri("/View/StationOverview.xaml", UriKind.Relative);
        public static readonly Uri VehicleOverviewPageUri = new Uri("/View/VehicleOverview.xaml", UriKind.Relative);
        public static readonly Uri SearchPageUri = new Uri("/View/Search.xaml", UriKind.Relative);
        public static readonly Uri HomePageUri = new Uri("/View/MainPage.xaml", UriKind.Relative);
        public static readonly Uri AboutUri = new Uri("/Track.About;component/About.xaml", UriKind.Relative);
        public static readonly Uri ConnectionUri = new Uri("/View/Connection.xaml", UriKind.Relative);
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<MainpageViewModel>();
            SimpleIoc.Default.Register<StationOverviewViewModel>();
            SimpleIoc.Default.Register<VehicleOverviewViewModel>();
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<TrackDatabase>();
            SimpleIoc.Default.Register<ConnectionViewModel>();
        }

        public MainpageViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainpageViewModel>(); }
        }

        public StationOverviewViewModel StationOverview
        {
            get { return ServiceLocator.Current.GetInstance<StationOverviewViewModel>(); }
        }

        public VehicleOverviewViewModel VehicleOverView
        {
            get { return ServiceLocator.Current.GetInstance<VehicleOverviewViewModel>(); }
        }

        public SearchViewModel Search
        {
            get { return ServiceLocator.Current.GetInstance<SearchViewModel>(); }
        }

        public ConnectionViewModel Connection
        {
            get { return ServiceLocator.Current.GetInstance<ConnectionViewModel>(); }
        }

        public TrackDatabase Database
        {
            get { return ServiceLocator.Current.GetInstance<TrackDatabase>(); }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}