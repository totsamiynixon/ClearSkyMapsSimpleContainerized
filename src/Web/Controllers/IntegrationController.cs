using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Data;
using Web.Data.Models;
using Web.Helpers.Interfaces;
using Web.Models.Api.Sensor;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IAdminDispatchHelper _adminDispatchHelper;
        private readonly IPWADispatchHelper _pwaDispatchHelper;
        private readonly ISensorCacheHelper _sensorCacheHelper;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorDataModel, Reading>();
            x.CreateMap<SensorDataModel, StaticSensorReading>();

        }));

        public IntegrationController(IRepository repository, IAdminDispatchHelper adminDispatchHelper, IPWADispatchHelper pwaDispatchHelper, ISensorCacheHelper sensorCacheHelper)
        {
            _repository = repository;
            _adminDispatchHelper = adminDispatchHelper;
            _pwaDispatchHelper = pwaDispatchHelper;
            _sensorCacheHelper = sensorCacheHelper;
        }

        //[Route("static")]
        [HttpPost]
        public async Task<IActionResult> PortDataAsync(SensorDataModel model)
        {
            var sensor = await _repository.GetSensorByApiKeyAsync(model.ApiKey);
            if (sensor == null)
            {
                return NotFound();
            }
            if (sensor is PortableSensor)
            {
                var reading = _mapper.Map<SensorDataModel, Reading>(model);
                _adminDispatchHelper.DispatchReadingsForPortableSensor(sensor.Id, reading);
                _adminDispatchHelper.DispatchCoordinatesForPortableSensor(sensor.Id, model.Latitude, model.Longitude);
            }
            else if (sensor is StaticSensor)
            {
                var reading = _mapper.Map<SensorDataModel, StaticSensorReading>(model);
                reading.StaticSensorId = sensor.Id;
                await _repository.AddReadingAsync(reading);
                await _sensorCacheHelper.UpdateSensorCacheWithReadingAsync(reading);
                _adminDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, reading);
                var pollutionLevel = await _sensorCacheHelper.GetPollutionLevelAsync(sensor.Id);
                _pwaDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, pollutionLevel, reading);
            }
            return Ok();
        }
    }
}