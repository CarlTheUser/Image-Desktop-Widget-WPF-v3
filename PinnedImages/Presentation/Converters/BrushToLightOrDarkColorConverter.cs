using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Presentation.Converters
{
    public class BrushToLightOrDarkBrushConverter : IValueConverter
    {
        private const int _threshold = 150;
        public Brush? BrushWhenValueIsDark { get; set; }
        public Brush? BrushWhenValueIsLight { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                // Use corrected weights for luminance
                double redWeight = 0.299 * solidColorBrush.Color.R,
                       greenWeight = 0.587 * solidColorBrush.Color.G,
                       blueWeight = 0.114 * solidColorBrush.Color.B;

                double luminance = redWeight + greenWeight + blueWeight;

                return (luminance < _threshold ? BrushWhenValueIsDark : BrushWhenValueIsLight) ?? Binding.DoNothing;
            }

            return BrushWhenValueIsLight ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
