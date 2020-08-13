using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Application.Emulation.Commands;
using Web.Application.Emulation.Exceptions;
using Web.Areas.Admin.Extensions;
using Web.Areas.Admin.Models.Default.Emulator;
using Web.Emulation;

namespace Web.Areas.Admin.Controllers.Default
{
    [Area(AdminArea.Name)]
    [Authorize]
    public class EmulatorController : Controller
    {
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorEmulator, SensorEmulatorListItemViewModel>()
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

            return View(_mapper.Map<List<SensorEmulator>, List<SensorEmulatorListItemViewModel>>(emulators));
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
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }

            return RedirectToAction("Index");
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
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }

            return RedirectToAction("Index");
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
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (EmulationDeviceNotFoundException ex)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, ex.Message);
            }

            return this.StatusCodeView(HttpStatusCode.OK, "Emulator device has been successfully started");
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
                return this.StatusCodeView(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (EmulationDeviceNotFoundException ex)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, ex.Message);
            }

            return this.StatusCodeView(HttpStatusCode.OK, "Emulator device has been successfully stopped");
        }
    }
}