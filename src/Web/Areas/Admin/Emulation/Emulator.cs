using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Models;
using Web.Helpers;
using Web.Helpers.Interfaces;

namespace Web.Areas.Admin.Emulation
{
    public static class Emulator
    {
        public static bool IsEmulationEnabled { get; private set; }
        private static Random _emulatorRandom = new Random();
        private static bool _firstInit = true;
        private static List<string> _guids { get; set; }
        public static List<SensorEmulator> Devices { get; private set; }

        private static IRepository Repository => (Startup.ServiceProvider().GetService(typeof(IRepository)) as IRepository);
        private static ISensorCacheHelper SensorCacheHelper => (Startup.ServiceProvider().GetService(typeof(ISensorCacheHelper)) as ISensorCacheHelper);
        private static ISettingsProvider SettingsProvider => (Startup.ServiceProvider().GetService(typeof(ISettingsProvider)) as ISettingsProvider);


        public static async Task RunEmulationAsync()
        {
            if (!IsEmulationEnabled)
            {
                IsEmulationEnabled = true;
                Repository.ReinitializeDb();
                SensorCacheHelper.RemoveAllSensorsFromCache();
            }
            if (_firstInit)
            {
                await SeedSensorsAsync();
                _firstInit = false;
            }
        }

        public static void StopEmulation()
        {
            if (IsEmulationEnabled)
            {
                IsEmulationEnabled = false;
                Repository.ReinitializeDb();
                SensorCacheHelper.RemoveAllSensorsFromCache();
            }
        }

        private static async Task SeedSensorsAsync()
        {
            _guids = new List<string>();
            Devices = new List<SensorEmulator>();
            await Repository.RemoveAllSensorsFromDatabaseAsync();
            SensorCacheHelper.RemoveAllSensorsFromCache();
            var iterationsForStatic = _emulatorRandom.Next(5, 10);
            for (int i = 0; i < iterationsForStatic; i++)
            {
                var guid = Guid.NewGuid().ToString();
                _guids.Add(guid);
                var fakeSensor = GetStaticFakeSensor(guid);
                await Repository.AddStaticSensorAsync(fakeSensor.sensor.ApiKey, fakeSensor.sensor.Latitude, fakeSensor.sensor.Longitude);
                Devices.Add(fakeSensor.emulator);
            }
            var iterationsForPortable = _emulatorRandom.Next(5, 10);
            for (int i = 0; i < iterationsForPortable; i++)
            {
                var guid = Guid.NewGuid().ToString();
                _guids.Add(guid);
                var fakeSensor = GetPortableFakeSensor(guid);
                await Repository.AddPortableSensorAsync(fakeSensor.sensor.ApiKey);
                Devices.Add(fakeSensor.emulator);
            }
        }

        private static (SensorEmulator emulator, StaticSensor sensor) GetStaticFakeSensor(string guid)
        {

            var apiKey = CryptoHelper.GenerateApiKey();
            var sensorEmulator = new SensorEmulator(guid, SettingsProvider.ServerIP, apiKey, typeof(StaticSensor));
            var sensor = new StaticSensor
            {
                ApiKey = apiKey,
                Latitude = sensorEmulator.Latitude,
                Longitude = sensorEmulator.Longitude
            };
            return (sensorEmulator, sensor);
        }


        private static (SensorEmulator emulator, PortableSensor sensor) GetPortableFakeSensor(string guid)
        {
            var apiKey = CryptoHelper.GenerateApiKey();
            var sensorEmulator = new SensorEmulator(guid, SettingsProvider.ServerIP, apiKey, typeof(PortableSensor));
            var sensor = new PortableSensor
            {
                ApiKey = apiKey
            };
            return (sensorEmulator, sensor);
        }
    }
}