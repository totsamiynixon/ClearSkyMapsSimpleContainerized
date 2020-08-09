using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class SensorActivationStateChangedNotificationHandler : INotificationHandler<SensorActivationStateChangedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public SensorActivationStateChangedNotificationHandler(ISensorCacheHelper sensorCacheHelper, IDataContextFactory<DataContext> dataContextFactory)
        {
            _sensorCacheHelper = sensorCacheHelper;
            _dataContextFactory = dataContextFactory;
        }

        public async Task Handle(SensorActivationStateChangedNotification notification,
            CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor =
                await context.Sensors.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == notification.SensorId, cancellationToken);

            if (sensor is StaticSensor staticSensor)
            {
                await _sensorCacheHelper.UpdateStaticSensorCacheAsync(staticSensor);
            }
        }
    }
}