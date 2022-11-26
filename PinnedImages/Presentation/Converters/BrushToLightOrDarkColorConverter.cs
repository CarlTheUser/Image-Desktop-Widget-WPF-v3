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
            if (value is not null && value is Brush brush)
            {
                if(brush is SolidColorBrush solidColorBrush)
                {
                    double redWeight = 0.2126 * solidColorBrush.Color.R,
                        greenWeight = 0.7152 * solidColorBrush.Color.G,
                        blueWeight = 0.7152 * solidColorBrush.Color.B;

                    double luminance = redWeight + greenWeight + blueWeight;

                    return (luminance < _threshold ? BrushWhenValueIsDark : BrushWhenValueIsLight) ?? Binding.DoNothing;
                }

                return BrushWhenValueIsLight ?? Binding.DoNothing;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
