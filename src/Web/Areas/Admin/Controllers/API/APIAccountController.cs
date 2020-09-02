using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Produces("application/json")]
    [Consumes("application/json")]
    public class APIAccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JWTAppSettings _jwtAppSettings;

        public APIAccountController(UserManager<User> userManager, JWTAppSettings jwtAppSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtAppSettings = jwtAppSettings ?? throw new ArgumentNullException(nameof(jwtAppSettings));
        }

        /// <summary>
        /// Perform login
        /// </summary>
        /// <response code="200">If login was successfully performed. Returns AccessToken</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="400">If login failed</response>
        /// <response code="404">If user was not found</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                return NotFound("Login failed: user with that email doesn't exist");
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return BadRequest("Login failed: wrong password!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var tokenString = GenerateJWTToken(user, string.Join(",", roles));
            return Ok(new TokenModel
            {
                Token = tokenString,
                UserDetails = new UserDetailsModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                },
            });
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
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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