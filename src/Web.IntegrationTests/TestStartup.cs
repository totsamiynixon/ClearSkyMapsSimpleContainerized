using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Areas;
using Web.Infrastructure;
using Web.Infrastructure.Data.Initialize;
using Web.IntegrationTests.Infrastructure.Data.Initialize;

namespace Web.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment env, IEnumerable<IArea> areas,
            AppSettings appSettings) : base(configuration, env, areas, appSettings)
        {
        }

        protected override void SetupDatabaseInitializers(IServiceCollection services)
        {
            services.AddTransient<IApplicationDatabaseInitializer, TestApplicationDatabaseInitializer>();
            base.SetupDatabaseInitializers(services);
        }

        protected override IMvcBuilder SetupMVC(IServiceCollection services)
        {
            return base.SetupMVC(services)
                .AddApplicationPart(typeof(Startup).Assembly);
        }
    }
}