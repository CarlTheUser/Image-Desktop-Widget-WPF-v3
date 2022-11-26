using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class ShadowToDropShadowEffectOpacityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Models.Shadow shadow)
            {
                return shadow.Visible ? shadow.Opacity: 0;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
