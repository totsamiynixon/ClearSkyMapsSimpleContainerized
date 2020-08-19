using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Initialize.Seed;

namespace Web.Tests.Functional.Integration
{
    public class BaseScenario
    {
        public TestServer CreateServer(IDatabaseSeeder<DataContext> _databaseSeeder = null, IDatabaseSeeder<IdentityDataContext> _identityDatabaseSeeder = null)
        {
            var path = Assembly.GetAssembly(typeof(TestStartup))
                ?.Location;

            var hostBuilder = Program.CreateWebHostBuilder(Array.Empty<string>())
                .UseStartup<TestStartup>()
                .ConfigureServices(services =>
                {
                    if (_databaseSeeder != null)
                    {
                        services.AddTransient<IDatabaseSeeder<DataContext>>(x => _databaseSeeder);
                    }

                    if (_identityDatabaseSeeder != null)
                    {
                        services.AddTransient<IDatabaseSeeder<IdentityDataContext>>(x => _identityDatabaseSeeder);
                    }
                })
                .UseContentRoot(Path.GetDirectoryName(path));

            var testServer = new TestServer(hostBuilder);

            Program.InitializeApplication(testServer.Host);

            return testServer;
        }
    }
}