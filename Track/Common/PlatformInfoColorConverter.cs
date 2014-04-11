using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TrackApi.Classes;

namespace Track.Common
{
    public class PlatformInfoColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var platformInfo = value as PlatformInfo;
            if (platformInfo != null)
            {
                return (platformInfo.name.Equals(platformInfo.normal)) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
