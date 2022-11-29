using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;
using System;

namespace Presentation
{
    public interface IPinnedImageRestyleViewLauncher : IViewLauncher<Models.PinnedImage>
    {

    }

    public class PinnedImageRestyleViewLauncher : IPinnedImageRestyleViewLauncher
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PinnedImageRestyleViewLauncher(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Launch(Models.PinnedImage parameter)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            var service = scope.ServiceProvider;

            var window = new PinnedImageRestyleWindow(
                pinnedImage: parameter,
                logger: service.GetRequiredService<ILogger<PinnedImageRestyleViewModel>>(),
                restylePinnedImageService: service.GetRequiredService<IRestylePinnedImageService>(),
                errorNotification: service.GetRequiredService<IUserNotification<Exception>>());

            window.ShowDialog();
        }
    }
}
