using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Web.Areas.PWA.Hubs;
using Web.Areas.PWA.Infrastructure.MIddlewares;

namespace Web.Areas.PWA
{
    public static class PWAArea
    {
        public static IApplicationBuilder UsePWAArea(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("pwa_area", "PWA",
                    "pwa/{controller=home}/{action=index}/{id?}");
                endpoints.MapHub<PWAStaticSensorHub>("/pwahub");
            });
                
            app.AppBundles();
            
            return app;
        }
    }
}
