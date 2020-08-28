using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;
using Web.Helpers.Interfaces;
using Web.Helpers.Implementations;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Web.Application.Readings.Queries;
using Web.Areas;
using Web.Areas.Admin.Application.Emulation.Queries;
using Web.Areas.Admin.Emulation;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Infrastructure;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Middlewares;
using Web.Infrastructure.Swagger;

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
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("integration", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CSM Open API",
                    Description = "Clear Sky Maps REST API for Integration",
                    Contact = new OpenApiContact
                    {
                        Name = "Yauheni But-Husaim",
                        Email = "totsamiynixon@gmail.com",
                        Url = new Uri("https://vk.com/id169573384"),
                    }
                });
                

                ConfigureAreaSwagger(options);


                options.DocInclusionPredicate((version, desc) =>
                {
                    if (desc.GroupName == null && version == "integration")
                    {
                        return true;   
                    }

                    if (desc.GroupName == "Admin" && version == "admin")
                    {
                        return true;
                    }
                    
                    //TODO: Think more about that implementation
                    return ConfigureAreaSwaggerInclusionPredicates(version, desc);
                });
                
                options.OperationFilter<AuthorizeOperationFilter>();
                options.DocumentFilter<LowercasePathsDocumentFilter>();
                options.DocumentFilter<AlphabetSchemaDocumentFilter>();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            ConfigureArea(app);
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/integration/swagger.json", "CSM API | Integration | v1");
            });

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
            return services.AddMvc(c => { c.Conventions.Add(new ApiExplorerGroupPerAreaConvention()); })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }


        protected virtual void ConfigureAreaServices(IServiceCollection services)
        {
            foreach (var area in _areas)
            {
                area.ConfigureServices(services);
            }
        }
        
        protected virtual void ConfigureAreaSwagger(SwaggerGenOptions options)
        {
            foreach (var area in _areas.OfType<ISwaggerSupportArea>())
            {
                area.ConfigureSwagger(options);
            }
        }
        
        protected virtual bool ConfigureAreaSwaggerInclusionPredicates(string version, ApiDescription description)
        {
            foreach (var area in _areas.OfType<ISwaggerSupportArea>())
            {
                if (area.SwaggerInclusionPredicate(version, description))
                {
                    return true;
                }
            }

            return false;
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