using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Application.Emulation.Notifications
{
    public class EmulationStoppedNotificationHandler : INotificationHandler<EmulationStoppedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IEmulationDataContextFactory<DataContext> _emulationDataContextFactory;

        public EmulationStoppedNotificationHandler(ISensorCacheHelper sensorCacheHelper, IEmulationDataContextFactory<DataContext> emulationDataContextFactory)
        {
            _sensorCacheHelper = sensorCacheHelper;
            _emulationDataContextFactory = emulationDataContextFactory;
        }

        public async Task Handle(EmulationStoppedNotification notification, CancellationToken cancellationToken)
        {
            _sensorCacheHelper.ClearCache();
            
            using (var context = _emulationDataContextFactory.Create())
            {
                await context.Database.EnsureDeletedAsync(cancellationToken);
            }
        }
    }
}