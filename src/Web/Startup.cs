using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Data.Models.Identity;
using Web.Extensions;
using Web.Helpers.Interfaces;
using Web.Helpers.Implementations;
using Microsoft.Extensions.Logging;
using Web.Middlewares;
using Web.Areas.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Helpers.Implementations;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Helpers.Implementations;
using Web.Areas.PWA;
using Web.Logging;
using System.IO;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static Func<IServiceProvider> ServiceProvider { get; private set; }

        private IServiceScopeFactory _scopeFactory { get; set; }

        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddTransient<UserManager<User>>();
            services.AddTransient<RoleManager<User>>();

            services.AddTransient<ISettingsProvider, JsonConfigSettingsProvider>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IPollutionCalculator, PollutionCalculator>();
            services.AddTransient<ISensorCacheHelper, SensorCacheHelper>();
            services.AddTransient<ISensorConnectionHelper, SensorWebSocketConnectionHelper>();
            services.AddTransient<IPWADispatchHelper, PWASignalrDispatchHelper>();
            services.AddTransient<IAdminDispatchHelper, AdminSignalRHubDispatchHelper>();
            services.AddTransient<IPWABootstrapper, PWAFileCompilerBootstrapper>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<DataContext>(options =>
            {

                var scopeFactory = services
                   .BuildServiceProvider()
                   .GetRequiredService<IServiceScopeFactory>();

                using (var scope = scopeFactory.CreateScope())
                {
                    {
                        options.UseSqlServer(
                            ((ISettingsProvider)scope.ServiceProvider.GetService(typeof(ISettingsProvider))).ConnectionString);
                    }
                }
            }, ServiceLifetime.Transient);
            services.AddDefaultIdentity<User>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Admin/Account/Login");
                });

            services.AddSignalR();

            services.AddAppBundling(_environment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            _scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            ServiceProvider = () =>
            {
                return _scopeFactory.CreateScope().ServiceProvider;
            };
            //ConfigureLogging(loggerFactory);
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=sensors}/{action=index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });
            app.UseAdminArea(env);
            app.UsePWAArea(env);
            app.InitializeDatabase();
            app.AppBundles();
        }


        private void ConfigureLogging(ILoggerFactory loggerFactory)
        {
            var errorPath = Path.Combine(Directory.GetCurrentDirectory(), "logs/errors.txt");
            var infoPath = Path.Combine(Directory.GetCurrentDirectory(), "logs/info.txt");
            loggerFactory.AddFile(errorPath, infoPath);
        }
    }
}
