using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Areas;
using Web.Areas.Admin;
using Web.Areas.Admin.Emulation;
using Web.Areas.Admin.Infrastructure;
using Web.Areas.Admin.Infrastructure.Auth.JWT;
using Web.Areas.Admin.Infrastructure.Data.Factory;
using Web.Areas.PWA;
using Web.Areas.PWA.Infrastructure;
using Web.Infrastructure;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.MediatR.Commands;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateWebHostBuilder(args);

            ConfigureAdminArea<AdminArea>(hostBuilder);
            ConfigurePWAArea<PWAArea>(hostBuilder);

            var host = hostBuilder.Build();

            InitializeApplication(host);

            host.Run();
        }

        public static IWebHostBuilder
            CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    builder
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                    builder.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddAzureWebAppDiagnostics();
                })
                .ConfigureServices((ctx, services) =>
                {
                    var appSettings = ctx.Configuration.GetSection("Settings").Get<AppSettings>();
                    var adminAppSettings = ctx.Configuration.GetSection("Admin").Get<AdminAppSettings>();
                    var emulationSettings = ctx.Configuration.GetSection("Emulation").Get<EmulationAppSettings>();
                    var pwaAppSettings = ctx.Configuration.GetSection("PWA").Get<PWAAppSettings>();

                    services.AddTransient<AppSettings>((_) => appSettings);
                    services.AddTransient<AdminAppSettings>((_) => adminAppSettings);
                    services.AddTransient<JWTAppSettings>((_) => adminAppSettings.JWT);
                    services.AddTransient<EmulationAppSettings>((_) => emulationSettings);
                    services.AddTransient<PWAAppSettings>((_) => pwaAppSettings);

                    //Hack to make migrations work
                    DesignTimeDataContextFactory.ConnectionString = appSettings.ConnectionString;
                    DesignTimeIdentityDataContextFactory.ConnectionString = appSettings.ConnectionString;
                })
                .UseStartup<Startup>();


        public static void ConfigureArea<TArea>(IWebHostBuilder webHostBuilder) where TArea : IArea
        {
            webHostBuilder.ConfigureServices(services => { services.AddSingleton(typeof(IArea), typeof(TArea)); });
        }


        public static void ConfigureAdminArea<TArea>(IWebHostBuilder webHostBuilder) where TArea : AdminArea
        {
            ConfigureArea<TArea>(webHostBuilder);
        }

        public static void ConfigurePWAArea<TArea>(IWebHostBuilder webHostBuilder) where TArea : PWAArea
        {
            ConfigureArea<TArea>(webHostBuilder);
        }

        public static void InitializeApplication(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetService<IMediator>();
                mediator.Send(new InitApplicationCommand()).Wait();
            }
        }
    }
}