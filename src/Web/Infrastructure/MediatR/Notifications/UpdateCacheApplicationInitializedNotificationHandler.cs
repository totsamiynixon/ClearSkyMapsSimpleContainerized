using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Application.Notifications;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Infrastructure.MediatR.Notifications
{
    public class
        UpdateCacheApplicationInitializedNotificationHandler : INotificationHandler<ApplicationInitializedNotification>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly ISensorCacheHelper _sensorCacheHelper;

        public UpdateCacheApplicationInitializedNotificationHandler(IDataContextFactory<DataContext> dataContextFactory,
            ISensorCacheHelper sensorCacheHelper)
        {
            _dataContextFactory = dataContextFactory;
            _sensorCacheHelper = sensorCacheHelper;
        }

        public async Task Handle(ApplicationInitializedNotification notification, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();

            var sensorsMap = await context.StaticSensors
                .Where(z => z.IsActive && z.IsVisible && !z.IsDeleted)
                .Select(x => new
                {
                    sensor = x,
                    readings = x.Readings.OrderByDescending(z => z.Created).Take(10)
                })
                .ToListAsync(cancellationToken);

            sensorsMap.ForEach(z => z.sensor.Readings = z.readings.ToList());

            var sensors = sensorsMap.Select(x => x.sensor).ToList();

            foreach (var sensor in sensors)
            {
                await _sensorCacheHelper.UpdateStaticSensorCacheAsync(sensor);
            }
        }
    }
}