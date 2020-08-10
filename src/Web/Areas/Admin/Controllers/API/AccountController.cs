using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Models.API.Account;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Controllers.API
{
    [Area("Admin")]
    [Authorize]
    //TODO: check area based api routes
    [Route("api/{area}/{controller}")]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                //TODO: pass model state
                return BadRequest("Invalid data");
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                //ModelState.AddModelError("", "Ошибка при попытке входа");
                return BadRequest("Invalid data");
            }
            //TODO: fix authenticate implementation for Bearer Token
            await Authenticate(model.Email, string.Join(",", await _userManager.GetRolesAsync(user)));
            return Redirect(returnUrl ?? "admin");
        }

        private async Task Authenticate(string userName, string roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, roles)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}