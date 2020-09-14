using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Application.Emulation.Commands;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Application.Emulation.Queries;
using Web.Areas.Admin.Application.Emulation.Queries.DTO;
using Web.Areas.Admin.Extensions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Emulator;

namespace Web.Areas.Admin.Controllers.Default
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    public class EmulatorController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IEmulationQueries _emulationQueries;

        public EmulatorController(IMediator mediator, IMapper mapper, IEmulationQueries emulationQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emulationQueries = emulationQueries ?? throw new ArgumentNullException(nameof(emulationQueries));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var devices = await _emulationQueries.GetEmulatorDevicesAsync();
            return View(_mapper.Map<List<EmulatorDeviceDTO>, List<SensorEmulatorListItemViewModel>>(devices));
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