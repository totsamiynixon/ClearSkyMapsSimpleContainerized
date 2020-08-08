using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Infrastructure.Data.Factory;

namespace Web.Infrastructure.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IDataContextFactory<IdentityDataContext> _identityDataContextFactory;

        public Repository(IServiceProvider serviceProvider,
            IDataContextFactory<DataContext> dataContextFactory,
            IDataContextFactory<IdentityDataContext> identityDataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
            _identityDataContextFactory = identityDataContextFactory;
        }

        public async Task<StaticSensor> AddStaticSensorAsync(string apiKey, double latitude, double longitude)
        {
            using (var context = _dataContextFactory.Create())
            {
                var sensor = new StaticSensor
                {
                    Readings = new List<StaticSensorReading>(),
                    ApiKey = apiKey,
                };
                sensor.Latitude = latitude;
                sensor.Longitude = longitude;
                context.StaticSensors.Add(sensor);
                await context.SaveChangesAsync();
                return sensor;
            }

            ;
        }

        public async Task<PortableSensor> AddPortableSensorAsync(string apiKey)
        {
            using (var context = _dataContextFactory.Create())
            {
                var sensor = new PortableSensor
                {
                    ApiKey = apiKey
                };
                context.PortableSensors.Add(sensor);
                await context.SaveChangesAsync();
                return sensor;
            }
        }

        public async Task<StaticSensor> UpdateStaticSensorCoordinatesAsync(int id, double latitude, double longitude)
        {
            using (var context = _dataContextFactory.Create())
            {
                var staticSensor = await context.StaticSensors.FirstOrDefaultAsync(f => f.Id == id);
                staticSensor.Latitude = latitude;
                staticSensor.Longitude = longitude;
                context.Entry(staticSensor).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return staticSensor;
            }
        }

        public async Task<StaticSensor> UpdateStaticSensorVisibilityAsync(int id, bool value)
        {
            using (var context = _dataContextFactory.Create())
            {
                var staticSensor = await context.StaticSensors.FirstOrDefaultAsync(f => f.Id == id);
                staticSensor.IsVisible = value;
                context.Entry(staticSensor).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return staticSensor;
            }
        }

        public async Task<Sensor> ChangeSensorActivationAsync(int id, bool value)
        {
            using (var context = _dataContextFactory.Create())
            {
                var sensor = await context.Sensors.FirstOrDefaultAsync(f => f.Id == id);
                sensor.IsActive = value;
                context.Entry(sensor).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return sensor;
            }
        }

        public async Task<List<Sensor>> GetSensorsAsync()
        {
            using (var context = _dataContextFactory.Create())
            {
                var query = context
                    .Set<Sensor>().AsNoTracking().Where(f => !f.IsDeleted);
                var sensors = await query.ToListAsync();
                return sensors;
            }
        }

        public async Task<List<StaticSensor>> GetStaticSensorsForCacheAsync()
        {
            using (var context = _dataContextFactory.Create())
            {
                var sensors = await context
                    .StaticSensors.AsNoTracking().Where(f => f.IsActive && f.IsVisible && !f.IsDeleted).Select(f =>
                        new StaticSensor
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
                        }).ToListAsync();
                return sensors;
            }
        }

        public async Task<List<StaticSensor>> GetStaticSensorsAsync(bool withReadings)
        {
            using (var context = _dataContextFactory.Create())
            {
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
                return sensors;
            }
        }

        public async Task<Sensor> GetSensorByIdAsync(int id)
        {
            using (var context = _dataContextFactory.Create())
            {
                return await context.Sensors.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            }
        }

        public async Task<Sensor> GetSensorByApiKeyAsync(string apiKey)
        {
            using (var context = _dataContextFactory.Create())
            {
                return await context.Sensors.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.ApiKey == apiKey && f.IsActive && !f.IsDeleted);
            }
        }

        public async Task<StaticSensor> GetStaticSensorByIdAsync(int id, bool withReadings = false)
        {
            using (var context = _dataContextFactory.Create())
            {
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

        public async Task DeleteSensorAsync(int id, bool isCompletely)
        {
            using (var context = _dataContextFactory.Create())
            {
                var sensor = await context.Sensors.FirstOrDefaultAsync(f => f.Id == id);
                if (!isCompletely)
                {
                    sensor.IsDeleted = true;
                }
                else
                { 
                    context.Sensors.Remove(sensor);
                }
                
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveSensorsFromDatabaseAsync(params int[] ids)
        {
            using (var context = _dataContextFactory.Create())
            { 
                context.Sensors.RemoveRange(context.Sensors.Where(f => ids.Any(s => s == f.Id)));
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllSensorsFromDatabaseAsync()
        {
            using (var context = _dataContextFactory.Create())
            {
                context.Sensors.RemoveRange(context.Sensors.Where(f => true));
                await context.SaveChangesAsync();
            }
        }

        public async Task<StaticSensorReading> AddReadingAsync(StaticSensorReading reading)
        {
            if (reading.StaticSensorId <= 0)
            {
                throw new ArgumentException("SensorId should be provided in reading!",
                    nameof(reading.StaticSensorId));
            }

            using (var context = _dataContextFactory.Create())
            {
                context.StaticSensorReadings.Add(reading);
                await context.SaveChangesAsync();
                return reading;
            }
        }
    }
}