using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;

namespace Web.Data
{
    public interface IRepository
    {
        Task<StaticSensor> AddStaticSensorAsync(string apiKey, double latitude, double longitude);

        Task<PortableSensor> AddPortableSensorAsync(string apiKey);

        Task<StaticSensor> UpdateStaticSensorCoordinatesAsync(int id, double latitude, double longitude);

        Task<StaticSensor> UpdateStaticSensorVisibilityAsync(int id, bool value);

        Task<Sensor> ChangeSensorActivationAsync(int id, bool value);

        Task<List<Sensor>> GetSensorsAsync();

        Task<List<StaticSensor>> GetStaticSensorsForCacheAsync();

        Task<List<StaticSensor>> GetStaticSensorsAsync(bool withReadings);

        Task<Sensor> GetSensorByIdAsync(int id);

        Task<Sensor> GetSensorByApiKeyAsync(string apiKey);

        Task<StaticSensor> GetStaticSensorByIdAsync(int id, bool withReadings = false);

        Task DeleteSensorAsync(int id);

        Task RemoveSensorsFromDatabaseAsync(params int[] ids);

        Task RemoveAllSensorsFromDatabaseAsync();

        Task<StaticSensorReading> AddReadingAsync(StaticSensorReading reading);

        void ReinitializeDb();
    }
}
