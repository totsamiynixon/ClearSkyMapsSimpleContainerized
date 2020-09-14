using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddAppBundling(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddBundling()
                    .UseDefaults(env) // see below
                    .UseNUglify(); // or .UseWebMarkupMin() - whichever minifier you prefer

        }
    }
}
