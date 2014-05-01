using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Track.Common
{
    public class FavoriteTypeVisibility:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Database.Type)
                return (((Database.Type) value).Equals(Database.Type.Station))
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
