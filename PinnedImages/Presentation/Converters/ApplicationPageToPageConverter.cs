using Microsoft.Extensions.DependencyInjection;
using Presentation.Pages;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class ApplicationPageToPageConverter : IValueConverter
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationPageToPageConverter()
        {
            _serviceProvider = ((Presentation.App)System.Windows.Application.Current).ServiceProvider;
        }

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ApplicationPage?) switch
            {
                ApplicationPage.MainPage => _serviceProvider.GetService<LibraryPage>() ?? Binding.DoNothing,
                ApplicationPage.SettingsPage => _serviceProvider.GetService<SettingsPage>() ?? Binding.DoNothing,
                _ => Binding.DoNothing
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
