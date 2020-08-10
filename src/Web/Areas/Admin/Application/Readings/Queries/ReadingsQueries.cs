using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Web.Areas.Admin.Application.Readings.Queries.DTO;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Queries
{
    public class ReadingsQueries : IReadingsQueries
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Sensor, SensorDTO>();
            x.CreateMap<StaticSensor, StaticSensorDTO>()
                .IncludeBase<Sensor, SensorDTO>();
            x.CreateMap<PortableSensor, PortableSensorDTO>()
                .IncludeBase<Sensor, SensorDTO>();
        }));
        
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public ReadingsQueries(
            IDataContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<List<SensorDTO>> GetSensorsAsync()
        {
            await using var context = _dataContextFactory.Create();
            var query = context
                .Set<Sensor>().AsNoTracking().Where(f => !f.IsDeleted);
            var sensors = await query.ToListAsync();
            return _mapper.Map<List<Sensor>, List<SensorDTO>>(sensors);
        }

        public async Task<List<StaticSensorDTO>> GetStaticSensorsAsync(bool withReadings)
        {
            await using var context = _dataContextFactory.Create();
            var query = context
                .StaticSensors.AsNoTracking().Where(f => !f.IsDeleted);
            if (withReadings)
            {
                query = query.Select(f => new StaticSensor
                {
                    Id = f.Id,
                    ApiKey = f.ApiKey,
                    IsActive = f.IsActive,
                    IsDeleted = f.IsDeleted,
                    IsVisible = f.IsVisible,
                    Latitude = f.Latitude,
                    Longitude = f.Longitude,
                    Readings = f.Readings
                        .OrderByDescending(s => s.Created)
                        .Take(10).ToList()
                });
            }

            var sensors = await query.ToListAsync();
            
            return _mapper.Map<List<StaticSensor>, List<StaticSensorDTO>>(sensors);
        }

        public async Task<SensorDTO> GetSensorByIdAsync(int id)
        {
            await using var context = _dataContextFactory.Create();
            return _mapper.Map<Sensor, SensorDTO>(await context.Sensors.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id));
        }

        public async Task<Sensor> GetSensorByApiKeyAsync(string apiKey)
        {
            await using var context = _dataContextFactory.Create();
            return await context.Sensors.AsNoTracking()
                .FirstOrDefaultAsync(f => f.ApiKey == apiKey && f.IsActive && !f.IsDeleted);
        }

        public async Task<StaticSensor> GetStaticSensorByIdAsync(int id, bool withReadings = false)
        {
            await using var context = _dataContextFactory.Create();
            var query = context
                .StaticSensors.AsNoTracking().AsQueryable();
            if (withReadings)
            {
                query = query.Select(f => new StaticSensor
                {
                    Id = f.Id,
                    ApiKey = f.ApiKey,
                    IsActive = f.IsActive,
                    IsDeleted = f.IsDeleted,
                    IsVisible = f.IsVisible,
                    Latitude = f.Latitude,
                    Longitude = f.Longitude,
                    Readings = f.Readings
                        .OrderByDescending(s => s.Created)
                        .Take(10).ToList()
                });
            }

            return await query.FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}