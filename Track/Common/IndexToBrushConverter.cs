using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Track.Common
{
    public class IndexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Int32)
                return ((int)value == 1) ? new SolidColorBrush(Colors.White) : Application.Current.Resources["PhoneInactiveBrush"];

            throw new InvalidOperationException("Converter can only convert to value of type int");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
