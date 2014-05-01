using System;
using System.Globalization;
using System.Windows.Data;

namespace Track.Common
{
    public class RowSpanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Database.Type)
                return (((Database.Type) value).Equals(Database.Type.Station))
                    ? 2
                    : 1;
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
