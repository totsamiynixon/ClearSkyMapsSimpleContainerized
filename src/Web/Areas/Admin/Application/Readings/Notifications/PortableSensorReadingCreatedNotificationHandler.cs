using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Web.Application.Readings.DTO;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Domain.Entities;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class PortableSensorReadingCreatedNotificationHandler : INotificationHandler<PortableReadingCreatedNotification>
    {
        
        private readonly IAdminDispatchHelper _adminDispatchHelper;
        
        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, PortableSensorReading>();
        }));

        public PortableSensorReadingCreatedNotificationHandler(IAdminDispatchHelper adminDispatchHelper)
        {
            _adminDispatchHelper = adminDispatchHelper;
        }
        public async Task Handle(PortableReadingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _adminDispatchHelper.DispatchReadingsForPortableSensorAsync(notification.SensorId, _mapper.Map<SensorReadingDTO, PortableSensorReading>(notification.Reading));
            await _adminDispatchHelper.DispatchCoordinatesForPortableSensorAsync(notification.SensorId, notification.Reading.Latitude, notification.Reading.Longitude);
        }
    }
}