using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Areas.PWA.Helpers.Implementations;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Hubs;
using Web.Areas.PWA.Infrastructure.MIddlewares;
using Web.Infrastructure.Middlewares;

namespace Web.Areas.PWA
{
    public static class PWAArea
    {
        public const string Name = "PWA";
        public const string DefaultRoutePrefix = "pwa";
        public const string APIRoutePrefix = "api/pwa";

        public static IServiceCollection AddPWAAreaServices(this IServiceCollection services)
        {
            services.AddTransient<IPWADispatchHelper, PWASignalrDispatchHelper>();
            return services;
        }

        public static IApplicationBuilder UsePWAArea(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWhen(x => IsDefaultRequest(x.Request)|| IsApiRequest(x.Request),
                builder =>
                {
                    if (env.IsDevelopment())
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

            return app;
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