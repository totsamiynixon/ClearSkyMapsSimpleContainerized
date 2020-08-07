using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using Web.Areas.Admin.Emulation.Models;
using Web.Domain.Entities;

namespace Web.Areas.Admin.Emulation
{
    public class SensorEmulator
    {
        private readonly string _sensorGuid;
        private readonly string _apiKey;
        private readonly string _serverIp;
        private readonly System.Timers.Timer _timer;


        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly Random _emulatorRandom = new Random();


        public SensorEmulator(string guid, string serverIp, string apiKey, Type type)
        {
            _sensorGuid = guid;
            _apiKey = apiKey;
            _serverIp = serverIp;
            SensorType = type;
            var location = GetLocation();
            Latitude = location.randomLatitude;
            Longitude = location.randomLongitude;
            _timer = new System.Timers.Timer();
            if (type == typeof(StaticSensor))
            {
                _timer.Interval = 10000;
            }
            else if (type == typeof(PortableSensor))
            {
                _timer.Interval = 500;
            }
            else
            {
                throw new InvalidCastException("Provided sensor is not portable or static");
            }
            _timer.Elapsed += async (s, e) =>
            {
                try
                {
                    var data = GetSensorData();
                    var response = await _httpClient.PostAsync($"http://{_serverIp}/api/integration", new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
                }
                catch (Exception ex)
                {

                }
            };
        }
        public Type SensorType { get; private set; }

        public bool IsPowerOn => _timer.Enabled;

        public string ApiKey => _apiKey;

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public void PowerOn()
        {
            _timer.Start();
        }

        public void PowerOff()
        {
            _timer.Stop();
        }

        public string GetGuid()
        {
            return _sensorGuid;
        }

        #region Private

        #region Helpers

        private SensorDataModel GetSensorData()
        {
            return new SensorDataModel
            {
                CO2 = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                LPG = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                CO = (float)Math.Round((float)_emulatorRandom.NextDouble() * 4, 3),
                CH4 = (float)Math.Round((float)_emulatorRandom.NextDouble() * 0.716, 3),
                Dust = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                Temp = (float)Math.Round((float)_emulatorRandom.NextDouble() * 40, 3),
                Preassure = (float)Math.Round((float)_emulatorRandom.NextDouble() * 20, 3),
                Hum = (float)Math.Round((float)_emulatorRandom.NextDouble() * 40, 3),
                Created = DateTime.UtcNow - new TimeSpan(0, 0, 10),
                Latitude = Latitude,
                Longitude = Longitude,
                ApiKey = _apiKey
            };
        }

        private static string SerializeJson(object json)
        {
            return JsonConvert.SerializeObject(json, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }



        private static (double randomLongitude, double randomLatitude) GetLocation()
        {

            double longitude = 53.904588;
            double latittude = 27.560597;
            int radius = 8000;
            Random random = _emulatorRandom;

            // Convert radius from meters to degrees
            double radiusInDegrees = radius / 111000f;

            double u = random.NextDouble();
            double v = random.NextDouble();
            double w = radiusInDegrees * Math.Sqrt(u);
            double t = 2 * Math.PI * v;
            double x = w * Math.Cos(t);
            double y = w * Math.Sin(t);

            // Adjust the x-coordinate for the shrinking of the east-west distances
            double new_x = x / Math.Cos((Math.PI / 180) * latittude);

            double foundLongitude = new_x + longitude;
            double foundLatitude = y + latittude;
            return (foundLatitude, foundLongitude);
        }
    }
    #endregion

    #endregion
}