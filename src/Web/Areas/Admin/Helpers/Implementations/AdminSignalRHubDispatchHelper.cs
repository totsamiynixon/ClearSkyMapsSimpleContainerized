using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.Admin.Hubs;
using Web.Areas.Admin.Models.Hub;
using Web.Data.Models;
using Web.Models.Hub;

namespace Web.Areas.Admin.Helpers.Implementations
{
    public class AdminSignalRHubDispatchHelper : IAdminDispatchHelper
    {
        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Reading, StaticSensorReadingDispatchModel>();
        }));

        private readonly IHubContext<AdminPortableSensorHub, IAdminPortableSensorClient> _portableSensorHubContext;
        private readonly IHubContext<AdminStaticSensorHub, IAdminStaticSensorClient> _staticSensorHubContext;

        public AdminSignalRHubDispatchHelper(
            IHubContext<AdminPortableSensorHub, IAdminPortableSensorClient> portableSensorHubContext,
            IHubContext<AdminStaticSensorHub, IAdminStaticSensorClient> staticSensorHubContext)
        {
            _portableSensorHubContext = portableSensorHubContext;
            _staticSensorHubContext = staticSensorHubContext;
        }

        public void DispatchCoordinatesForPortableSensor(int sensorId, double latitude, double longitude)
        {
            _portableSensorHubContext.Clients.Group(AdminPortableSensorHub.PortableSensorGroup(sensorId)).DispatchCoordinates(new PortableSensorCoordinatesDispatchModel
            {
                Latitude = latitude,
                Longitude = longitude
            });
        }

        public void DispatchReadingsForPortableSensor(int sensorId, Reading reading)
        {
            _portableSensorHubContext.Clients.Group(AdminPortableSensorHub.PortableSensorGroup(sensorId)).DispatchReading(_mapper.Map<Reading, PortableSensorReadingsDispatchModel>(reading));
        }

        public void DispatchReadingsForStaticSensor(int sensorId, Reading reading)
        {
            _staticSensorHubContext.Clients.All.DispatchReading(_mapper.Map<Reading, StaticSensorReadingDispatchModel>(reading));
        }
    }
}
