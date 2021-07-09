using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToGridLengthConverter : IValueConverter
    {
        public static ToGridLengthConverter Instance { get; } =
            new ToGridLengthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return new GridLength(d, GridUnitType.Pixel);
            }

            return new GridLength();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
