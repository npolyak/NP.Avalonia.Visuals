using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class BorderThicknessConverter : IValueConverter
    {
        public static BorderThicknessConverter Instance { get; } = new BorderThicknessConverter();

        public Thickness Convert(object value)
        {
            if (value is double d)
                return new Thickness(d);

            return new Thickness();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
