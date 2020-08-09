using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Models.Sensors;
using Microsoft.AspNetCore.Authorization;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Commands;
using Web.Areas.Admin.Application.Readings.Queries;
using Web.Areas.Admin.Infrastructure.Filters;
using Web.Domain.Entities;
using Web.Helpers;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SensorsController : Controller
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Sensor, SensorListItemViewModel>();
            x.CreateMap<StaticSensor, StaticSensorListItemViewModel>()
                .IncludeBase<Sensor, SensorListItemViewModel>();
            x.CreateMap<Sensor, SensorDetailsViewModel>();
            x.CreateMap<StaticSensor, StaticSensorDetailsViewModel>()
                .IncludeBase<Sensor, SensorDetailsViewModel>();
            x.CreateMap<Sensor, ActivateSensorViewModel>().ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<Sensor, ChangeVisibilityStaticSensorViewModel>()
                .ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<Sensor, DeleteSensorViewModel>().ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<CreateStaticSensorModel, Sensor>();

            x.CreateMap<CreateStaticSensorModel, CreateStaticSensorCommand>()
                .ConstructUsing(z => new CreateStaticSensorCommand(z.ApiKey, z.Latitude, z.Longitude));
            x.CreateMap<CreatePortableSensorModel, CreatePortableSensorCommand>()
                .ConstructUsing(z => new CreatePortableSensorCommand(z.ApiKey));
            x.CreateMap<DeleteSensorModel, DeleteSensorCommand>()
                .ConstructUsing(z => new DeleteSensorCommand(z.Id.Value, z.IsCompletely));
            x.CreateMap<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>()
                .ConstructUsing(z => new ChangeSensorActivationStateCommand(z.Id.Value, z.IsActive));
            x.CreateMap<ChangeVisibilityStaticSensorModel, ChangeStaticSensorVisibilityStateCommand>()
                .ConstructUsing(z => new ChangeStaticSensorVisibilityStateCommand(z.Id.Value, z.IsVisible));
        }));


        private readonly IReadingsQueries _readingsQueries;
        private readonly IMediator _mediator;

        public SensorsController(IReadingsQueries readingsQueries, IMediator mediator)
        {
            _readingsQueries = readingsQueries;
            _mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            var sensors = await _readingsQueries.GetSensorsAsync();
            var model = new SensorsIndexViewModel
            {
                PortableSensors =
                    _mapper.Map<List<PortableSensor>, List<SensorListItemViewModel>>(sensors.OfType<PortableSensor>()
                        .ToList()),
                StaticSensors =
                    _mapper.Map<List<StaticSensor>, List<StaticSensorListItemViewModel>>(sensors.OfType<StaticSensor>()
                        .ToList()),
            };
            return View(model);
        }


        [HttpGet]
        [RestoreModelStateFromTempData]
        public ActionResult CreateStaticSensor()
        {
            return View(new CreateStaticSensorModel
            {
                ApiKey = CryptoHelper.GenerateApiKey()
            });
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> CreateStaticSensor(CreateStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateStaticSensor");
            }
            
            var command = _mapper.Map<CreateStaticSensorModel, CreateStaticSensorCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [RestoreModelStateFromTempData]
        public ActionResult CreatePortableSensor()
        {
            return View(new CreatePortableSensorModel
            {
                ApiKey = CryptoHelper.GenerateApiKey()
            });
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> CreatePortableSensor(CreatePortableSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }

            var command = _mapper.Map<CreatePortableSensorModel, CreatePortableSensorCommand>(model);
            await _mediator.Send(command);
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound($"Sensor with id: {sensorId} not found");
            }

            if (sensor.IsActive)
            {
                return RedirectToAction("Index");
            }

            var mappedSensor = _mapper.Map<Sensor, DeleteSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(DeleteSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Delete", new {sensorId = model.Id});
            }

            var command = _mapper.Map<DeleteSensorModel, DeleteSensorCommand>(model);
            await _mediator.Send(command);
            
            return RedirectToAction("Index");
        }


        [HttpGet]
        [RestoreModelStateFromTempData]
        public async Task<ActionResult> ChangeActivation(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound($"Sensor with id: {sensorId} not found");
            }

            var mappedSensor = _mapper.Map<Sensor, ActivateSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> ChangeActivation(ChangeActivationSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ChangeActivation", new {sensorId = model.Id});
                ;
            }

            var command = _mapper.Map<ChangeActivationSensorModel, ChangeSensorActivationStateCommand>(model);
            await _mediator.Send(command);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [RestoreModelStateFromTempData]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return NotFound("Id is required");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound("Sensor not found");
            }
            var mappedSensor = _mapper.Map<Sensor, ChangeVisibilityStaticSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(ChangeVisibilityStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ChangeVisibility", new {sensorId = model.Id});
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

        public async Task<ActionResult> PortableSensorDetails(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Sensor id is required!");
            }

            var sensor = await _readingsQueries.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound($"Sensor with id: {sensorId} not found");
            }

            return View(sensorId.Value);
        }
    }
}