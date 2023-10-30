using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class RotationToDoubleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Shared.Rotation rotation)
            {
                return rotation.Angle;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is double v)
            {
                return new Shared.Rotation(Angle: v);
            }
            return Binding.DoNothing;
        }
    }
}
