using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.PWA.Helpers.Interfaces;

namespace Web.Areas.PWA.Extensions
{
    public static class PWABootstrapperMiddleware
    {
        private static readonly object Lock = new object();
        private static bool _dbInitialized;
        public static IApplicationBuilder BootstrapPWA(this IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var bootstrapper = serviceScope.ServiceProvider.GetService<IPWABootstrapper>();
                bootstrapper.InitializePWA();
            }
            return builder;
        }
    }
}
