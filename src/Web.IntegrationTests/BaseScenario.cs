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

            public TestServerBuilder UseDatabaseSeeder(IDatabaseSeeder<DataContext> databaseSeeder)
            {
                _databaseSeeders.Add(databaseSeeder);

                return this;
            }

            public TestServerBuilder UseIdentityDatabaseSeeder(
                IDatabaseSeeder<IdentityDataContext> identityDatabaseSeeder)
            {
                _identityDatabaseSeeders.Add(identityDatabaseSeeder);

                return this;
            }

            public TestServerBuilder UseSensors(params Sensor[] sensors)
            {
                _databaseSeeders.Add(new TestSensorsDatabaseSeeder(sensors));

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
                Roles = new List<string> {AuthSettings.Roles.Supervisor};
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