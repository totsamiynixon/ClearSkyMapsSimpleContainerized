using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Domain.Entities;
using Web.Helpers;
using Web.Infrastructure;

namespace Web.Emulation
{
    public class Emulator
    {
        public bool IsEmulationEnabled { get; private set; }
        private Random _emulatorRandom = new Random();
        private bool _firstInit = true;
        private List<string> _guids { get; set; }
        public List<SensorEmulator> Devices { get; private set; }
        
        private readonly AppSettings _appSettings;
        
        public Emulator(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }
        
        public async Task RunEmulationAsync()
        {
            if (!IsEmulationEnabled)
            {
                IsEmulationEnabled = true;
                /*_repository.ReinitializeDb();
                _sensorCacheHelper.RemoveAllSensorsFromCache();*/
            }
            if (_firstInit)
            {
                await SeedSensorsAsync();
                _firstInit = false;
            }
        }

        public void StopEmulation()
        {
            if (IsEmulationEnabled)
            {
                IsEmulationEnabled = false;
                /*_repository.ReinitializeDb();
                _sensorCacheHelper.RemoveAllSensorsFromCache();*/
            }
        }

        private async Task SeedSensorsAsync()
        {
            _guids = new List<string>();
            Devices = new List<SensorEmulator>();
            /*await _repository.RemoveAllSensorsFromDatabaseAsync();
            _sensorCacheHelper.RemoveAllSensorsFromCache();*/
            var iterationsForStatic = _emulatorRandom.Next(5, 10);
            for (int i = 0; i < iterationsForStatic; i++)
            {
                var guid = Guid.NewGuid().ToString();
                _guids.Add(guid);
                var fakeSensor = GetStaticFakeSensor(guid);
               // await _repository.AddStaticSensorAsync(fakeSensor.sensor.ApiKey, fakeSensor.sensor.Latitude, fakeSensor.sensor.Longitude);
                Devices.Add(fakeSensor.emulator);
            }
            var iterationsForPortable = _emulatorRandom.Next(5, 10);
            for (int i = 0; i < iterationsForPortable; i++)
            {
                var guid = Guid.NewGuid().ToString();
                _guids.Add(guid);
                var fakeSensor = GetPortableFakeSensor(guid);
                //await _repository.AddPortableSensorAsync(fakeSensor.sensor.ApiKey);
                Devices.Add(fakeSensor.emulator);
            }
        }

        private (SensorEmulator emulator, StaticSensor sensor) GetStaticFakeSensor(string guid)
        {

            var apiKey = CryptoHelper.GenerateApiKey();
            var sensorEmulator = new SensorEmulator(guid, _appSettings.ServerUrl, apiKey, typeof(StaticSensor));
            var sensor = new StaticSensor
            {
                ApiKey = apiKey,
                Latitude = sensorEmulator.Latitude,
                Longitude = sensorEmulator.Longitude
            };
            return (sensorEmulator, sensor);
        }


        private (SensorEmulator emulator, PortableSensor sensor) GetPortableFakeSensor(string guid)
        {
            var apiKey = CryptoHelper.GenerateApiKey();
            var sensorEmulator = new SensorEmulator(guid, _appSettings.ServerUrl, apiKey, typeof(PortableSensor));
            var sensor = new PortableSensor
            {
                ApiKey = apiKey
            };
            return (sensorEmulator, sensor);
        }
    }
}