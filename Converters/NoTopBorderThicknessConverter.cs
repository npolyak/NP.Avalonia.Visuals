using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class NoTopBorderThicknessConverter : IValueConverter
    {
        public static NoTopBorderThicknessConverter Instance { get; } = new NoTopBorderThicknessConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness borderThickness)
            {
                return new Thickness(borderThickness.Left, 0, borderThickness.Right, borderThickness.Bottom);
            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
