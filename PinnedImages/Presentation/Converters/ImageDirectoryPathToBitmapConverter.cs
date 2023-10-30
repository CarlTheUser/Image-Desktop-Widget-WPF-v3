using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Presentation.Converters
{
    public class ImageDirectoryPathToBitmapConverter : IValueConverter
    {
        public string FileName { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Shared.ImageDirectory imageDirectory && !string.IsNullOrWhiteSpace(imageDirectory))
            {
                var configuration = ((App)System.Windows.Application.Current).Configuration;

                //I'll need to do this if I want to delete the image
                //otherwise it'll throw an exception from an unreleased resource
                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(
                    uriString: Path.Combine(configuration["Application:Environment:Paths:PinnedImages"], imageDirectory, FileName),
                    uriKind: UriKind.RelativeOrAbsolute);
                bitmap.EndInit();

                return bitmap;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
