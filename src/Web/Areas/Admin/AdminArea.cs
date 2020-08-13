using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Helpers.Implementations;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Auth.JWT;
using Web.Areas.Admin.Infrastructure.Hubs;
using Web.Areas.Admin.Infrastructure.Middlewares;
using Web.Infrastructure;
using Web.Infrastructure.Middlewares;

namespace Web.Areas.Admin
{
    public static class AdminArea
    {
        public const string Name = "Admin";
        public const string DefaultRoutePrefix = "admin";
        public const string APIRoutePrefix = "api/admin";

        public static IServiceCollection AddAdminAreaServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT").Get<JWTAppSettings>();
            
            services.AddTransient<JWTAppSettings>((_) => jwtSettings);
            
            services.AddTransient<IReadingsQueries, ReadingsQueries>();
            services.AddTransient<IUserQueries, UserQueries>();
            services.AddTransient<IAdminDispatchHelper, AdminSignalRHubDispatchHelper>();
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "CSM.Admin.Auth";
                    options.LoginPath = new PathString($"/{DefaultRoutePrefix}/account/login");
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
            
            services.AddAuthorization(config =>
            {
                config.AddPolicy(AuthPolicies.Admin, AuthPolicies.AdminPolicy());
                config.AddPolicy(AuthPolicies.Supervisor, AuthPolicies.SupervisorPolicy());
            });

            
            return services;
        }

        public static IApplicationBuilder UseAdminArea(this IApplicationBuilder app, IConfiguration configuration,
            IWebHostEnvironment env)
        {
            app.UseWhen(x => IsDefaultRequest(x.Request) || IsApiRequest(x.Request),
                builder =>
                {
                    builder.UseWhen(x => IsDefaultRequest(x.Request), applicationBuilder =>
                    {
                        if (env.IsDevelopment())
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

            return app;
        }

        private static bool IsDefaultRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{DefaultRoutePrefix}");
        }

        private static bool IsApiRequest(HttpRequest request)
        {
            return request.Path.Value.StartsWith($"/{APIRoutePrefix}");
        }
    }
}