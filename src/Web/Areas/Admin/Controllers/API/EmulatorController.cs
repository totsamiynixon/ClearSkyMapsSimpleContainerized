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
    [Area("Admin")]
    [Authorize]
    //TODO: check area based api routes
    [Route("api/{area}/{controller}")]
    public class EmulatorController : Controller
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

        public EmulatorController(Emulator emulator, IMediator mediator)
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


        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
        public ActionResult PowerOn(string guid)
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

        [HttpPost]
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