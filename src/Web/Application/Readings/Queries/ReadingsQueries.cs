using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Web.Application.Readings.Queries.DTO;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;

namespace Web.Application.Readings.Queries
{
    public class ReadingsQueries : IReadingsQueries
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Reading, StaticSensorReadingDTO>();
        }));

        private readonly ISensorCacheHelper _sensorCacheHelper;

        public ReadingsQueries(ISensorCacheHelper sensorCacheHelper)
        {
            _sensorCacheHelper = sensorCacheHelper;
        }

        public async Task<List<StaticSensorDTO>> GetStaticSensorsAsync()
        {
            var sensors = await _sensorCacheHelper.GetStaticSensorsAsync();
            
            var model = sensors?.Select(f => new StaticSensorDTO
            {
                Id = f.Sensor.Id,
                Latitude = f.Sensor.Latitude,
                Longitude = f.Sensor.Longitude,
                PollutionLevel = f.PollutionLevel,
                Readings = _mapper.Map<List<StaticSensorReading>, List<StaticSensorReadingDTO>>(f.Sensor.Readings)
            }).ToList();

            return model;
        }
    }
}