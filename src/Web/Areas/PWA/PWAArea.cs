using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.PWA.Extensions;
using Web.Areas.PWA.Hubs;

namespace Web.Areas.PWA
{
    public static class PWAArea
    {
        public static IApplicationBuilder UsePWAArea(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.BootstrapPWA();
            app.UseSignalR(routes =>
            {
                routes.MapHub<PWAStaticSensorHub>("/pwahub");
            });

            return app;
        }
    }
}
