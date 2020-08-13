using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Application.Readings.Notifications
{
    public class StaticSensorReadingCreatedNotificationHandler : INotificationHandler<StaticSensorReadingCreatedNotification>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly ISensorCacheHelper _sensorCacheHelper;

        public StaticSensorReadingCreatedNotificationHandler(IDataContextFactory<DataContext> dataContextFactory, ISensorCacheHelper sensorCacheHelper)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _sensorCacheHelper = sensorCacheHelper ??  throw new ArgumentNullException(nameof(sensorCacheHelper));
        }

        public async Task Handle(StaticSensorReadingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var staticSensor = await context.Set<StaticSensor>().AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == notification.SensorId, cancellationToken);
            if (staticSensor.IsAvailable())
                await _sensorCacheHelper.UpdateSensorCacheWithReadingAsync(notification.Reading);
        }
    }
}