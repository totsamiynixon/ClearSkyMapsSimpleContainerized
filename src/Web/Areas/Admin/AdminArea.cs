using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Emulation;
using Web.Areas.Admin.Helpers.Implementations;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Infrastructure;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Auth.JWT;
using Web.Areas.Admin.Infrastructure.Data;
using Web.Areas.Admin.Infrastructure.Data.Factory;
using Web.Areas.Admin.Infrastructure.Data.Initialize;
using Web.Areas.Admin.Infrastructure.Hubs;
using Web.Areas.Admin.Infrastructure.Middlewares;
using Web.Domain.Entities.Identity;
using Web.Infrastructure;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.Data.Initialize.Seed;
using Web.Infrastructure.Middlewares;

namespace Web.Areas.Admin
{
    public class AdminArea : IArea
    {
        public const string Name = "Admin";
        public const string DefaultRoutePrefix = "admin";
        public const string APIRoutePrefix = "api/admin";

        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _environment;
        protected readonly AppSettings _appSettings;
        protected readonly AdminAppSettings _adminAppSettings;
        protected readonly JWTAppSettings _jwtAppSettings;
        protected readonly EmulationAppSettings _emulationAppSettings;

        public AdminArea(IConfiguration configuration, IWebHostEnvironment environment, AppSettings appSettings,
            AdminAppSettings adminAppSettings, JWTAppSettings jwtAppSettings, EmulationAppSettings emulationAppSettings)
        {
            _configuration = configuration;
            _environment = environment;
            _appSettings = appSettings;
            _adminAppSettings = adminAppSettings;
            _jwtAppSettings = jwtAppSettings;
            _emulationAppSettings = emulationAppSettings;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IReadingsQueries, ReadingsQueries>();
            services.AddTransient<IUserQueries, UserQueries>();
            services.AddTransient<IAdminDispatchHelper, AdminSignalRHubDispatchHelper>();

            SetupDatabase(services, _appSettings, _emulationAppSettings);
            SetupDatabaseInitializers(services);
            SetupDataContext(services);

            services.AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<IdentityDataContext>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureAuthentication(services, _jwtAppSettings);

            services.AddAuthorization(config =>
            {
                config.AddPolicy(AuthPolicies.Admin, AuthPolicies.AdminPolicy());
                config.AddPolicy(AuthPolicies.Supervisor, AuthPolicies.SupervisorPolicy());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWhen(x => IsDefaultRequest(x.Request) || IsApiRequest(x.Request),
                builder =>
                {
                    builder.UseWhen(x => IsDefaultRequest(x.Request), applicationBuilder =>
                    {
                        if (_environment.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }
                        else
                        {
                            applicationBuilder.UseExceptionHandler($"/{DefaultRoutePrefix}/error/serverError");
                            app.UseMiddleware<ExceptionLoggerMiddleware>();
                        }
                    });

                    builder.UseWhen(x => IsApiRequest(x.Request), applicationBuilder =>
                    {
                        app.UseMiddleware<ExceptionHandlerMiddleware>();
                        app.UseMiddleware<ExceptionLoggerMiddleware>();
                    });


                    builder.UseRouting();

                    builder.UseAuthentication();
                    builder.UseAuthorization();

                    builder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapAreaControllerRoute("admin_area", Name,
                            $"{DefaultRoutePrefix}/{{controller=sensors}}/{{action=index}}/{{id?}}");
                        endpoints.MapHub<AdminPortableSensorHub>($"/{DefaultRoutePrefix}portable");
                        endpoints.MapHub<AdminStaticSensorHub>($"/{DefaultRoutePrefix}static");
                    });
                });

            app.AppBundles();
        }

        private static bool IsDefaultRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{DefaultRoutePrefix}");
        }

        private static bool IsApiRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{APIRoutePrefix}");
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services, JWTAppSettings jwtSettings)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "CSM.Admin.Auth";
                    options.LoginPath = new PathString($"/{AdminArea.DefaultRoutePrefix}/account/login");
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        protected virtual void SetupDatabase(IServiceCollection services, AppSettings appSettings,
            EmulationAppSettings emulationAppSettings)
        {
            services.AddSingleton<IEmulationDataContextFactory<DataContext>>(provider =>
                new EmulationDataContextFactory<DataContext>(emulationAppSettings.ConnectionString));
            services.AddSingleton<IEmulationDataContextFactory<IdentityDataContext>>(provider =>
                new EmulationDataContextFactory<IdentityDataContext>(emulationAppSettings.ConnectionString));

            services.AddTransient<IDataContextFactory<DataContext>>(provider =>
            {
                var emulator = provider.GetService<Emulator>();
                return new DefaultDataContextFactory<DataContext>(emulator.IsEmulationStarted
                    ? emulationAppSettings.ConnectionString
                    : appSettings.ConnectionString);
            });
            services.AddTransient<IDataContextFactory<IdentityDataContext>>(provider =>
            {
                var emulator = provider.GetService<Emulator>();
                return new DefaultDataContextFactory<IdentityDataContext>(emulator.IsEmulationStarted
                    ? emulationAppSettings.ConnectionString
                    : appSettings.ConnectionString);
            });
        }
        
        protected virtual void SetupDatabaseInitializers(IServiceCollection services)
        {
            services.AddTransient<IDatabaseSeeder<IdentityDataContext>, IdentityDataContextDatabaseSeeder>();
            services.AddTransient<IApplicationDatabaseInitializer, AdminAreaApplicationDatabaseInitializer>();
        }

        protected virtual void SetupDataContext(IServiceCollection services)
        {
            //TODO: Check why doesnt work with identity

            /*services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(_appSettings.ConnectionString));*/
            
            services.AddScoped<IdentityDataContext>(
                (provider) => provider.GetService<IDataContextFactory<IdentityDataContext>>().Create());
        }
    }
}