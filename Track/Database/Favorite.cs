using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Track.Database
{
    public enum Type
    {
        Station,
        Vehicle
    }

    [Table]
    public class Favorite: INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region properties
        public const string NamePropertyName = "Name";
        private string _name;
        [Column]
        public string Name {
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

        public const string NavigateUriPropertyName = "NavigateUri";
        private string _navigateUri;
        [Column]
        public string NavigateUri
        {
            get { return _navigateUri; }
            set
            {
                NotifyPropertyChanging(NavigateUriPropertyName);
                _navigateUri = value;
                NotifyPropertyChanged(NavigateUriPropertyName);
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
