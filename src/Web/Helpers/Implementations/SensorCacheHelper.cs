using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data.Repository;
using Web.Models.Cache;

namespace Web.Helpers.Implementations
{
    public class SensorCacheHelper : ISensorCacheHelper
    {
        private const string _sensorCacheKey = "CSMStaticSensors";
        private static TimeSpan _sensorCacheLifetime => new TimeSpan(1, 0, 0);


        private readonly IMemoryCache _memCache;
        private readonly IRepository _repository;
        private readonly IPollutionCalculator _pollutionCalculator;

        public SensorCacheHelper(IMemoryCache cache, IRepository repository, IPollutionCalculator pollutionCalculator)
        {
            _memCache = cache;
            _repository = repository;
            _pollutionCalculator = pollutionCalculator;
        }

        public async Task AddStaticSensorToCacheAsync(int sensorId)
        {
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == sensorId);
            if (sensorInCache == null)
            {
                var sensor = await _repository.GetStaticSensorByIdAsync(sensorId, true);
                sensorsCacheItems.Add(new SensorCacheItemModel(sensor, _pollutionCalculator.CalculatePollutionLevel(sensor.Readings.Cast<Reading>().ToList())));
                _memCache.Set(_sensorCacheKey, sensorsCacheItems, _sensorCacheLifetime);
            }
        }

        public async Task<PollutionLevel> GetPollutionLevelAsync(int sensorId)
        {
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == sensorId);
            if (sensorInCache == null)
            {
                throw new KeyNotFoundException();
            }
            return sensorInCache.PollutionLevel;
        }

        public async Task<List<SensorCacheItemModel>> GetStaticSensorsAsync()
        {
            return await _memCache.GetOrCreateAsync(_sensorCacheKey, async (entry) =>
             {
                 entry.SetAbsoluteExpiration(_sensorCacheLifetime);
                 List<SensorCacheItemModel> result = new List<SensorCacheItemModel>();
                 var sensors = await _repository.GetStaticSensorsForCacheAsync();
                 foreach (var sensor in sensors)
                 {
                     result.Add(new SensorCacheItemModel(sensor, _pollutionCalculator.CalculatePollutionLevel(sensor.Readings.Cast<Reading>().ToList())));
                 }
                 return result;
             });
        }

        public void ClearCache()
        {
            _memCache.Remove(_sensorCacheKey);
        }

        public async Task RemoveStaticSensorFromCacheAsync(int sensorId)
        {
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == sensorId);
            if (sensorInCache != null)
            {
                sensorsCacheItems.Remove(sensorInCache);
                _memCache.Set(_sensorCacheKey, sensorsCacheItems, _sensorCacheLifetime);
            }
        }

        public async Task UpdateSensorCacheWithReadingAsync(StaticSensorReading reading)
        {
            if (reading.StaticSensorId <= 0)
            {
                throw new ArgumentException("SensorId should be provided in reading!", nameof(reading.StaticSensorId));
            }
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == reading.StaticSensorId);
            if (sensorInCache == null)
            {
                throw new KeyNotFoundException();
            }
            sensorInCache.Sensor.Readings.Add(reading);
            sensorInCache.Sensor.Readings = sensorInCache.Sensor.Readings.OrderByDescending(f => f.Created).Take(10).ToList();
            sensorInCache.PollutionLevel = _pollutionCalculator.CalculatePollutionLevel(sensorInCache.Sensor.Readings.Cast<Reading>().ToList());
            _memCache.Set(_sensorCacheKey, sensorsCacheItems, _sensorCacheLifetime);
        }

        public async Task UpdateStaticSensorCacheAsync(StaticSensor sensor)
        {
            if (sensor.IsAvailable())
            {
                await AddStaticSensorToCacheAsync(sensor.Id);
            }
            else
            {
                await RemoveStaticSensorFromCacheAsync(sensor.Id);
            }
        }
    }
}
