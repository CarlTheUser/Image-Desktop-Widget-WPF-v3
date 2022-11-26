using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class MaximizedOrNormalWindowStateToGlyphConverter : IValueConverter
    {
        public string? MaximizedValue { get; set; }
        public string? NormalValue { get; set; }

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is WindowState windowState)
            {
                return windowState == WindowState.Normal ? MaximizedValue : NormalValue;
            }
            return Binding.DoNothing;
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
