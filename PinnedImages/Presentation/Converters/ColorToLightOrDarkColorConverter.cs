using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Presentation.Converters
{
    public class ColorToLightOrDarkColorConverter : IValueConverter
    {
        private const int _threshold = 150;
        public Color WhenValueIsDarkColor { get; set; }
        public Color WhenValueIsLightColor { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
            {
                // Use corrected weights for luminance
                double redWeight = 0.299 * color.R,
                       greenWeight = 0.587 * color.G,
                       blueWeight = 0.114 * color.B;

                double luminance = redWeight + greenWeight + blueWeight;

                return luminance < _threshold ? WhenValueIsDarkColor : WhenValueIsLightColor;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
