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
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Application.Readings.Queries.DTO;
using Web.Areas.Admin.Extensions;
using Web.Areas.Admin.Infrastructure.Auth;
using Web.Areas.Admin.Models.Default.Sensors;
using Web.Helpers;

namespace Web.Areas.Admin.Controllers.Default
{
    [Area(AdminArea.Name)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = AuthPolicies.Admin)]
    public class SensorsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IReadingsQueries _readingsQueries;
        private readonly IMediator _mediator;

        public SensorsController(IReadingsQueries readingsQueries, IMediator mediator, IMapper mapper)
        {
            _readingsQueries = readingsQueries ?? throw new ArgumentNullException(nameof(readingsQueries));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ActionResult> Index()
        {
            var sensors = await _readingsQueries.GetSensorsAsync();
            var model = new SensorsIndexViewModel
            {
                PortableSensors =
                    _mapper.Map<List<PortableSensorDTO>, List<SensorListItemViewModel>>(sensors
                        .OfType<PortableSensorDTO>()
                        .ToList()),
                StaticSensors =
                    _mapper.Map<List<StaticSensorDTO>, List<StaticSensorListItemViewModel>>(sensors
                        .OfType<StaticSensorDTO>()
                        .ToList()),
            };
            return View(model);
        }
    
        #region Create

        [HttpGet]
        public ActionResult CreateStaticSensor()
        {
            return View(new CreateStaticSensorViewModel(new CreateStaticSensorModel
            {
                ApiKey = CryptoHelper.GenerateApiKey()
            }));
        }


        [HttpPost]
        public async Task<ActionResult> CreateStaticSensor([Bind(Prefix = nameof(CreateStaticSensorViewModel.Model))]
            CreateStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new CreateStaticSensorViewModel(model));
            }

            var command = _mapper.Map<CreateStaticSensorModel, CreateStaticSensorCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CreatePortableSensor()
        {
            return View(new CreatePortableSensorViewModel(new CreatePortableSensorModel
            {
                ApiKey = CryptoHelper.GenerateApiKey()
            }));
        }


        [HttpPost]
        public async Task<ActionResult> CreatePortableSensor([Bind(Prefix = nameof(CreatePortableSensorViewModel.Model))]
            CreatePortableSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new CreatePortableSensorViewModel(model));
            }

            var command = _mapper.Map<CreatePortableSensorModel, CreatePortableSensorCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }

        #endregion
        
        #region Delete

        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(int? sensorId)
        {
            return await BuildDeleteResultAsync(sensorId);
        }


        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete([Bind(Prefix = nameof(DeleteSensorViewModel.Model))]
            DeleteSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return await BuildDeleteResultAsync(model.Id, model);
            }

            var command = _mapper.Map<DeleteSensorModel, DeleteSensorCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }

        private async Task<ActionResult> BuildDeleteResultAsync(int? sensorId, DeleteSensorModel model = null)
        {
            if (!sensorId.HasValue)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, "Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, $"Sensor with id: {sensorId} not found");
            }

            if (sensor.IsActive)
            {
                return this.StatusCodeView(HttpStatusCode.Forbidden, "Unable to delete active sensor");
            }

            var detailsVM = _mapper.Map<SensorDTO, SensorDetailsViewModel>(sensor);
            var vm = new DeleteSensorViewModel(model ?? new DeleteSensorModel
            {
                Id = sensor.Id
            }, detailsVM);

            return View(vm);
        }

        #endregion

        #region Activation

        [HttpGet]
        public async Task<ActionResult> ChangeActivation(int? sensorId)
        {
            return await BuildChangeActivationResultAsync(sensorId);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeActivation([Bind(Prefix = nameof(ChangeActivationSensorViewModel.Model))]
            ChangeActivationSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return await BuildChangeActivationResultAsync(model.Id, model);
            }

            var command = _mapper.Map<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }


        private async Task<ActionResult> BuildChangeActivationResultAsync(int? sensorId,
            ChangeActivationSensorModel model = null)
        {
            if (!sensorId.HasValue)
            {
                return this.StatusCodeView(HttpStatusCode.BadRequest, "Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, $"Sensor with id: {sensorId} not found");
            }

            var detailsVM = _mapper.Map<SensorDTO, SensorDetailsViewModel>(sensor);
            var vm = new ChangeActivationSensorViewModel(model ?? new ChangeActivationSensorModel
            {
                Id = sensor.Id,
                IsActive = sensor.IsActive
            }, detailsVM);

            return View(vm);
        }

        #endregion Activation
        
        #region Visibility

        [HttpGet]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(int? sensorId)
        {
            return await BuildChangeVisibilityStaticSensorResultAsync(sensorId);
        }


        [HttpPost]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(
            [Bind(Prefix = nameof(ChangeVisibilityStaticSensorViewModel.Model))]
            ChangeVisibilityStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return await BuildChangeVisibilityStaticSensorResultAsync(model.Id, model);
            }

            try
            {
                var command =
                    _mapper.Map<ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>(model);
                await _mediator.Send(command);
            }
            catch (SensorNotFoundException ex)
            {
                return NotFound(ex);
            }

            return RedirectToAction("Index");
        }

        private async Task<ActionResult> BuildChangeVisibilityStaticSensorResultAsync(int? sensorId, ChangeVisibilityStaticSensorModel model = null)
        {
            if (!sensorId.HasValue)
            {
                return this.StatusCodeView(HttpStatusCode.BadRequest, "Id is required");
            }

            var sensor = await _readingsQueries.GetStaticSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound,"Static Sensor not found");
            }

            var detailsVM = _mapper.Map<SensorDTO, SensorDetailsViewModel>(sensor);
            var vm = new ChangeVisibilityStaticSensorViewModel(model ?? new ChangeVisibilityStaticSensorModel
            {
                Id = sensor.Id,
                IsVisible = sensor.IsVisible
            }, detailsVM);

            return View(vm);
        }
        
        #endregion

        public async Task<ActionResult> PortableSensorDetails(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return this.StatusCodeView(HttpStatusCode.BadRequest, "Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return this.StatusCodeView(HttpStatusCode.NotFound, "Sensor with id: {sensorId} not found");
            }

            return View(sensorId.Value);
        }
    }
}