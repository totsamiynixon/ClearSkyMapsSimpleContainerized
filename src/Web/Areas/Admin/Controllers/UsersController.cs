using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Models.Users;
using Web.Domain.Entities.Identity;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Supervisor")]
    [Area("Admin")]
    public class UsersController : Controller
    {

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<User, UserListItemViewModel>();
            x.CreateMap<User, UserChangePasswordModel>();
            x.CreateMap<User, DeleteUserModel>();
            x.CreateMap<User, ActivateUserModel>();

            x.CreateMap<CreateUserModel, CreateUserCommand>()
                .ConstructUsing(z => new CreateUserCommand(z.Email, z.Password));
            x.CreateMap<UserChangePasswordModel, ChangeUserPasswordCommand>()
                .ConstructUsing(z => new ChangeUserPasswordCommand(z.Id, z.Email, z.Password, z.ConfirmPassword));
            x.CreateMap<DeleteUserModel, DeleteUserCommand>()
                .ConstructUsing(z => new DeleteUserCommand(z.Id));
            x.CreateMap<ActivateUserModel, ChangeUserActivationStateCommand>()
                .ConstructUsing(z => new ChangeUserActivationStateCommand(z.Id, z.IsActive));
        }));

        

        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;

        public UsersController(UserManager<User> userManager, IMediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
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
        public async Task<ActionResult> Create(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var command = _mapper.Map<CreateUserModel, CreateUserCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserEmailAddressIsAlreadyTakenException ex)
            {
                ModelState.AddModelError("EmailIsAlreadyTaken", ex.Message);
                return View(model);
            }
            
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
            var mappedUser = _mapper.Map<User, UserChangePasswordModel>(user);
            return View(new UserChangePasswordViewModel(mappedUser));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(nameof(UserChangePasswordViewModel.Model))]UserChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new UserChangePasswordViewModel(model));
            }

            try
            {
                var command = _mapper.Map<UserChangePasswordModel, ChangeUserPasswordCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return Forbid(ex.Message);
            }
            
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
            var mappedUser = _mapper.Map<User, DeleteUserModel>(user);
            return View(new DeleteUserViewModel(mappedUser));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind(nameof(DeleteUserViewModel.Model))]DeleteUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new DeleteUserViewModel(model));
            }
            
            try
            {
                var command = _mapper.Map<DeleteUserModel, DeleteUserCommand>(model);
                await _mediator.Send(command);
            }
            //TODO: Create Exception Filter for command exceptions
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return Forbid(ex.Message);
            }
            
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
            var mappedUser = _mapper.Map<User, ActivateUserModel>(user);
            return View(new ActivateUserViewModel(mappedUser));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeActivation([Bind(nameof(ActivateUserViewModel.Model))]ActivateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new ActivateUserViewModel(model));
            }
            
            try
            {
                var command = _mapper.Map<ActivateUserModel, ChangeUserActivationStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return Forbid(ex.Message);
            }
            
            return RedirectToAction("Index");
        }
    }
}