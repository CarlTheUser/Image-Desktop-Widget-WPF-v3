using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class ExceptionToTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Exception exception)
            {
                return exception.GetType().FullName ?? exception.GetType().Name;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
