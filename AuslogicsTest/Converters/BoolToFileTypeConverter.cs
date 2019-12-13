using System;
using System.Globalization;
using System.Windows.Data;

namespace AuslogicsTest.Converters
{
    class BoolToFileTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Registry";
            return "Start Menu";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
