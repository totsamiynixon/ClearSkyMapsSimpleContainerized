using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;
using Web.Enums;
using Web.Models.Cache;

namespace Web.Helpers.Interfaces
{
    public interface ISensorCacheHelper
    {
        Task<List<SensorCacheItemModel>> GetStaticSensorsAsync();

        Task UpdateStaticSensorCacheAsync(StaticSensor sensor);

        Task AddStaticSensorToCacheAsync(int sensorId);

        Task RemoveStaticSensorFromCacheAsync(int sensorId);

        void RemoveAllSensorsFromCache();

        Task UpdateSensorCacheWithReadingAsync(StaticSensorReading reading);

        Task<PollutionLevel> GetPollutionLevelAsync(int sensorId);
    }
}
