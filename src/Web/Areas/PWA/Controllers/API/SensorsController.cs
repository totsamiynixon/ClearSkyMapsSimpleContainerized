using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Data.Models;
using Web.Helpers.Interfaces;
using Web.Models.Api.Sensor;

namespace Web.Areas.PWA.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;

        public SensorsController(ISensorCacheHelper sensorCacheHelper)
        {
            _sensorCacheHelper = sensorCacheHelper;
        }

        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<StaticSensor, StaticSensorModel>();
            x.CreateMap<Reading, StaticSensorReadingModel>();
        }));

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var sensors = await _sensorCacheHelper.GetStaticSensorsAsync();
            var model = sensors.Select(f => new StaticSensorModel
            {
                Id = f.Sensor.Id,
                Latitude = f.Sensor.Latitude,
                Longitude = f.Sensor.Longitude,
                PollutionLevel = f.PollutionLevel,
                Readings = _mapper.Map<List<StaticSensorReading>, List<StaticSensorReadingModel>>(f.Sensor.Readings)
            });
            return Ok(model.ToArray());
        }
    }
}