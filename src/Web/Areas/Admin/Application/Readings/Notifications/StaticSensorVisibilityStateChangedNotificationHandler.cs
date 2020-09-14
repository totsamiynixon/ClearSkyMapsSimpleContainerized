using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class
        StaticSensorVisibilityStateChangedNotificationHandler : INotificationHandler<
            StaticSensorVisibilityStateChangedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public StaticSensorVisibilityStateChangedNotificationHandler(ISensorCacheHelper sensorCacheHelper,
            IDataContextFactory<DataContext> dataContextFactory)
        {
            _sensorCacheHelper = sensorCacheHelper ?? throw new ArgumentNullException(nameof(sensorCacheHelper));
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        public async Task Handle(StaticSensorVisibilityStateChangedNotification notification,
            CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensorQuery =
                await context.StaticSensors.AsNoTracking()
                    .Where(z => z.Id == notification.SensorId)
                    .Select(x => new
                    {
                        sensor = x,
                        readings = x.Readings
                            .OrderByDescending(z => z.Created)
                            .Take(10)
                            .ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken);

            if (sensorQuery == null)
            {
                throw new SensorNotFoundException(notification.SensorId);
            }

            sensorQuery.sensor.Readings = sensorQuery.readings;

            await _sensorCacheHelper.UpdateStaticSensorCacheAsync(sensorQuery.sensor);
        }
    }
}