using System;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Hubs;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Models.Hub;

namespace Web.Areas.PWA.Helpers.Implementations
{
    public class PWASignalrDispatchHelper : IPWADispatchHelper
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<PWAStaticSensorHub, IPWAStaticSensorClient> _staticSensorHubContext;

        public PWASignalrDispatchHelper(
            IHubContext<PWAStaticSensorHub, IPWAStaticSensorClient> staticSensorHubContext, IMapper mapper)
        {
            _staticSensorHubContext = staticSensorHubContext ?? throw new ArgumentNullException(nameof(staticSensorHubContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public void DispatchReadingsForStaticSensor(int sensorId, PollutionLevel pollutionLevel, Reading reading)
        {
            _staticSensorHubContext.Clients.All.DispatchReading(new StaticSensorReadingDispatchModel
            {
                PollutionLevel = pollutionLevel,
                Reading = _mapper.Map<Reading, ReadingDispatchModel>(reading),
                SensorId = sensorId
            });
        }
    }
}
