using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
            x.CreateMap<SensorDataModel, PortableSensorReading>();
            x.CreateMap<SensorDataModel, StaticSensorReading>();
        }));

        public IntegrationController(IRepository repository, IAdminDispatchHelper adminDispatchHelper,
            IPWADispatchHelper pwaDispatchHelper, ISensorCacheHelper sensorCacheHelper)
        {
            _repository = repository;
            _adminDispatchHelper = adminDispatchHelper;
            _pwaDispatchHelper = pwaDispatchHelper;
            _sensorCacheHelper = sensorCacheHelper;
        }
   
        [HttpGet("getaspost")]
        public async Task<IActionResult> GetAsPostDataAsync(string data)
        {
            var model = GetModelFromString(data);
            if (model == null)
            {
                return BadRequest("Invalid Data");
            }

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
            else if (sensor is StaticSensor staticSensor)
            {
                var reading = _mapper.Map<SensorDataModel, StaticSensorReading>(model);
                reading.StaticSensorId = sensor.Id;
                await _repository.AddReadingAsync(reading);
                _adminDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, reading);
                if (staticSensor.IsAvailable())
                {
                    await _sensorCacheHelper.UpdateSensorCacheWithReadingAsync(reading);
                    var pollutionLevel = await _sensorCacheHelper.GetPollutionLevelAsync(sensor.Id);
                    _pwaDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, pollutionLevel, reading);
                }
            }

            return Ok();
        }


        private SensorDataModel GetModelFromString(string data)
        {
            try
            {
                var trimmed = data.Trim(';');
                var groupes = trimmed.Split(",").Select(x => x.Replace('.', ',')).ToArray();
                return new SensorDataModel
                {
                    ApiKey = groupes[0],
                    Temp = float.Parse(groupes[1]),
                    Hum = float.Parse(groupes[2]),
                    Preassure = float.Parse(groupes[3]),
                    CO2 = float.Parse(groupes[4]),
                    LPG = float.Parse(groupes[5]),
                    CO = float.Parse(groupes[6]),
                    CH4 = float.Parse(groupes[7]),
                    Dust = float.Parse(groupes[8]),
                    Longitude = float.Parse(groupes[9]),
                    Latitude = float.Parse(groupes[10])
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}