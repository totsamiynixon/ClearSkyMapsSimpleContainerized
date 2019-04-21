using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddAppBundling(this IServiceCollection services, IHostingEnvironment env)
        {
            services.AddBundling()
                    .UseDefaults(env) // see below
                    .UseNUglify(); // or .UseWebMarkupMin() - whichever minifier you prefer

        }
    }
}
