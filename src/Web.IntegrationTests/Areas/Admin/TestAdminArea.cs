using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Web.Areas.Admin;
using Web.Areas.Admin.Emulation;
using Web.Areas.Admin.Infrastructure;
using Web.Areas.Admin.Infrastructure.Auth.JWT;
using Web.Infrastructure;
using Web.IntegrationTests.Areas.Admin.Infrastructure;
using Web.IntegrationTests.Areas.Admin.Infrastructure.Auth;

namespace Web.IntegrationTests.Areas.Admin
{
    
    public class TestAdminArea : AdminArea
    {
        private readonly TestAdminAreaOptions _adminAreaOptions;
        
        
        public TestAdminArea(IConfiguration configuration, IWebHostEnvironment environment, IOptions<TestAdminAreaOptions> options, AppSettings appSettings, AdminAppSettings adminAppSettings, JWTAppSettings jwtAppSettings, EmulationAppSettings emulationAppSettings) : base(configuration, environment, appSettings, adminAppSettings, jwtAppSettings, emulationAppSettings)
        {
            _adminAreaOptions = options.Value;
        }

        protected override void ConfigureAuthentication(IServiceCollection services)
        {
            if (!_adminAreaOptions.Auth.UseCustomAuth)
            {
                base.ConfigureAuthentication(services);
                return;
            }

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<TestAuthSchemeOptions, TestAuthHandler>(
                    CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AuthenticationScheme =
                            CookieAuthenticationDefaults.AuthenticationScheme;
                        options.User = _adminAreaOptions.Auth.User;
                        options.Roles = _adminAreaOptions.Auth.Roles;
                    });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<TestAuthSchemeOptions, TestAuthHandler>(
                    JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.AuthenticationScheme =
                            JwtBearerDefaults.AuthenticationScheme;
                        options.User = _adminAreaOptions.Auth.User;
                        options.Roles = _adminAreaOptions.Auth.Roles;
                    });;
        }
    }
}