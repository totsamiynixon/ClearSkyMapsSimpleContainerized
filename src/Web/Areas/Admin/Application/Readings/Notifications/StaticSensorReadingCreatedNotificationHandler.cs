using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class
        StaticSensorReadingCreatedNotificationHandler : INotificationHandler<StaticSensorReadingCreatedNotification>
    {
        private readonly IAdminDispatchHelper _adminDispatchHelper;

        public StaticSensorReadingCreatedNotificationHandler(IAdminDispatchHelper adminDispatchHelper)
        {
            _adminDispatchHelper = adminDispatchHelper ?? throw new ArgumentNullException(nameof(adminDispatchHelper));
        }

        public async Task Handle(StaticSensorReadingCreatedNotification notification,
            CancellationToken cancellationToken)
        {
            await _adminDispatchHelper.DispatchReadingsForStaticSensorAsync(notification.SensorId,
                notification.Reading);
        }
    }
}