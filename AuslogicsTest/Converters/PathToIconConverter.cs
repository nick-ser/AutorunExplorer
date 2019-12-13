using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace AuslogicsTest.Converters
{
    class PathToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var filePath = value as string;
            ImageSource icon = null;
            if (File.Exists(filePath))
            {
                using (var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(filePath))
                {
                    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              sysicon.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
            }
            else
            {
                using (var iconStream = App.GetResourceStream(new Uri("pack://application:,,,/Resources/Unknown.ico")).Stream)
                {
                    using (var drawingIcon = new System.Drawing.Icon(iconStream))
                    {
                        icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            drawingIcon.Handle,
                            System.Windows.Int32Rect.Empty,
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
