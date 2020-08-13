using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Emulation.Commands;
using Web.Application.Emulation.Exceptions;
using Web.Areas.Admin.Models.API.Emulator;
using Web.Emulation;

namespace Web.Areas.Admin.Controllers.API
{
    [Authorize]
    [Area(AdminArea.Name)]
    //TODO: check area based api routes
    //TODO: check how to resolve naming conflict
    [Route( AdminArea.APIRoutePrefix + "/emulator")]
    [ApiController]
    public class APIEmulatorController : Controller
    {
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorEmulator, SensorEmulatorListItemModel>()
                .ForMember(f => f.Latitude, m => m.MapFrom((s, d) => s.Latitude))
                .ForMember(f => f.Longitude, m => m.MapFrom((s, d) => s.Longitude))
                .ForMember(f => f.Guid, m => m.MapFrom(s => s.GetGuid()))
                .ForMember(f => f.IsOn, m => m.MapFrom(s => s.IsPowerOn))
                .ForMember(f => f.ApiKey, m => m.MapFrom(s => s.ApiKey))
                .ForMember(f => f.Type, m => m.MapFrom(s => s.SensorType.Name))
                ;
        }));

        public APIEmulatorController(Emulator emulator, IMediator mediator)
        {
            _emulator = emulator;
            _mediator = mediator;
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<SensorEmulator> emulators = new List<SensorEmulator>();
            if (_emulator.IsEmulationEnabled)
            {
                emulators = _emulator.Devices;
            }

            return Ok(_mapper.Map<List<SensorEmulator>, List<SensorEmulatorListItemModel>>(emulators));
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