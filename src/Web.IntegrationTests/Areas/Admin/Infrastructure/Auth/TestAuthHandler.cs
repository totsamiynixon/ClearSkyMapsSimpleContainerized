using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Web.IntegrationTests.Areas.Admin.Infrastructure.Auth
{
    public class TestAuthHandler : AuthenticationHandler<TestAuthSchemeOptions>
    {

        

        public TestAuthHandler(IOptionsMonitor<TestAuthSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            var _authenticationScheme = Options.AuthenticationScheme;
            var _user = Options.User;
            var _roles = Options.Roles;
            
            var claims = new[]
                {new Claim(ClaimTypes.Name, _user.UserName), new Claim(ClaimTypes.Role, string.Join(",", _roles))};
            var identity = new ClaimsIdentity(claims, _authenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, _authenticationScheme);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}