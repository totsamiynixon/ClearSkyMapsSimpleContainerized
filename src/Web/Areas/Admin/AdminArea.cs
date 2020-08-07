using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Web.Areas.Admin.Infrastructure.Hubs;
using Web.Areas.Admin.Infrastructure.MIddlewares;

namespace Web.Areas.Admin
{
    public static class AdminArea
    {
        public static IApplicationBuilder UseAdminArea(this IApplicationBuilder app, IConfiguration configuration,  IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("adminArea", "Admin",  "{area:exists}/{controller=sensors}/{action=index}/{id?}");
                endpoints.MapHub<AdminPortableSensorHub>("/adminportable");
                endpoints.MapHub<AdminStaticSensorHub>("/adminstatic");
            });
            
            app.AppBundles();

            return app;
        }
    }
}
