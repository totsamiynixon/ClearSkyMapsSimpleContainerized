using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Areas.Admin.Models.Sensors;
using Web.Data;
using Web.Data.Models;
using Web.SensorActions.Output;
using Web.Areas.Admin.Filters;
using Microsoft.AspNetCore.Authorization;
using Web.Helpers.Interfaces;
using Web;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SensorsController : Controller
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Sensor, SensorListItemViewModel>()
            .ForMember(f => f.IsConnected, m => m.MapFrom(f => (Startup.ServiceProvider().GetService(typeof(ISensorConnectionHelper)) as ISensorConnectionHelper).IsConnected(f.Id)));
            x.CreateMap<StaticSensor, StaticSensorListItemViewModel>()
            //.ForMember(f => f.PollutionLevel, m => m.MapFrom(f => (Startup.ServiceProvider()().GetService(typeof(ISensorCacheHelper)) as ISensorCacheHelper).GetPollutionLevelAsync(f.Id)))
            .IncludeBase<Sensor, SensorListItemViewModel>();
            x.CreateMap<Sensor, SensorDetailsViewModel>();
            x.CreateMap<StaticSensor, StaticSensorDetailsViewModel>()
            .IncludeBase<Sensor, SensorDetailsViewModel>();
            x.CreateMap<Sensor, ActivateSensorViewModel>().ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<Sensor, ChangeVisibilityStaticSensorViewModel>().ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<Sensor, DeleteSensorViewModel>().ForMember(f => f.Details, m => m.MapFrom(f => f));
            x.CreateMap<CreateStaticSensorModel, Sensor>();
            x.CreateMap<Sensor, PushStateActionPayload>();
        }));


        private readonly IRepository _repository;
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly ISensorConnectionHelper _sensorConnectionHelper;

        public SensorsController(IRepository repository, ISensorCacheHelper sensorCacheHelper, ISensorConnectionHelper sensorConnectionHelper)
        {
            _repository = repository;
            _sensorCacheHelper = sensorCacheHelper;
            _sensorConnectionHelper = sensorConnectionHelper;
        }

        public async Task<ActionResult> Index()
        {

            var sensors = await _repository.GetSensorsAsync();
            var model = new SensorsIndexViewModel
            {
                PortableSensors = _mapper.Map<List<PortableSensor>, List<SensorListItemViewModel>>(sensors.OfType<PortableSensor>().ToList()),
                StaticSensors = _mapper.Map<List<StaticSensor>, List<StaticSensorListItemViewModel>>(sensors.OfType<StaticSensor>().ToList()),
            };
            return View(model);
        }


        [HttpGet]
        [RestoreModelStateFromTempData]
        public ActionResult CreateStaticSensor()
        {
            return View();
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> CreateStaticSensor(CreateStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            try
            {
                await _repository.AddStaticSensorAsync(model.IPAddress, model.Latitude, model.Longitude);
            }
            catch (DbUpdateException e)
            {
                SqlException innerException = null;
                Exception tmp = e;
                while (innerException == null && tmp != null)
                {
                    if (tmp != null)
                    {
                        innerException = tmp.InnerException as SqlException;
                        tmp = tmp.InnerException;
                    }

                }
                if (innerException != null && innerException.Number == 2601)
                {
                    ModelState.AddModelError("Insert", innerException.Message);
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [RestoreModelStateFromTempData]
        public ActionResult CreatPortableSensor()
        {
            return View();
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> CreatePortableSensor(CreatePortableSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Create");
            }
            try
            {
                await _repository.AddPortableSensorAsync(model.IPAddress);
            }
            catch (DbUpdateException e)
            {
                SqlException innerException = null;
                Exception tmp = e;
                while (innerException == null && tmp != null)
                {
                    if (tmp != null)
                    {
                        innerException = tmp.InnerException as SqlException;
                        tmp = tmp.InnerException;
                    }

                }
                if (innerException != null && innerException.Number == 2601)
                {
                    ModelState.AddModelError("Insert", innerException.Message);
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Необходим id датчика!");
            }
            var sensor = await _repository.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound("Датик с таким id не найден");
            }
            if (sensor.IsActive)
            {
                return RedirectToAction("Index");
            }
            var mappedSensor = _mapper.Map<Sensor, DeleteSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpGet]
        [Authorize(Roles = "Supervisor")]
        public async Task<ActionResult> Delete(DeleteSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Delete", new { sensorId = model.Id });
            }
            await _repository.DeleteSensorAsync(model.Id.Value);
            _sensorConnectionHelper.TriggerChangeState(await _repository.GetSensorByIdAsync(model.Id.Value));
            return RedirectToAction("Index");
        }


        [HttpGet]
        [RestoreModelStateFromTempData]
        public async Task<ActionResult> ChangeActivation(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Необходим id датчика!");
            }
            var sensor = await _repository.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound("Датчик с таким id не найден");
            }
            var mappedSensor = _mapper.Map<Sensor, ActivateSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> ChangeActivation(ActivateSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ChangeActivation", new { sensorId = model.Id }); ;
            }
            var sensor = await _repository.ChangeSensorActivationAsync(model.Id.Value, model.IsActive.Value);
            if (sensor is StaticSensor)
            {
                await _sensorCacheHelper.UpdateStaticSensorCacheAsync(sensor as StaticSensor);
            }
            _sensorConnectionHelper.TriggerChangeState(sensor);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [RestoreModelStateFromTempData]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return NotFound("Необходим id датчика!");
            }
            var sensor = await _repository.GetSensorByIdAsync(sensorId.Value);
            var mappedSensor = _mapper.Map<Sensor, ChangeVisibilityStaticSensorViewModel>(sensor);
            return View(mappedSensor);
        }


        [HttpPost]
        [SetTempDataModelState]
        public async Task<ActionResult> ChangeVisibilityStaticSensor(ChangeVisibilityStaticSensorModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ChangeVisibility", new { sensorId = model.Id }); ;
            }
            var sensor = await _repository.GetSensorByIdAsync(model.Id.Value);
            if (sensor == null)
            {
                return NotFound("Датчик с таким id не найден");
            }
            sensor = await _repository.UpdateStaticSensorVisibilityAsync(model.Id.Value, model.IsVisible.Value);
            await _sensorCacheHelper.UpdateStaticSensorCacheAsync(sensor as StaticSensor);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Connect(SensorConnectDisconnectModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var sensor = await _repository.GetSensorByIdAsync(model.Id);
            if (sensor == null)
            {
                return NotFound("Датчик с таким id не найден");
            }
            if (_sensorConnectionHelper.IsConnected(sensor.Id))
            {
                return RedirectToAction("Index");
            }
            try
            {
                _sensorConnectionHelper.ConnectSensor(sensor);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Disconnect(SensorConnectDisconnectModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var sensor = await _repository.GetSensorByIdAsync(model.Id);
            if (sensor == null)
            {
                return NotFound("Датчик с таким id не найден");
            }
            if (!_sensorConnectionHelper.IsConnected(sensor.Id))
            {
                return RedirectToAction("Index");
            }
            _sensorConnectionHelper.DisconnectSensor(sensor.Id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> PortableSensorDetails(int? sensorId)
        {
            if (!sensorId.HasValue)
            {
                return BadRequest("Необходим id датчика!");
            }
            var sensor = await _repository.GetSensorByIdAsync(sensorId.Value);
            if (sensor == null)
            {
                return NotFound("Датик с таким id не найден");
            }
            return View(sensorId.Value);
        }


    }
}