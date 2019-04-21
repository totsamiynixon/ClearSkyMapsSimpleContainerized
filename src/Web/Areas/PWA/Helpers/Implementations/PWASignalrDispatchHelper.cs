using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Areas.PWA.Hubs;
using Web.Data.Models;
using Web.Enums;
using Web.Models.Hub;

namespace Web.Areas.PWA.Helpers.Implementations
{
    public class PWASignalrDispatchHelper : IPWADispatchHelper
    {
        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
       {
           x.CreateMap<Reading, ReadingDispatchModel>();
       }));

        private readonly IHubContext<PWAStaticSensorHub, IPWAStaticSensorClient> _staticSensorHubContext;

        public PWASignalrDispatchHelper(
            IHubContext<PWAStaticSensorHub, IPWAStaticSensorClient> staticSensorHubContext)
        {
            _staticSensorHubContext = staticSensorHubContext;
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
