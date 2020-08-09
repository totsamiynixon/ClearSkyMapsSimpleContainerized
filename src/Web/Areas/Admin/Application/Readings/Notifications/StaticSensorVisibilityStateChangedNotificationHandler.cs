using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class
        StaticSensorVisibilityStateChangedNotificationHandler : INotificationHandler<StaticSensorVisibilityStateChangedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public StaticSensorVisibilityStateChangedNotificationHandler(ISensorCacheHelper sensorCacheHelper,
            IDataContextFactory<DataContext> dataContextFactory)
        {
            _sensorCacheHelper = sensorCacheHelper;
            _dataContextFactory = dataContextFactory;
        }

        public async Task Handle(StaticSensorVisibilityStateChangedNotification notification, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor =
                await context.StaticSensors.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == notification.SensorId, cancellationToken);

            await _sensorCacheHelper.UpdateStaticSensorCacheAsync(sensor);
        }
    }
}