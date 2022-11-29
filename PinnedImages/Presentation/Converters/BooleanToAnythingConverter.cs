using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class BooleanToAnythingConverter : IValueConverter
    {
        public object? TrueValue { get; set; }
        public object? FalseValue { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool isTrue)
            {
                return (isTrue ? TrueValue : FalseValue) ?? Binding.DoNothing;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                return value == TrueValue;
            }

            return Binding.DoNothing;
        }
    }
}
