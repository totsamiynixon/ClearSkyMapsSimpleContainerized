using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Helpers.Interfaces;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class SensorDeletedNotificationHandler : INotificationHandler<SensorDeletedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;

        public SensorDeletedNotificationHandler(ISensorCacheHelper sensorCacheHelper)
        {
            _sensorCacheHelper = sensorCacheHelper ?? throw new ArgumentNullException(nameof(sensorCacheHelper));
        }

        public async Task Handle(SensorDeletedNotification notification, CancellationToken cancellationToken)
        {
            await _sensorCacheHelper.RemoveStaticSensorFromCacheAsync(notification.SensorId);
        }
    }
}