using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Application.Emulation.Commands;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Application.Emulation.Queries;
using Web.Areas.Admin.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Emulator;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflict
    [Route(AdminArea.APIRoutePrefix + "/emulator")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class APIEmulatorController : Controller
    {
        private readonly IEmulationQueries _emulationQueries;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public APIEmulatorController(IMediator mediator, IMapper mapper, IEmulationQueries emulationQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emulationQueries = emulationQueries ?? throw new ArgumentNullException(nameof(emulationQueries));
        }

        /// <summary>
        /// Returns all emulator sensors
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SensorEmulatorListItemModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            var devices = await _emulationQueries.GetEmulatorDevicesAsync();
            return Ok(_mapper.Map<List<EmulatorDeviceDTO>, List<SensorEmulatorListItemModel>>(devices));
        }

        /// <summary>
        /// Start emulation
        /// </summary>
        /// <response code="403">If emulation is not available</response>
        [HttpPost("start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> StartEmulation()
        {
            try
            {
                await _mediator.Send(new BeginEmulationCommand());
            }
            catch (EmulationIsNotAvailableException ex)
            {
                return Forbid(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Stop emulation
        /// </summary>
        /// <response code="403">If emulation is not available</response>
        [HttpPost("stop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> StopEmulation()
        {
            try
            {
                await _mediator.Send(new StopEmulationCommand());
            }
            catch (EmulationIsNotAvailableException ex)
            {
                return Forbid(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Power on emulator device
        /// </summary>
        /// <response code="403">If emulation is not available</response>
        /// <response code="404">If emulator device was not found</response>
        [HttpPost("device/{guid}/powerOn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult DevicePowerOn(string guid)
        {
            try
            {
                _mediator.Send(new DevicePowerOnCommand(guid));
            }
            catch (EmulationIsNotAvailableException ex)
            {
                return Forbid(ex.Message);
            }
            catch (EmulationDeviceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok("Emulator device has been successfully started");
        }

        /// <summary>
        /// Power off emulator device
        /// </summary>
        /// <response code="403">If emulation is not available</response>
        /// <response code="404">If emulator device was not found</response>
        [HttpPost("device/{guid}/powerOff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult PowerOff(string guid)
        {
            try
            {
                _mediator.Send(new DevicePowerOffCommand(guid));
            }
            catch (EmulationIsNotAvailableException ex)
            {
                return Forbid(ex.Message);
            }
            catch (EmulationDeviceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok("Emulator device has been successfully stopped");
        }
    }
}