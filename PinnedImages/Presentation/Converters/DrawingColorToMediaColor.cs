using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class DrawingColorToMediaColor : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
            {
                return System.Windows.Media.Color.FromArgb(
                    a: color.A,
                    r: color.R,
                    g: color.G,
                    b: color.B);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
