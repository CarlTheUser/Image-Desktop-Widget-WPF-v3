using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{

    public class BooleanToBrushConverter : IValueConverter
    {
        public System.Windows.Media.Brush? BrushWhenTrue { get; set; }
        public System.Windows.Media.Brush? BrushWhenFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTrue)
            {
                return (isTrue ? BrushWhenTrue : BrushWhenFalse) ?? Binding.DoNothing;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
