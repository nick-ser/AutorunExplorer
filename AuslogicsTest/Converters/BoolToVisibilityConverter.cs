using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AuslogicsTest.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public bool ValueForVisibility { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == ValueForVisibility)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
