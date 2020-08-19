using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.MediatR.Commands;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            InitializeApplication(host);
            
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((cxt, config) =>
                {
                    var env = cxt.HostingEnvironment;
                    config
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddAzureWebAppDiagnostics();
                })
                .UseStartup<Startup>();

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