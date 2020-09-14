using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Models.Cache;

namespace Web.Helpers.Interfaces
{
    public interface ISensorCacheHelper
    {
        Task<List<SensorCacheItemModel>> GetStaticSensorsAsync();

        Task UpdateStaticSensorCacheAsync(StaticSensor sensor);
        
        Task RemoveStaticSensorFromCacheAsync(int sensorId);

        void ClearCache();

        Task UpdateSensorCacheWithReadingAsync(StaticSensorReading reading);

        Task<PollutionLevel> GetPollutionLevelAsync(int sensorId);
    }
}
