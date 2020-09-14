using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Produces("application/json")]
    [Consumes("application/json")]
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

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserListItemModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(_mapper.Map<IEnumerable<UserListItemDTO>, List<UserListItemModel>>(
                await _userQueries.GetUsersAsync()));
        }
        
        
        /// <summary>
        /// Create user
        /// </summary>
        /// <response code="200">If user successfully created</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="409">If user state can't be changed</response>
        /// <response code="409">If failed to create user</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Create(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                var command = _mapper.Map<CreateUserModel, CreateUserCommand>(model);
                await _mediator.Send(command);
            }
            catch (UserEmailAddressIsAlreadyTakenException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserUnableToCreateException ex)
            {
                return Conflict(ex.Message);
            }

            return Ok();
        }

        //TODO: Important! fix the way how it works, now everybody can change everybodies password
        /// <summary>
        /// Change user password
        /// </summary>
        /// <response code="200">If password was successfully updated</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If user not found</response>
        /// <response code="409">If user state can't be changed</response>
        [HttpPost("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ChangePassword(UserChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
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
                return Conflict(ex.Message);
            }

            return Ok();
        }


        /// <summary>
        /// Delete user
        /// </summary>
        /// <response code="200">If user has been deleted successfully</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If user not found</response>
        /// <response code="409">If user state can't be changed</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Delete(DeleteUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            try
            {
                var command = _mapper.Map<DeleteUserModel, DeleteUserCommand>(model);
                await _mediator.Send(command);
            }

            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UserUnableToChangeStateException ex)
            {
                return Conflict(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Change user activation state
        /// </summary>
        /// <response code="200">If activation state was successfully updated</response>
        /// <response code="400">If model is invalid</response>
        /// <response code="404">If user not found</response>
        /// <response code="409">If user state can't be changed</response>
        [HttpPost("changeActivation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> ChangeActivation(ActivateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
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
                return Conflict(ex.Message);
            }

            return Ok();
        }
    }
}