using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Areas.Admin.Models.Users;
using Web.Data.Models.Identity;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [Area("Admin")]
    public class UsersController : Controller
    {


        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<User, UserListItemViewModel>();
            x.CreateMap<User, UserChangePasswordViewModel>();
            x.CreateMap<User, DeleteUserViewModel>();
            x.CreateMap<User, ActivateUserViewModel>();
        }));


        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var mappedUsers = _mapper.Map<List<User>, List<UserListItemViewModel>>(users);
            return View(mappedUsers);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Email == model.Email);
            if (user != null)
            {
                ModelState.AddModelError("EmailIsBusy", "Данный адрес электронной почты уже занят другим пользователем!");
                return View(model);
            }
            var newUser = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Admin");
            }
            //ShowAlert(Enums.AlertTypes.Success, "Пользователь был успешно добавлен!");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> ChangePassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Необходим id пользователя");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не найден");
            }
            var mappedUser = _mapper.Map<User, UserChangePasswordViewModel>(user);
            return View(mappedUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(UserChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == model.Id);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не найден");
            }
            if (await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return Forbid("Невозможно сменить пароль пользователю с ролью Supervisor");
            }
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.Password);
            await _userManager.UpdateAsync(user);
            //ShowAlert(Enums.AlertTypes.Success, "Обновление пароля прошло успешно!");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Необходим id пользователя");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не найден");
            }
            if (await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return Forbid("Невозможно удалить пользователя с ролью Supervisor");
            }
            var mappedUser = _mapper.Map<User, DeleteUserViewModel>(user);
            return View(mappedUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(DeleteUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Delete", new { userId = model.Id });
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == model.Id);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не найден");
            }
            if (await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return Forbid("Невозможно удалить пользователя с ролью Supervisor");
            }
            await _userManager.DeleteAsync(user);
            // ShowAlert(Enums.AlertTypes.Success, "Удаление пользователя прошло успешно!");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> ChangeActivation(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Необходим id пользователя");
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return NotFound("Пользователь с таким id не найден");
            }
            if (await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return Forbid("Невозможно активировать/деактивировать пользователя с ролью Supervisor");
            }
            var mappedUser = _mapper.Map<User, ActivateUserViewModel>(user);
            return View(mappedUser);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeActivation(ActivateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.Id == model.Id);
            if (user == null)
            {
                return BadRequest("Пользователь с таким id не найден");
            }
            if (await _userManager.IsInRoleAsync(user, "Supervisor"))
            {
                return Forbid("Невозможно активировать/деактивировать пользователя с ролью Supervisor");
            }
            user.IsActive = model.IsActive;
            await _userManager.UpdateAsync(user);
            //ShowAlert(Enums.AlertTypes.Success, $"{(user.IsActive ? "Активация" : "Деактивация")} пользователя прошло успешно!");
            return RedirectToAction("Index");
        }
    }
}