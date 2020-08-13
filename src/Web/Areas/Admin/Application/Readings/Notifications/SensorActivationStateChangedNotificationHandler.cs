using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
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
            _sensorCacheHelper = sensorCacheHelper ?? throw new ArgumentNullException(nameof(sensorCacheHelper));
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        public async Task Handle(SensorActivationStateChangedNotification notification,
            CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor =
                await context.Sensors.AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == notification.SensorId, cancellationToken);

            if (sensor == null)
            {
                throw new SensorNotFoundException(notification.SensorId);
            }
            
            if (sensor is StaticSensor staticSensor)
            {
                staticSensor.Readings = await context.StaticSensorReadings
                    .Where(z => z.StaticSensorId == staticSensor.Id)
                    .OrderByDescending(z => z.Created)
                    .Take(10)
                    .ToListAsync(cancellationToken);
                
                await _sensorCacheHelper.UpdateStaticSensorCacheAsync(staticSensor);
            }
        }
    }
}