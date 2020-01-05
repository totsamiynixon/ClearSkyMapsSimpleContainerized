using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Web.Areas.PWA.Hubs;

namespace Web.Areas.PWA
{
    public static class PWAArea
    {
        public static IApplicationBuilder UsePWAArea(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSignalR(routes =>
            {
                routes.MapHub<PWAStaticSensorHub>("/pwahub");
            });

            return app;
        }
    }
}
