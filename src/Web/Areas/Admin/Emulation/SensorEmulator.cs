using Fleck;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Web.Enums;
using Web.Helpers;
using Web.SensorActions.Input;
using Web.SensorActions.Output;
using WebSocketSharp;

namespace Web.Areas.Admin.Emulation
{
    public class SensorEmulator
    {
        private readonly string _sensorGuid;
        private readonly string _ip;
        private readonly string _port;
        private (double latitude, double longitude)? _location;
        private IWebSocketConnection _connection;

        private IMemoryCache MemCache => (Startup.ServiceProvider().GetService(typeof(IMemoryCache)) as IMemoryCache);

        private static readonly Random _emulatorRandom = new Random();


        public SensorEmulator(string ip, string port, string guid)
        {
            _sensorGuid = guid;
            _ip = ip;
            _port = port;
        }


        public SensorState State { get; private set; }

        public bool IsPowerOn { get; private set; }

        public void PowerOn()
        {
            RestoreState();
            InitializeWebSockets();
            IsPowerOn = true;
        }

        public void PowerOff()
        {

            State = null;
            if (_connection != null)
            {
                _connection.Close();
            }
            IsPowerOn = false;
        }

        public string GetGuid()
        {
            return _sensorGuid;
        }

        public string GetIp()
        {
            return _ip;
        }

        public string GetPort()
        {
            return _port;
        }

        public (double latitude, double longitude) GetCoordinates()
        {
            if (!_location.HasValue)
            {
                _location = GetLocation();
            }
            return _location.Value;
        }

        #region Private


        private void InitializeWebSockets()
        {
            var server = new WebSocketServer($"ws://{_ip}:{_port}");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {

                };
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = async (data) => await HandleMessageAsync(data, socket);
            });
        }

        private void RestoreState()
        {
            State = MemCache.Get<SensorState>(_sensorGuid) ?? new SensorState();
            if (!_location.HasValue)
            {
                _location = GetLocation();
            }
            State.Latitude = _location.Value.latitude;
            State.Longitude = _location.Value.longitude;
        }

        private async Task HandleMessageAsync(string data, IWebSocketConnection socket)
        {
            dynamic json = JsonConvert.DeserializeObject(data);
            if (json.type == OuputSensorActionType.PushState)
            {
                SetState(JsonConvert.DeserializeObject<PushStateActionPayload>(JsonConvert.SerializeObject(json.payload)));
            }
            if (json.type == OuputSensorActionType.PullReadings)
            {
                var action = GetPushReadingsAction();
                await socket.Send(SerializeJson(action));
                return;
            }
            if (json.type == OuputSensorActionType.PullCoordinates)
            {
                var action = GetPushCoordinatesAction();
                await socket.Send(SerializeJson(action));
            }

        }

        private void SetState(PushStateActionPayload payload)
        {
            State.IsActive = payload.IsActive;
            MemCache.Set(_sensorGuid, State);
        }

        #region Helpers

        private PushReadingsAction GetPushReadingsAction()
        {
            var action = new PushReadingsAction(new PushReadingsActionPayload
            {
                CO2 = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                LPG = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                CO = (float)Math.Round((float)_emulatorRandom.NextDouble() * 4, 3),
                CH4 = (float)Math.Round((float)_emulatorRandom.NextDouble() * 0.716, 3),
                Dust = (float)Math.Round((float)_emulatorRandom.NextDouble() * 350, 3),
                Temp = (float)Math.Round((float)_emulatorRandom.NextDouble() * 40, 3),
                Preassure = (float)Math.Round((float)_emulatorRandom.NextDouble() * 20, 3),
                Hum = (float)Math.Round((float)_emulatorRandom.NextDouble() * 40, 3),
                Created = DateTime.UtcNow - new TimeSpan(0, 0, 10)
            });
            return action;
        }

        private PushCoordinatesAction GetPushCoordinatesAction()
        {
            var action = new PushCoordinatesAction(new PushCoordinatesActionPayload
            {
                Latitude = _location.Value.latitude,
                Longitude = _location.Value.longitude
            });
            return action;
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
            return (foundLongitude, foundLatitude);
        }
    }
    #endregion

    #endregion
}