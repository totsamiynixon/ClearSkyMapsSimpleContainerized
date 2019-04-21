using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.Admin.Hubs;

namespace Web.Areas.Admin
{
    public static class AdminArea
    {
        public static IApplicationBuilder UseAdminArea(this IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.Map("/admin", builder =>
            //{
            //    app.UseMvc(routes =>
            //    {
            //        routes.MapRoute(
            //          name: "Admin_default",
            //          template: "{area}/{controller=Sensors}/{action=Index}");
            //    });
            //});

            app.UseSignalR(routes =>
            {
                routes.MapHub<AdminPortableSensorHub>("/adminportable");
                routes.MapHub<AdminStaticSensorHub>("/adminstatic");
            });

            return app;
        }
    }
}
