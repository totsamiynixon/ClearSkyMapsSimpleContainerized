using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Areas.PWA;
using Web.Domain.Entities;
using Web.Domain.Entities.Identity;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Initialize.Seed;
using Web.IntegrationTests.Areas.Admin;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Web.IntegrationTests.Areas.Admin.Infrastructure.Data.Initialize.Seed;
using Web.IntegrationTests.Infrastructure.Data.Initialize.Seed;

namespace Web.IntegrationTests
{
    public class BaseScenario
    {
        public class TestServerBuilder
        {
            //database
            private readonly List<IDatabaseSeeder<DataContext>> _databaseSeeders;
            private readonly List<IDatabaseSeeder<IdentityDataContext>> _identityDatabaseSeeders;

            //authentication
            private bool _useCustomAuth = false;
            private User User { get; set; }
            private List<string> Roles { get; set; }

            public TestServerBuilder()
            {
                _databaseSeeders = new List<IDatabaseSeeder<DataContext>>();
                _identityDatabaseSeeders = new List<IDatabaseSeeder<IdentityDataContext>>();
            }

            public TestServerBuilder UseSensors(params Sensor[] sensors)
            {
                _databaseSeeders.Add(new TestSensorsDatabaseSeeder(sensors));

                return this;
            }

            public TestServerBuilder UseUsers(params User[] users)
            {
                _identityDatabaseSeeders.Add(new TestIdentityDatabaseSeeder(users));

                return this;
            }

            public TestServerBuilder UseUsersWithRoles(params (User user, List<string> roles)[] usersWithRoles)
            {
                _identityDatabaseSeeders.Add(new TestIdentityDatabaseSeeder(usersWithRoles));

                return this;
            }

            public TestServerBuilder UseCustomAuth(User user, params string[] roles)
            {
                User = user;
                Roles = roles.ToList();
                _useCustomAuth = true;

                return this;
            }

            public TestServerBuilder UseDefaultAuth()
            {
                User = AdminAreaDefaults.DefaultUser;
                Roles = new List<string> {AuthSettings.Roles.Admin};
                _useCustomAuth = true;

                return this;
            }

            public TestServer Build()
            {
                var path = Assembly.GetAssembly(typeof(TestStartup))
                    ?.Location;

                var hostBuilder = Program.CreateWebHostBuilder(Array.Empty<string>())
                    .UseStartup<TestStartup>()
                    .ConfigureTestServices(services =>
                    {
                        if (_databaseSeeders.Any())
                        {
                            foreach (var seeder in _databaseSeeders)
                            {
                                services.AddTransient<IDatabaseSeeder<DataContext>>(x => seeder);
                            }
                        }

                        if (_identityDatabaseSeeders.Any())
                        {
                            foreach (var seeder in _identityDatabaseSeeders)
                            {
                                services.AddTransient<IDatabaseSeeder<IdentityDataContext>>(
                                    x => seeder);
                            }
                        }
                    })
                    .ConfigureServices(services =>
                    {
                        services.Configure<TestAdminAreaOptions>(x =>
                        {
                            x.Auth = new TestAdminAreaAuthOptions
                            {
                                UseCustomAuth = _useCustomAuth,
                                Roles = Roles,
                                User = User
                            };
                        });
                    })
                    .UseContentRoot(Path.GetDirectoryName(path));

                Program.ConfigureAdminArea<TestAdminArea>(hostBuilder);
                Program.ConfigurePWAArea<PWAArea>(hostBuilder);

                var testServer = new TestServer(hostBuilder);

                Program.InitializeApplication(testServer.Host);

                return testServer;
            }
        }
    }
}

/*public TestServerBuilder UseDatabaseSeeder(IDatabaseSeeder<DataContext> databaseSeeder)
{
    _databaseSeeders.Add(databaseSeeder);

    return this;
}

public TestServerBuilder UseIdentityDatabaseSeeder(
    IDatabaseSeeder<IdentityDataContext> identityDatabaseSeeder)
{
    _identityDatabaseSeeders.Add(identityDatabaseSeeder);

    return this;
}*/

//TODO: check how it works when docker fully implemented
/*.ConfigureAppConfiguration((context, configBuilder) =>
{
    var config = configBuilder.Build();
    configBuilder.AddInMemoryCollection(
        new Dictionary<string, string>
        {
            ["Settings:ConnectionString"] = config.GetSection("Settings").GetValue<string>("ConnectionString").Replace("{Id}", Guid.NewGuid().ToString())
        });
})*/
/*.ConfigureAppConfiguration((context, configBuilder) =>
{
    var testJsonConfigRootPath = Assembly.GetAssembly(typeof(TestStartup))
        ?.Location;
    
    //var testJsonFileProvider = new PhysicalFileProvider(testJsonConfigRootPath);
    //configBuilder.AddJsonFile(provider: testJsonFileProvider, path: "appsettings.json", optional: false, reloadOnChange: true);

    var testJsonStr = File.ReadAllText(Path.Combine(testJsonConfigRootPath, "appsettings.json"));
    configBuilder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(testJsonStr)));
})*/