using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domain.Entities;

namespace Web.Areas.Admin.Application.Readings.Queries
{
    public interface IReadingsQueries
    {
        Task<List<Sensor>> GetSensorsAsync();

        Task<List<StaticSensor>> GetStaticSensorsForCacheAsync();

        Task<List<StaticSensor>> GetStaticSensorsAsync(bool withReadings);

        Task<Sensor> GetSensorByIdAsync(int id);

        Task<Sensor> GetSensorByApiKeyAsync(string apiKey);

        Task<StaticSensor> GetStaticSensorByIdAsync(int id, bool withReadings = false); 
    }
}