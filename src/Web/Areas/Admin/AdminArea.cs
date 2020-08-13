using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Helpers.Implementations;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Infrastructure.Hubs;
using Web.Areas.Admin.Infrastructure.Middlewares;
using Web.Infrastructure.Middlewares;

namespace Web.Areas.Admin
{
    public static class AdminArea
    {
        public const string Name = "Admin";
        public const string DefaultRoutePrefix = "admin";
        public const string APIRoutePrefix = "api/admin";

        public static IServiceCollection AddAdminAreaServices(this IServiceCollection services)
        {
            services.AddTransient<IReadingsQueries, ReadingsQueries>();
            services.AddTransient<IUserQueries, UserQueries>();
            services.AddTransient<IAdminDispatchHelper, AdminSignalRHubDispatchHelper>();

            return services;
        }

        public static IApplicationBuilder UseAdminArea(this IApplicationBuilder app, IConfiguration configuration,
            IWebHostEnvironment env)
        {
            app.UseWhen(x => IsDefaultRequest(x.Request) || IsApiRequest(x.Request),
                builder =>
                {
                    builder.UseWhen(x => IsDefaultRequest(x.Request), applicationBuilder =>
                    {
                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            applicationBuilder.UseExceptionHandler($"/{DefaultRoutePrefix}/Error/ServerError");
                            app.UseMiddleware<ExceptionLoggerMiddleware>();
                        }

                    });
                    
                    builder.UseWhen(x => IsApiRequest(x.Request), applicationBuilder =>
                    {
                        app.UseMiddleware<ExceptionHandlerMiddleware>();
                        app.UseMiddleware<ExceptionLoggerMiddleware>();
                    });
                    

                    builder.UseRouting();

                    builder.UseAuthentication();
                    builder.UseAuthorization();

                    builder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapAreaControllerRoute("admin_area", Name,
                            $"{DefaultRoutePrefix}/{{controller=sensors}}/{{action=index}}/{{id?}}");
                        endpoints.MapHub<AdminPortableSensorHub>($"/{DefaultRoutePrefix}portable");
                        endpoints.MapHub<AdminStaticSensorHub>($"/{DefaultRoutePrefix}static");
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