using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Presentation.Converters
{
    //https://stackoverflow.com/questions/25426819/finding-out-if-a-hex-color-is-dark-or-light
    public class ColorToLightOrDarkColorConverter : IValueConverter
    {
        private const int _threshold = 150;
        public Color WhenValueIsDarkColor { get; set; }
        public Color WhenValueIsLightColor { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is not null && value is System.Drawing.Color color)
            {
                double redWeight = 0.2126 * color.R,
                    greenWeight = 0.7152 * color.G,
                    blueWeight = 0.7152 * color.B;

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
