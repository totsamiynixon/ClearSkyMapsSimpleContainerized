using System.Collections.Generic;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;
using Web.Helpers.Interfaces;
using Web.Helpers.Implementations;
using Microsoft.Extensions.Hosting;
using Web.Application.Readings.Queries;
using Web.Areas;
using Web.Areas.Admin.Application.Emulation.Queries;
using Web.Areas.Admin.Emulation;
using Web.Areas.Admin.Infrastructure.Data;
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
        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _environment;
        protected readonly IEnumerable<IArea> _areas;
        protected readonly AppSettings _appSettings;

        public Startup(IConfiguration configuration, IWebHostEnvironment env, IEnumerable<IArea> areas,
            AppSettings appSettings)
        {
            _configuration = configuration;
            _environment = env;
            _areas = areas;
            _appSettings = appSettings;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));

            services.AddSingleton<Emulator>();
            services.AddTransient<IReadingsQueries, ReadingsQueries>();
            services.AddTransient<IEmulationQueries, EmulationQueries>();
            services.AddTransient<IPollutionCalculator, PollutionCalculator>();
            services.AddTransient<ISensorCacheHelper, SensorCacheHelper>();

            SetupDatabase(services, _appSettings);
            SetupDatabaseInitializers(services);
            SetupDataContext(services);

            SetupMVC(services);

            services.AddMemoryCache();

            services.AddSignalR();

            services.AddAppBundling(_environment);

            ConfigureAreaServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            ConfigureArea(app);

            if (_environment.IsDevelopment())
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

        protected virtual void SetupDatabase(IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IDataContextFactory<DataContext>>(provider =>
                new DefaultDataContextFactory<DataContext>(appSettings.ConnectionString));
            services.AddTransient<IDataContextFactory<IdentityDataContext>>(provider =>
                new DefaultDataContextFactory<IdentityDataContext>(appSettings.ConnectionString));
        }


        protected virtual void SetupDatabaseInitializers(IServiceCollection services)
        {
            services.AddTransient<IDatabaseSeeder<DataContext>, DataContextDatabaseSeeder>();
            services.AddTransient<IApplicationDatabaseInitializer, DefaultApplicationDatabaseInitializer>();
        }

        protected virtual void SetupDataContext(IServiceCollection services)
        {
            //TODO: Check why doesnt work with identity
            /*services.AddDbContext<DataContext>(
                (provider, builder) => provider.GetService<IDataContextFactory<DataContext>>().Create());*/
            
            services.AddScoped<DataContext>(
                (provider) => provider.GetService<IDataContextFactory<DataContext>>().Create());
        }

        protected virtual IMvcBuilder SetupMVC(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            return services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }


        protected virtual void ConfigureAreaServices(IServiceCollection services)
        {
            foreach (var area in _areas)
            {
                area.ConfigureServices(services);
            }
        }

        protected virtual void ConfigureArea(IApplicationBuilder app)
        {
            foreach (var area in _areas)
            {
                area.Configure(app);
            }
        }
    }
}