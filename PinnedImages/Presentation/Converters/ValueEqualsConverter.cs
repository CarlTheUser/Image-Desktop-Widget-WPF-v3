using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class ValueEqualsConverter : IValueConverter
    {
        public object? Subject { get; set; }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return Subject == value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
