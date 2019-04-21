using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Data;
using Web.Data.Models;
using Web.Enums;
using Web.Helpers.Interfaces;
using Web.SensorActions.Input;
using Web.SensorActions.Output;
using WebSocketSharp;

namespace Web.Helpers.Implementations
{
    public class SensorWebSocketConnectionHelper : ISensorConnectionHelper
    {

        private static Dictionary<int, WebSocket> Connections { get; set; } = new Dictionary<int, WebSocket>();
        private static Dictionary<int, System.Timers.Timer> Timers { get; set; } = new Dictionary<int, System.Timers.Timer>();
        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<PushReadingsActionPayload, PortableSensorReading>();
            x.CreateMap<PushReadingsActionPayload, StaticSensorReading>();
        }));

        private readonly IRepository _repository;
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly ILogger _logger;
        private readonly IAdminDispatchHelper _adminDispatchHelper;
        private readonly IPWADispatchHelper _pwaDispatchHelper;

        public SensorWebSocketConnectionHelper(
            IRepository repository,
            ILoggerFactory loggerFactory,
            ISensorCacheHelper sensorCacheHelper,
            IAdminDispatchHelper adminDispatchHelper,
            IPWADispatchHelper pwaDispatchHelper)
        {
            _repository = repository;
            _sensorCacheHelper = sensorCacheHelper;
            _adminDispatchHelper = adminDispatchHelper;
            _pwaDispatchHelper = pwaDispatchHelper;
            _logger = loggerFactory.CreateLogger<SensorWebSocketConnectionHelper>();
        }

        public async Task ConnectAllSensorsAsync()
        {
            var sensors = await _repository.GetSensorsAsync();
            foreach (var sensor in sensors)
            {
                try
                {
                    ConnectSensor(sensor);
                }
                catch (InvalidOperationException)
                {

                }
            }
        }

        public void ConnectSensor(Sensor sensor)
        {
            WebSocket ws = null;
            if (Connections.ContainsKey(sensor.Id))
            {
                ws = Connections[sensor.Id];
                if (ws != null)
                {
                    if (ws.ReadyState == WebSocketState.Open)
                    {
                        _logger.LogInformation("Попытка подключить уже подключенный датчик!");
                        return;
                    }
                    ws.Close();
                }
            }
            ws = new WebSocket($"ws://{sensor.IPAddress}");
            ws.OnMessage += async (sender, e) =>
            {
                //TODO: check sensor state here
                var scopedRepository = Startup.ServiceProvider().GetService(typeof(IRepository)) as IRepository;
                dynamic json = JsonConvert.DeserializeObject(e.Data);
                var sensorState = await scopedRepository.GetSensorByIdAsync(sensor.Id);
                if (sensorState is StaticSensor)
                {
                    var staticSensorState = (StaticSensor)sensorState;
                    if (json["type"] == InputSensorActionType.PushReadings)
                    {
                        var payload = JsonConvert.DeserializeObject<PushReadingsActionPayload>(JsonConvert.SerializeObject(json["payload"]));
                        StaticSensorReading staticSensorReading = _mapper.Map<PushReadingsActionPayload, StaticSensorReading>(payload);
                        staticSensorReading.StaticSensorId = sensor.Id;
                        scopedRepository = Startup.ServiceProvider().GetService(typeof(IRepository)) as IRepository;
                        await scopedRepository.AddReadingAsync(staticSensorReading);
                        if (staticSensorState.IsAvailable())
                        {
                            await _sensorCacheHelper.UpdateSensorCacheWithReadingAsync(staticSensorReading);
                            var pollutionLevel = await _sensorCacheHelper.GetPollutionLevelAsync(sensor.Id);
                            _pwaDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, pollutionLevel, staticSensorReading);
                        }
                        _adminDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, staticSensorReading);
                    }
                }
                else if (sensorState is PortableSensor)
                {
                    if (json.type == InputSensorActionType.PushReadings)
                    {
                        var payload = JsonConvert.DeserializeObject<PushReadingsActionPayload>(JsonConvert.SerializeObject(json["payload"]));
                        PortableSensorReading portableSensorReading = _mapper.Map<PushReadingsActionPayload, PortableSensorReading>(payload);
                        _adminDispatchHelper.DispatchReadingsForPortableSensor(sensor.Id, portableSensorReading);
                    }
                    if (json.type == InputSensorActionType.PushCoordinates)
                    {
                        var payload = JsonConvert.DeserializeObject<PushCoordinatesActionPayload>(JsonConvert.SerializeObject(json["payload"]));
                        _adminDispatchHelper.DispatchCoordinatesForPortableSensor(sensor.Id, payload.Latitude, payload.Longitude);
                    }
                }
            };
            ws.OnClose += (sender, e) =>
            {
                _logger.LogInformation($"Датчик с id {sensor.Id} был отключен!");
            };
            ws.OnError += (sender, e) =>
            {
                _logger.LogError(e.Exception, $"Произошла ошибка при работе с датчиком с id {sensor.Id}");

            };
            ws.Connect();
            if (ws.ReadyState != WebSocketState.Open)
            {
                throw new InvalidOperationException("Датчик не включен");
            }
            Connections[sensor.Id] = ws;
            TriggerChangeState(sensor);
        }

        public void DisconnectSensor(int id)
        {
            DisconnectWebSocket(id);
            StopTimer(id);
        }

        public void DisconnectAllSensors()
        {
            var keysC = Connections.Select(f => f.Key).ToList();
            foreach (var key in keysC)
            {
                DisconnectWebSocket(key);
            }
            var keysT = Timers.Select(f => f.Key).ToList();
            foreach (var key in keysC)
            {
                StopTimer(key);
            }
        }

        public bool IsConnected(int id)
        {
            if (!Connections.ContainsKey(id))
            {
                return false;
            }
            var webSocket = Connections[id];
            if (webSocket == null)
            {
                return false;
            }
            if (webSocket.ReadyState == WebSocketState.Open)
            {
                return true;
            }
            return false;
        }


        public void TriggerChangeState(Sensor sensor)
        {
            if (Timers.ContainsKey(sensor.Id))
            {
                Timers[sensor.Id].Stop();
            }
            Timers[sensor.Id] = new Timer();
            if (!sensor.IsActive || sensor.IsDeleted)
            {
                return;
            }
            if (sensor is PortableSensor)
            {
                Timers[sensor.Id].Interval = 500;
            }
            else if (sensor is StaticSensor)
            {
                Timers[sensor.Id].Interval = 10000;
            }
            Timers[sensor.Id].Elapsed += (sender, e) =>
            {
                if (Connections.ContainsKey(sensor.Id))
                {
                    Connections[sensor.Id].Send(SerializeJson(new PullReadingsAction(new PullReadingsActionPayload())));
                    if (sensor is PortableSensor)
                    {
                        Connections[sensor.Id].Send(SerializeJson(new PullCoordinatesAction(new PullCoordinatesActionPayload())));
                    }
                }
            };
            Timers[sensor.Id].Start();
        }

        private void DisconnectWebSocket(int id)
        {
            if (!Connections.ContainsKey(id))
            {
                return;
            }
            var webSocket = Connections[id];
            if (webSocket == null)
            {
                return;
            }
            if (webSocket.ReadyState != WebSocketState.Closed)
            {
                webSocket.Close();
            }
            Connections.Remove(id);
        }

        private void StopTimer(int id)
        {
            if (!Timers.ContainsKey(id))
            {
                return;
            }
            var timer = Timers[id];
            if (timer == null)
            {
                return;
            }
            if (timer.Enabled)
            {
                timer.Stop();
            }
            Timers.Remove(id);
        }

        private string SerializeJson(object json)
        {
            return JsonConvert.SerializeObject(json, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}