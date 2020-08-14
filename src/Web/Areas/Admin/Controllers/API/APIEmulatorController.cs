using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Emulation.Commands;
using Web.Application.Emulation.Exceptions;
using Web.Application.Emulation.Queries;
using Web.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.API.Emulator;

namespace Web.Areas.Admin.Controllers.API
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflict
    [Route( AdminArea.APIRoutePrefix + "/emulator")]
    [ApiController]
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

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var devices = await _emulationQueries.GetEmulatorDevicesAsync();
            return Ok(_mapper.Map<List<EmulatorDeviceDTO>, List<SensorEmulatorListItemModel>>(devices));
        }


        [HttpPost("start")]
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

        [HttpPost("stop")]
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

        [HttpPost("device/{guid}/powerOn")]
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

        [HttpPost("device/{guid}/powerOff")]
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