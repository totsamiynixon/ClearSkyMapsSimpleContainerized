using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Helpers.Interfaces;
using Web.Models.Cache;

namespace Web.Helpers.Implementations
{
    public class SensorCacheHelper : ISensorCacheHelper
    {
        private const string SensorCacheKey = "CSMStaticSensors";
        private static TimeSpan SensorCacheLifetime => new TimeSpan(1, 0, 0);


        private readonly IMemoryCache _memCache;
        private readonly IPollutionCalculator _pollutionCalculator;

        public SensorCacheHelper(IMemoryCache cache, IPollutionCalculator pollutionCalculator)
        {
            _memCache = cache ?? throw new ArgumentNullException(nameof(cache));
            _pollutionCalculator = pollutionCalculator ?? throw new ArgumentNullException(nameof(pollutionCalculator));
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

        public Task<List<SensorCacheItemModel>> GetStaticSensorsAsync()
        {
            //TODO: Investigate that moment
            return Task.FromResult(_memCache.Get<List<SensorCacheItemModel>>(SensorCacheKey) ?? new List<SensorCacheItemModel>());
        }

        public void ClearCache()
        {
            _memCache.Remove(SensorCacheKey);
        }

        public async Task RemoveStaticSensorFromCacheAsync(int sensorId)
        {
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == sensorId);
            if (sensorInCache != null)
            {
                sensorsCacheItems.Remove(sensorInCache);
                _memCache.Set(SensorCacheKey, sensorsCacheItems, SensorCacheLifetime);
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
            _memCache.Set(SensorCacheKey, sensorsCacheItems, SensorCacheLifetime);
        }

        public async Task UpdateStaticSensorCacheAsync(StaticSensor sensor)
        {
            if (sensor.IsAvailable())
            {
                await AddStaticSensorToCacheAsync(sensor);
            }
            else
            {
                await RemoveStaticSensorFromCacheAsync(sensor.Id);
            }
        }
        
        private async Task AddStaticSensorToCacheAsync(StaticSensor sensor)
        {
            var sensorsCacheItems = await GetStaticSensorsAsync();
            var sensorInCache = sensorsCacheItems.FirstOrDefault(f => f.Sensor.Id == sensor.Id);
            if (sensorInCache == null)
            {
                sensorsCacheItems.Add(new SensorCacheItemModel(sensor, _pollutionCalculator.CalculatePollutionLevel(sensor.Readings.Cast<Reading>().ToList())));
                _memCache.Set(SensorCacheKey, sensorsCacheItems, SensorCacheLifetime);
            }
        }
    }
}
