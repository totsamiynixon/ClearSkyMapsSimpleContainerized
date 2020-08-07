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
    public class StaticSensorReadingCreatedNotificationHandler : INotificationHandler<StaticSensorReadingCreatedNotification>
    {
        private readonly IAdminDispatchHelper _adminDispatchHelper;
                
        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, StaticSensorReading>();
        }));
        public StaticSensorReadingCreatedNotificationHandler(IAdminDispatchHelper adminDispatchHelper)
        {
            _adminDispatchHelper = adminDispatchHelper;
        }
        public async Task Handle(StaticSensorReadingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _adminDispatchHelper.DispatchReadingsForStaticSensorAsync(notification.SensorId,
                _mapper.Map<SensorReadingDTO, StaticSensorReading>(notification.Reading));
        }
    }
}