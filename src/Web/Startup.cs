using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;
using Web.Helpers.Interfaces;
using Web.Helpers.Implementations;
using Microsoft.Extensions.Logging;
using Web.Areas.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Helpers.Implementations;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Helpers.Implementations;
using Web.Areas.PWA;
using Web.Domain.Entities.Identity;
using Web.Emulation;
using Web.Infrastructure;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;
using Web.Infrastructure.Data.Repository;
using Web.Infrastructure.Middlewares;

namespace Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.GetSection("Settings").Get<AppSettings>();

            //TODO: Check how it works
            services.AddTransient<AppSettings>((_) => config);
            services.AddMediatR(typeof(Startup));
            
            services.AddTransient<UserManager<User>>();
            services.AddTransient<RoleManager<User>>();

            services.AddSingleton<Emulator>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IPollutionCalculator, PollutionCalculator>();
            services.AddTransient<ISensorCacheHelper, SensorCacheHelper>();
            services.AddTransient<IPWADispatchHelper, PWASignalrDispatchHelper>();
            services.AddTransient<IAdminDispatchHelper, AdminSignalRHubDispatchHelper>();

            services.AddSingleton<IEmulationDataContextFactory<DataContext>, EmulationDataContextFactory>();
            services.AddSingleton<IEmulationDataContextFactory<IdentityDataContext>, EmulationIdentityDataContextFactory>();

            services.AddSingleton<IDataContextFactory<DataContext>>(provider =>
            {
                var appSettings = provider.GetService<AppSettings>();
                var emulator = provider.GetService<Emulator>();
                return emulator.IsEmulationEnabled
                    ? (IDataContextFactory<DataContext>)new EmulationDataContextFactory(appSettings)
                    : (IDataContextFactory<DataContext>)new DefaultDataContextFactory(appSettings);
            });
            services.AddSingleton<IDataContextFactory<IdentityDataContext>>(provider =>
            {
                var appSettings = provider.GetService<AppSettings>();
                var emulator = provider.GetService<Emulator>();
                return emulator.IsEmulationEnabled
                    ? (IDataContextFactory<IdentityDataContext>)new EmulationIdentityDataContextFactory(appSettings)
                    : (IDataContextFactory<IdentityDataContext>)new DefaultIdentityDataContextFactory(appSettings);
            });
            services.AddSingleton<IDatabaseSeeder<DataContext>, DataContextDatabaseSeeder>();
            services.AddSingleton<IDatabaseSeeder<IdentityDataContext>, IdentityDataContextDatabaseSeeder>();
            services.AddSingleton<IApplicationDatabaseInitializer, DefaultApplicationDatabaseInitializer>();

            //TODO: Check why doesnt work with identity
            /*services.AddDbContext<DataContext>(
                (provider, builder) => provider.GetService<IDataContextFactory<DataContext>>().Create());
            services.AddDbContext<IdentityDataContext>(
                (provider, builder) => provider.GetService<IDataContextFactory<IdentityDataContext>>().Create());*/
                
            services.AddScoped<DataContext>(
                (provider) => provider.GetService<IDataContextFactory<DataContext>>().Create());
            services.AddScoped<IdentityDataContext>(
                (provider) => provider.GetService<IDataContextFactory<IdentityDataContext>>().Create());

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "CSM.Auth";
                    options.LoginPath = new PathString("/Admin/Account/Login");
                });

            services.AddSignalR();

            services.AddAppBundling(_environment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseRouting();

            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseAdminArea(Configuration, env);
            app.UsePWAArea(env);
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=pwa}/{controller=home}/{action=index}/{id?}");
            });
            
        }
    }
}