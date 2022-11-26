using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class CornerToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Shared.Corner corner)
            {
                return new CornerRadius(uniformRadius: corner.Radius);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is CornerRadius v)
            {
                return new Shared.Corner(Radius: v.TopRight);
            }
            return Binding.DoNothing;
        }
    }
    public class CornerToDoubleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Shared.Corner corner)
            {
                return corner.Radius;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is double v)
            {
                return new Shared.Corner(Radius: v);
            }
            return Binding.DoNothing;
        }
    }
}
