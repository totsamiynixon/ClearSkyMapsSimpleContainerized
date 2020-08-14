using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Application.Users.Commands;
using Web.Areas.Admin.Application.Users.Exceptions;
using Web.Areas.Admin.Application.Users.Queries;
using Web.Areas.Admin.Application.Users.Queries.DTO;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Users;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Supervisor)]
    //TODO: check area based api routes
    [Route( AdminArea.APIRoutePrefix + "/users")]
    [ApiController]
    public class APIUsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public APIUsersController(IMediator mediator, IUserQueries userQueries, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<UserListItemDTO>, List<UserListItemModel>>(
                await _userQueries.GetUsersAsync()));
        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
            }

            try
            {
                var command = _mapper.Map<CreateUserModel, CreateUserCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserEmailAddressIsAlreadyTakenException ex)
            {
                return Forbid(ex.Message);
            }

            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> ChangePassword(UserChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
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

            return Ok();
        }


        [HttpPost]
        public async Task<ActionResult> Delete(DeleteUserModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
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

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeActivation(ActivateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                //TODO: check how to return model state
                return BadRequest("Invalid Data");
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

            return Ok();
        }
    }
}