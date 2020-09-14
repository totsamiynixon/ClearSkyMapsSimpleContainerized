using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Infrastructure.Hubs;
using Web.Areas.Admin.Models.Hub;
using Web.Domain.Entities;
using Web.Models.Hub;

namespace Web.Areas.Admin.Helpers.Implementations
{
    public class AdminSignalRHubDispatchHelper : IAdminDispatchHelper
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<AdminPortableSensorHub, IAdminPortableSensorClient> _portableSensorHubContext;
        private readonly IHubContext<AdminStaticSensorHub, IAdminStaticSensorClient> _staticSensorHubContext;

        public AdminSignalRHubDispatchHelper(
            IHubContext<AdminPortableSensorHub, IAdminPortableSensorClient> portableSensorHubContext,
            IHubContext<AdminStaticSensorHub, IAdminStaticSensorClient> staticSensorHubContext, IMapper mapper)
        {
            _portableSensorHubContext = portableSensorHubContext ?? throw new ArgumentNullException(nameof(portableSensorHubContext));
            _staticSensorHubContext = staticSensorHubContext ?? throw new ArgumentNullException(nameof(staticSensorHubContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task DispatchCoordinatesForPortableSensorAsync(int sensorId, double latitude, double longitude)
        {
            await _portableSensorHubContext.Clients.Group(AdminPortableSensorHub.PortableSensorGroup(sensorId)).DispatchCoordinatesAsync(new PortableSensorCoordinatesDispatchModel
            {
                Latitude = latitude,
                Longitude = longitude
            });
        }

        public async Task DispatchReadingsForPortableSensorAsync(int sensorId, Reading reading)
        {
           await _portableSensorHubContext.Clients.Group(AdminPortableSensorHub.PortableSensorGroup(sensorId)).DispatchReading(_mapper.Map<Reading, PortableSensorReadingsDispatchModel>(reading));
        }

        public async Task DispatchReadingsForStaticSensorAsync(int sensorId, Reading reading)
        {
            await _staticSensorHubContext.Clients.All.DispatchReading(_mapper.Map<Reading, StaticSensorReadingDispatchModel>(reading));
        }
    }
}
