using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Practices.ServiceLocation;
using Track.ViewModel;
using TrackApi.Classes;

namespace Track.Database
{
    public enum Type
    {
        Station,
        Vehicle
    }

    [Table]
    public class Favorite : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region properties
        public const string IdPropertyName = "Id";
        private int _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                NotifyPropertyChanging(IdPropertyName);
                _id = value;
                NotifyPropertyChanged(IdPropertyName);
            }
        }

        public const string NamePropertyName = "Name";
        private string _name;
        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging(NamePropertyName);
                _name = value;
                NotifyPropertyChanged(NamePropertyName);
            }
        }

        public const string DetailPropertyName = "Detail";
        private string _detail;
        [Column]
        public string Detail
        {
            get { return _detail; }
            set
            {
                NotifyPropertyChanging(DetailPropertyName);
                _detail = value;
                NotifyPropertyChanged(DetailPropertyName);
            }
        }

        public const string QueryElementPropertyName = "QueryElement";
        private string _queryElement;
        [Column]
        public string QueryElement
        {
            get { return _queryElement; }
            set
            {
                NotifyPropertyChanging(QueryElementPropertyName);
                _queryElement = value;
                NotifyPropertyChanged(QueryElementPropertyName);
            }
        }

        public const string TypePropertyName = "Type";
        private Type _type;
        [Column]
        public Type Type
        {
            get { return _type; }
            set
            {
                NotifyPropertyChanging(TypePropertyName);
                _type = value;
                NotifyPropertyChanged(TypePropertyName);
            }
        }
        #endregion

        public void Navigate()
        {
            switch (Type)
            {
                case Type.Station:
                    var station = new Station {Name = Name, Id = QueryElement, TimeStamp = DateTime.Now};
                    Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<StationOverviewViewModel>().Station = station);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(ViewModelLocator.StationOverviewPageUri);
                    break;
                case Type.Vehicle:
                    Deployment.Current.Dispatcher.BeginInvoke(() => ServiceLocator.Current.GetInstance<VehicleOverviewViewModel>().Vehicle = QueryElement);
                    ServiceLocator.Current.GetInstance<INavigationService>().NavigateTo(ViewModelLocator.VehicleOverviewPageUri);
                    break;
            }
        }

        #region notify
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        #endregion
    }
}
