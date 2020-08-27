using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Web.Areas.Admin.Infrastructure.Auth.JWT;

namespace Web.Areas.Admin.Infrastructure.Auth
{
    public class AuthConfigurator
    {
        public virtual void Configure(IServiceCollection services, JWTAppSettings jwtSettings)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureAuthentication(services, jwtSettings);
            
            services.AddAuthorization(config =>
            {
                config.AddPolicy(AuthPolicies.Admin, AuthPolicies.AdminPolicy());
                config.AddPolicy(AuthPolicies.Supervisor, AuthPolicies.SupervisorPolicy());
            });
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
    }
}