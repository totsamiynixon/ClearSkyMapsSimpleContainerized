using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Infrastructure.Data.Factory;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;

namespace Web.Areas.Admin.Application.Emulation.Notifications
{
    public class EmulationStoppedNotificationHandler : INotificationHandler<EmulationStoppedNotification>
    {
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IEmulationDataContextFactory<DataContext> _emulationDataContextFactory;

        public EmulationStoppedNotificationHandler(ISensorCacheHelper sensorCacheHelper, IEmulationDataContextFactory<DataContext> emulationDataContextFactory)
        {
            _sensorCacheHelper = sensorCacheHelper ?? throw new ArgumentNullException(nameof(sensorCacheHelper));
            _emulationDataContextFactory = emulationDataContextFactory ?? throw new ArgumentNullException(nameof(emulationDataContextFactory));
        }

        public async Task Handle(EmulationStoppedNotification notification, CancellationToken cancellationToken)
        {
            _sensorCacheHelper.ClearCache();

            await using var context = _emulationDataContextFactory.Create();
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }
}