using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Areas.Admin.Extensions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Users;

namespace Web.Areas.Admin.Controllers.Default
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
        Policy = AuthPolicies.Supervisor)]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public UsersController(IMediator mediator, IUserQueries userQueries, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(
                _mapper.Map<IEnumerable<UserListItemDTO>, List<UserListItemViewModel>>(
                    await _userQueries.GetUsersAsync()));
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
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
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (UserUnableToCreateException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }


            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> ChangePassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, "User id is required");
            }

            var user = await _userQueries.GetUserAsync(userId);
            if (user == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, $"User with id : {userId} not found");
            }

            var mappedUser = _mapper.Map<UserDetailsDTO, UserChangePasswordModel>(user);
            return View(new UserChangePasswordViewModel(mappedUser, user.Email));
        }


        [HttpPost]
        public async Task<ActionResult> ChangePassword(
            [Bind(Prefix = nameof(UserChangePasswordViewModel.Model))] [BindRequired]
            UserChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userQueries.GetUserAsync(model.Id);
                return View(new UserChangePasswordViewModel(model, user?.Email));
            }

            try
            {
                var command = _mapper.Map<UserChangePasswordModel, ChangeUserPasswordCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserNotFoundException ex)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return this.StatusCodeView(HttpStatusCode.BadRequest, "User id is required");
            }

            var user = await _userQueries.GetUserAsync(userId);
            if (user == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, $"User with id : {userId} not found");
            }

            if (user.Roles.Select(z => z.Name).Contains(AuthSettings.Roles.Supervisor))
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden, "Unable delete user with role: Supervisor");
            }

            var mappedUser = _mapper.Map<UserDetailsDTO, DeleteUserModel>(user);
            return View(new DeleteUserViewModel(mappedUser));
        }


        [HttpDelete]
        public async Task<ActionResult> Delete([Bind(Prefix = nameof(DeleteUserViewModel.Model))]
            DeleteUserModel model)
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
                return this.StatusCodeView(HttpStatusCode.NotFound, ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> ChangeActivation(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return this.StatusCodeView(HttpStatusCode.BadRequest, "User id is required");
            }

            var user = await _userQueries.GetUserAsync(userId);
            if (user == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, $"User with id : {userId} not found");
            }

            if (user.Roles.Select(z => z.Name).Contains(AuthSettings.Roles.Supervisor))
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden,
                    "Unable activate/deactivate user with role: Supervisor");
            }

            var mappedUser = _mapper.Map<UserDetailsDTO, ActivateUserModel>(user);
            return View(new ActivateUserViewModel(mappedUser));
        }


        [HttpPost]
        public async Task<ActionResult> ChangeActivation(
            [Bind(Prefix = nameof(ActivateUserViewModel.Model))] [BindRequired]
            ActivateUserModel model)
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
                return this.StatusCodeView(HttpStatusCode.NotFound, ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }

            return RedirectToAction("Index");
        }
    }
}