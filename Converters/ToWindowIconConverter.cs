using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToWindowIconConverter : IValueConverter
    {
        public static ToWindowIconConverter Instance { get; } = new ToWindowIconConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IBitmap bitmap)
            {
                return new WindowIcon(bitmap);
            }

            if (value is string fileName)
            {
                return new WindowIcon(fileName);
            }

            if (value is Stream stream)
            {
                return new WindowIcon(stream);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
