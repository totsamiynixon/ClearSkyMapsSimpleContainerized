using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Infrastructure;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Tests.Functional.Integration.Infrastructure.Data;
using Web.Tests.Functional.Integration.Infrastructure.Data.Initialize;

namespace Web.Tests.Functional.Integration
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }


        protected override void SetupDatabase(IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient(typeof(IDataContextFactory<>), typeof(TestDataContextFactory<>));
        }

        protected override void SetupDatabaseInitializers(IServiceCollection services)
        {
            base.SetupDatabaseInitializers(services);
            /*services.Remove(ServiceDescriptor
                .Transient<IApplicationDatabaseInitializer, DefaultApplicationDatabaseInitializer>());*/
            //services.AddTransient<IDatabaseSeeder<DataContext>, TestDataContextDatabaseSeeder>();
            services.AddTransient<IApplicationDatabaseInitializer, TestApplicationDatabaseInitializer>();
        }

        protected override IMvcBuilder SetupMVC(IServiceCollection services)
        {
           return base.SetupMVC(services)
                .AddApplicationPart(typeof(Startup).Assembly);
        }
    }
}