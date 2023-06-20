using Application.Services;
using CommunityToolkit.Mvvm.Messaging;
using Data.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;
using System;
using System.Windows.Media;

namespace Presentation
{
    public interface IPinnedImageViewLauncher : IViewLauncher<Data.Projections.PinnedImage>
    {

    }

    public class PinnedImageViewLauncher : IPinnedImageViewLauncher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PinnedImageViewLauncher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Launch(PinnedImage parameter)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var service = scope.ServiceProvider;

            parameter.Deconstruct(
                out Shared.ImageId imageId,
                out Shared.ImageDirectory imageDirectory,
                out Shared.Dimension dimension,
                out Shared.Location location,
                out Shared.ImageColor imageColor,
                out Shared.FrameThickness frameThickness,
                out Shared.Rotation rotation,
                out Shared.Corner corner,
                out Shared.Caption caption,
                out Shared.Shadow shadow,
                out bool isShown,
                out DateTime creationTimestamp);

            var window = new PinnedImageWindow(
                pinnedImage: new Models.PinnedImage(
                    id: imageId,
                    directory: imageDirectory,
                    dimension: new Models.Dimension(
                        width: dimension.Width,
                        height: dimension.Height),
                    location: new Models.Location(
                        x: location.X,
                        y: location.Y),
                    color: (Color)ColorConverter.ConvertFromString(value: imageColor),
                    frameThickness: frameThickness,
                    rotation: rotation,
                    corner: corner,
                    caption: new Models.Caption(
                        text: caption.Text,
                        visible: caption.IsVisible),
                    shadow: new Models.Shadow(
                        opacity: shadow.Opacity,
                        depth: shadow.Depth,
                        direction: shadow.Direction,
                        blurRadius: shadow.BlurRadius,
                        visible: shadow.IsVisible),
                    creationTimestamp: creationTimestamp),
                mainWindowViewLauncher: service.GetRequiredService<MainWindowViewLauncher>(),
                errorNotification: service.GetRequiredService<IUserNotification<Exception>>(),
                messageNotification: service.GetRequiredService<IUserNotification<Presentation.Message>>(),
                deletePinnedImageService: service.GetRequiredService<IDeletePinnedImageService>(),
                changePinnedImageDisplayParameterService: service.GetRequiredService<IChangePinnedImageDisplayParameterService>(),
                unpinImageService: service.GetRequiredService<IUnpinImageService>(),
                logger: service.GetRequiredService<ILogger<PinnedImageViewModel>>(),
                pinnedImageRestyleViewLauncher: service.GetRequiredService<IPinnedImageRestyleViewLauncher>(),
                messenger: service.GetRequiredService<IMessenger>());

            window.Show();
            window.Activate();
        }
    }
}
