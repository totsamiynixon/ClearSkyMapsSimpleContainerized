using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Web.Data;
using Web.Data.Models;
using Web.Helpers;
using Web.Helpers.Interfaces;
using Z.EntityFramework.Plus;

namespace Web.Areas.Admin.Emulation
{
    public static class Emulator
    {
        public static bool IsEmulationEnabled { get; private set; }
        private static Random _emulatorRandom = new Random();
        private static bool _firstInit = true;
        private static List<string> _guids { get; set; }
        public static List<SensorEmulator> Devices { get; private set; }

        private static ISensorConnectionHelper SensorConnectionHelper => (Startup.ServiceProvider().GetService(typeof(ISensorConnectionHelper)) as ISensorConnectionHelper);
        private static IRepository Repository => (Startup.ServiceProvider().GetService(typeof(IRepository)) as IRepository);
        private static ISensorCacheHelper SensorCacheHelper => (Startup.ServiceProvider().GetService(typeof(ISensorCacheHelper)) as ISensorCacheHelper);


        public static async Task RunEmulationAsync()
        {
            if (!IsEmulationEnabled)
            {
                IsEmulationEnabled = true;
                SensorConnectionHelper.DisconnectAllSensors();
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
                SensorConnectionHelper.DisconnectAllSensors();
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
                await Repository.AddStaticSensorAsync(fakeSensor.sensor.IPAddress, fakeSensor.sensor.Latitude, fakeSensor.sensor.Longitude);
                Devices.Add(fakeSensor.emulator);
            }
            var iterationsForPortable = _emulatorRandom.Next(5, 10);
            for (int i = 0; i < iterationsForPortable; i++)
            {
                var guid = Guid.NewGuid().ToString();
                _guids.Add(guid);
                var fakeSensor = GetPortableFakeSensor(guid);
                await Repository.AddPortableSensorAsync(fakeSensor.sensor.IPAddress);
                Devices.Add(fakeSensor.emulator);
            }
        }

        private static (SensorEmulator emulator, StaticSensor sensor) GetStaticFakeSensor(string guid)
        {
            var sensorEmulator = new SensorEmulator(GetLocalIPAddress(), GetAvailableLocalPort().ToString(), guid);
            var coordinates = sensorEmulator.GetCoordinates();
            var sensor = new StaticSensor
            {
                IPAddress = $"{sensorEmulator.GetIp()}:{sensorEmulator.GetPort()}",
                Latitude = coordinates.latitude,
                Longitude = coordinates.longitude
            };
            return (sensorEmulator, sensor);
        }


        private static (SensorEmulator emulator, PortableSensor sensor) GetPortableFakeSensor(string guid)
        {
            var sensorEmulator = new SensorEmulator(GetLocalIPAddress(), GetAvailableLocalPort().ToString(), guid);
            var sensor = new PortableSensor
            {
                IPAddress = $"{sensorEmulator.GetIp()}:{sensorEmulator.GetPort()}"
            };
            return (sensorEmulator, sensor);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static int GetAvailableLocalPort()
        {

            for (var port = 10000; port <= 11000; port++)
            {
                bool isAvailable = true;
                if (Devices.Any(f => f.GetPort() == port.ToString()))
                {
                    isAvailable = false;
                }
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                {
                    if (tcpi.LocalEndPoint.Port == port)
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    return port;
                }
            }
            throw new Exception("Free port not found");
        }
    }
}