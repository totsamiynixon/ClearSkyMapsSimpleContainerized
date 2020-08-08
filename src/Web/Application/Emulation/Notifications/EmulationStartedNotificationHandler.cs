using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;
using Web.Infrastructure.Data.Initialize;

namespace Web.Application.Emulation.Notifications
{
    public class EmulationStartedNotificationHandler : INotificationHandler<EmulationStartedNotification>
    {
        private readonly IApplicationDatabaseInitializer _applicationDatabaseInitializer;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly ISensorCacheHelper _sensorCacheHelper;

        public EmulationStartedNotificationHandler(IApplicationDatabaseInitializer applicationDatabaseInitializer, IDataContextFactory<DataContext> dataContextFactory, ISensorCacheHelper sensorCacheHelper)
        {
            _applicationDatabaseInitializer = applicationDatabaseInitializer;
            _dataContextFactory = dataContextFactory;
            _sensorCacheHelper = sensorCacheHelper;
        }

        //TODO: check this code
        public async Task Handle(EmulationStartedNotification notification, CancellationToken cancellationToken)
        {
            _sensorCacheHelper.ClearCache();
            await _applicationDatabaseInitializer.InitializeDbAsync();
            
            await using var context = _dataContextFactory.Create();
            context.Sensors.RemoveRange(context.Sensors);
            await context.SaveChangesAsync(cancellationToken);

            foreach (var device in notification.Emulator.Devices)
            {
                if (device.SensorType == typeof(StaticSensor))
                {
                    context.Add(new StaticSensor
                    {
                        ApiKey = device.ApiKey,
                        Latitude = device.Latitude,
                        Longitude = device.Latitude
                    });
                }

                if (device.SensorType == typeof(PortableSensor))
                {
                    context.Add(new PortableSensor
                    {
                        ApiKey = device.ApiKey
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}