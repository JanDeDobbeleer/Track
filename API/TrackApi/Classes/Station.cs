using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Device.Location;
using Localization.Resources;
using Newtonsoft.Json;
using TrackApi.Tools;

namespace TrackApi.Classes
{
    [Table]
    public class Station: INotifyPropertyChanged, INotifyPropertyChanging
    {
        public const string IdPropertyName = "Id";
        private string _id;
        [Column(IsPrimaryKey = true, IsDbGenerated = false)]
        [JsonProperty(PropertyName = "id")]
        public string Id 
        {
            get
            {
                return _id;
            }
            set
            {
                NotifyPropertyChanging(IdPropertyName);
                _id = value;
                NotifyPropertyChanged(IdPropertyName);
            } 
        }

        public const string LocationXPropertyName = "LocationX";
        private string _locationX;
        [Column]
        [JsonProperty(PropertyName = "locationX")]
        public string LocationX
        {
            get
            {
                return _locationX;
            }
            set
            {
                NotifyPropertyChanging(LocationXPropertyName);
                _locationX = value;
                NotifyPropertyChanged(LocationXPropertyName);
            }
        }

        public const string LocationYPropertyName = "LocationY";
        private string _locationY;
        [Column]
        [JsonProperty(PropertyName = "locationY")]
        public string LocationY
        {
            get
            {
                return _locationY;
            }
            set
            {
                NotifyPropertyChanging(LocationYPropertyName);
                _locationY = value;
                NotifyPropertyChanged(LocationYPropertyName);
            }
        }

        public const string NamePropertyName = "Name";
        private string _name;
        [Column]
        [JsonProperty(PropertyName = "name")]
        [JsonConverter(typeof(StationNameConverter))]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                NotifyPropertyChanging(NamePropertyName);
                _name = value;
                NotifyPropertyChanged(NamePropertyName);
            }
        }

        public const string StandardNamePropertyName = "Standardname";
        private string _standardName;
        [Column]
        [JsonProperty(PropertyName = "standardname")]
        public string StandardName
        {
            get
            {
                return _standardName;
            }
            set
            {
                NotifyPropertyChanging(StandardNamePropertyName);
                _standardName = value;
                NotifyPropertyChanged(StandardNamePropertyName);
            }
        }
        
        //create a geocordinate based on the X and Y
        private GeoCoordinate _geoCoordinate;
        public GeoCoordinate GeoCoordinate
        {
            get
            {
                if (ReferenceEquals(_geoCoordinate, null))
                {
                    _geoCoordinate = new GeoCoordinate(double.Parse(LocationY.Replace('.', ',')), double.Parse(LocationX.Replace('.', ',')));
                }
                return _geoCoordinate;
            }
        }

        public double DistanceToCurrentPhonePosition { get; set; }

        public string Distance
        {
            get
            {
                if (DistanceToCurrentPhonePosition > 2)
                    return Math.Round(DistanceToCurrentPhonePosition, 2) + " " +  AppResources.Kilometers;
                if (DistanceToCurrentPhonePosition > 1)
                    return Math.Round(DistanceToCurrentPhonePosition, 2) + " " + AppResources.Kilometer;
                return Math.Round((DistanceToCurrentPhonePosition * 1000), 0) + " " + AppResources.Meters;
            }
        }

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
    }
}
