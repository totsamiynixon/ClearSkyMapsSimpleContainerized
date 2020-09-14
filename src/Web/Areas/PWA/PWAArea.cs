using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Areas.PWA.Helpers.Implementations;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Hubs;
using Web.Areas.PWA.Infrastructure.MIddlewares;
using Web.Infrastructure.Middlewares;

namespace Web.Areas.PWA
{
    public class PWAArea : IArea
    {
        public const string Name = "PWA";
        public const string DefaultRoutePrefix = "pwa";
        public const string APIRoutePrefix = "api/pwa";


        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public PWAArea(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPWADispatchHelper, PWASignalrDispatchHelper>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWhen(x => IsDefaultRequest(x.Request) || IsApiRequest(x.Request),
                builder =>
                {
                    if (_env.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseMiddleware<ExceptionHandlerMiddleware>();
                        app.UseMiddleware<ExceptionLoggerMiddleware>();
                    }

                    builder.UseRouting();

                    builder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapAreaControllerRoute("pwa_area", Name,
                            $"{DefaultRoutePrefix}/{{controller=home}}/{{action=index}}/{{id?}}");
                        endpoints.MapHub<PWAStaticSensorHub>($"/{DefaultRoutePrefix}hub");
                    });
                });

            app.AppBundles();
        }

        private static bool IsDefaultRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{DefaultRoutePrefix}");
        }

        private static bool IsApiRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{APIRoutePrefix}");
        }
    }
}