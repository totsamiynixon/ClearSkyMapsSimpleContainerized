using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;
using Web.Helpers.Interfaces;
using Web.Helpers.Implementations;
using Microsoft.Extensions.Logging;
using Web.Areas.Admin;
using Microsoft.Extensions.Hosting;
using Web.Application.Readings.Queries;
using Web.Areas.Admin.Application.Emulation.Queries;
using Web.Areas.PWA;
using Web.Domain.Entities.Identity;
using Web.Emulation;
using Web.Infrastructure;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;
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
            var appSettings = Configuration.GetSection("Settings").Get<AppSettings>();

            //TODO: Check how it works
            services.AddTransient<AppSettings>((_) => appSettings);
            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<Emulator>();
            services.AddTransient<IReadingsQueries, ReadingsQueries>();
            services.AddTransient<IEmulationQueries, EmulationQueries>();
            services.AddTransient<IPollutionCalculator, PollutionCalculator>();
            services.AddTransient<ISensorCacheHelper, SensorCacheHelper>();

            services.AddSingleton<IEmulationDataContextFactory<DataContext>>(provider => new DefaultDataContextFactory<DataContext>(appSettings.Emulation.ConnectionString));
            services.AddSingleton<IEmulationDataContextFactory<IdentityDataContext>>(provider => new DefaultDataContextFactory<IdentityDataContext>(appSettings.Emulation.ConnectionString));

            services.AddTransient<IDataContextFactory<DataContext>>(provider =>
            {
                var emulator = provider.GetService<Emulator>();
                return new DefaultDataContextFactory<DataContext>(emulator.IsEmulationStarted
                    ? appSettings.Emulation.ConnectionString
                    : appSettings.ConnectionString);
            });
            services.AddTransient<IDataContextFactory<IdentityDataContext>>(provider =>
            {
                var emulator = provider.GetService<Emulator>();
                return new DefaultDataContextFactory<IdentityDataContext>(emulator.IsEmulationStarted
                    ? appSettings.Emulation.ConnectionString
                    : appSettings.ConnectionString);
            });
            services.AddTransient<IDatabaseSeeder<DataContext>, DataContextDatabaseSeeder>();
            services.AddTransient<IDatabaseSeeder<IdentityDataContext>, IdentityDataContextDatabaseSeeder>();
            services.AddTransient<IApplicationDatabaseInitializer, DefaultApplicationDatabaseInitializer>();

            //TODO: Check why doesnt work with identity
            /*services.AddDbContext<DataContext>(
                (provider, builder) => provider.GetService<IDataContextFactory<DataContext>>().Create());
            services.AddDbContext<IdentityDataContext>(
                (provider, builder) => provider.GetService<IDataContextFactory<IdentityDataContext>>().Create());*/
                
            services.AddScoped<DataContext>(
                (provider) => provider.GetService<IDataContextFactory<DataContext>>().Create());
            services.AddScoped<IdentityDataContext>(
                (provider) => provider.GetService<IDataContextFactory<IdentityDataContext>>().Create());

            services.AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMemoryCache();

            services.AddSignalR();

            services.AddAppBundling(_environment);

            services.AddAdminAreaServices(Configuration);
            services.AddPWAAreaServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseAdminArea(Configuration, env);
            app.UsePWAArea(env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlerMiddleware>();
                app.UseMiddleware<ExceptionLoggerMiddleware>();
            }
            
            app.UseRouting();
            
            app.UseStaticFiles();
            app.UseCookiePolicy();

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