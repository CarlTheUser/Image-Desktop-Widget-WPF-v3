using Application.Services;
using Data.Common.Contracts;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Misc.Utilities;
using Presentation.Pages;
using Presentation.View.Misc;
using Presentation.ViewModels;
using Serilog;
using Serilog.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using P = Data.Projections;
using S = Data.Common.Contracts.SpecificationRepositories;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddSerilog(new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: configuration["Application:Environment:Paths:ErrorLogs"],
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Day)
                    .CreateLogger(),
                    dispose: false);
            });

            services.AddTransient<IRandomStringGenerator, AbcRandomStringGenerator>();

            services.AddTransient<IAsyncQuery<IEnumerable<Color>, FileInfo>, ColorsFromJsonFileQuery>();
            //services.AddTransient<IAsyncQuery<IEnumerable<Color>, FileInfo>, StubColorsQuery>();

            services.AddTransient<S.IAsyncRepository<ImageId, Core.PinnedImage>>(
                s => new LiteDBPinnedImageRepository(
                    connectionString: configuration.GetConnectionString("PinnedImages")));

            services.AddTransient<IAsyncQuery<IEnumerable<P.PinnedImageListItem>>>(
                s => new LiteDbAllPinnedImageListItemsQuery(
                    connectionString: configuration.GetConnectionString("PinnedImages")));

            services.AddTransient<IUserPrompt<Stream?, OpenFileDialogPromptParameter>, OpenFileDialogPrompt>();

            services.AddTransient<IAsyncQuery<IEnumerable<P.PinnedImage>>>(
                s => new LiteDbAllShownPinnedImagesQuery(
                    connectionString: configuration.GetConnectionString("PinnedImages")));

            services.AddTransient<IPinImageService>(
                s => new PinImageService(
                    relativeFolderNameLength: configuration.GetValue<int>("Application:ImageKeeping:RelativeFolderNameLength"),
                    repository: s.GetRequiredService<S.IAsyncRepository<ImageId, Core.PinnedImage>>(),
                    thumbnailLength: configuration.GetValue<int>("Application:ImageKeeping:Thumbnail:Length"),
                    pinnedImagesDirectory: new DirectoryInfo(configuration["Application:Environment:Paths:PinnedImages"]),
                    randomStringGenerator: s.GetRequiredService<IRandomStringGenerator>()));

            services.AddTransient<IDeletePinnedImageService>(
                s => new DeletePinnedImageService(
                    repository: s.GetRequiredService<S.IAsyncRepository<ImageId, Core.PinnedImage>>(),
                    pinnedImagesDirectory: new DirectoryInfo(configuration["Application:Environment:Paths:PinnedImages"])));

            services.AddTransient<IRestylePinnedImageService, RestylePinnedImageService>();

            services.AddTransient<IChangePinnedImageDisplayParameterService, ChangePinnedImageDisplayParameterService>();

            services.AddTransient<IUnpinImageService, UnpinImageService>();

            services.AddTransient<IRepinImageService, RepinImageService>();

            services.AddSingleton<IUserNotification<Exception>, ErrorNotification>();

            services.AddSingleton<MainWindowViewLauncher>();

            services.AddSingleton<IPinnedImageViewLauncher, PinnedImageViewLauncher>();

            services.AddSingleton<IPinnedImageRestyleViewLauncher, PinnedImageRestyleViewLauncher>();

            services.AddSingleton<LibraryPageViewModel>();

            services.AddTransient<LibraryPage>();

            services.AddSingleton<SettingsPageViewModel>();

            services.AddTransient<SettingsPage>();
        }

        private const string _appName = "PinnedImages";

        public IServiceProvider ServiceProvider { get; }

        public IConfiguration Configuration { get; }

        private readonly ILogger<App> _logger;

        public App() : base()
        {
            _ = new Mutex(
                initiallyOwned: true,
                name: _appName,
                createdNew: out bool isNewInstance);

            if (!isNewInstance)
            {
                _ = MessageBox.Show("An instance of the application is already running.", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);

                Current.Shutdown();
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            ConfigureServices(services, configuration);

            Configuration = configuration;

            ServiceProvider = services.BuildServiceProvider();

            _logger = ServiceProvider.GetRequiredService<ILogger<App>>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = ServiceProvider;

            var errorNotification = services.GetRequiredService<IUserNotification<Exception>>();

            try
            {
                var query = services.GetRequiredService<IAsyncQuery<IEnumerable<P.PinnedImage>>>();

                var mainWindowViewLauncher = services.GetRequiredService<MainWindowViewLauncher>();

                var pinnedImageViewLauncher = services.GetRequiredService<IPinnedImageViewLauncher>();

                IEnumerable<P.PinnedImage> pinnedImages = await query.ExecuteAsync();

                if (pinnedImages.Any())
                {
                    foreach (var image in pinnedImages)
                    {
                        pinnedImageViewLauncher.Launch(parameter: image);
                    }
                }
                else
                {
                    mainWindowViewLauncher.Launch();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "A fatal error occurred.");

                errorNotification.Notify(ex);
            }
        }
    }
}
