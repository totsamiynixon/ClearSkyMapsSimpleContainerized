using System;
using System.Collections.Generic;
using Web.Domain.Entities;
using Web.Helpers;
using Web.Infrastructure;

namespace Web.Areas.Admin.Emulation
{
    public class Emulator
    {
        public bool IsEmulationStarted { get; private set; }
        private Random _emulatorRandom = new Random();
        private bool _firstInit = true;
        private List<string> _guids { get; set; }
        public List<SensorEmulator> Devices { get; private set; }
        
        private readonly AppSettings _appSettings;
        
        public Emulator(AppSettings appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }
        
        public void RunEmulation()
        {
            if (!IsEmulationStarted)
            {
                IsEmulationStarted = true;
                SeedSensors();
            }
        }

        public void StopEmulation()
        {
            if (IsEmulationStarted)
            {
                IsEmulationStarted = false;
                ClearSensors();
            }
        }

        private void SeedSensors()
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

        private void ClearSensors()
        {
            Devices.Clear();
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