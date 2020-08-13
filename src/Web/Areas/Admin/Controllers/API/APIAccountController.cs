using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Infrastructure.Auth.JWT;
using Web.Areas.Admin.Models.API.Account;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflicts
    [Route(AdminArea.APIRoutePrefix + "/account")]
    [ApiController]
    public class APIAccountController : Controller
    {
        private UserManager<User> _userManager;
        private JWTAppSettings _jwtAppSettings;

        public APIAccountController(UserManager<User> userManager, JWTAppSettings jwtAppSettings)
        {
            _userManager = userManager;
            _jwtAppSettings = jwtAppSettings;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var tokenString = GenerateJWTToken(user, string.Join(",", await _userManager.GetRolesAsync(user)));
                response = Ok(new
                {
                    token = tokenString,
                    userDetails = new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        email = user.Email
                    },
                });
            }

            return response;
        }

        private string GenerateJWTToken(User user, string roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAppSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roles),
                //TODO: investigate that
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(
                issuer: _jwtAppSettings.Issuer,
                audience:
                _jwtAppSettings.Audience,
                claims:
                claims,
                expires:
                DateTime.Now.AddMinutes(60),
                signingCredentials:
                credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}