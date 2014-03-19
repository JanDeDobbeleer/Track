using System;
using System.Windows;
using System.Windows.Data;

namespace Track.Common
{
    public class BooleanToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean)
                return ((bool)value == true) ? Visibility.Visible : Visibility.Collapsed;

            throw new InvalidOperationException("Converter can only convert to value of type bool");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
