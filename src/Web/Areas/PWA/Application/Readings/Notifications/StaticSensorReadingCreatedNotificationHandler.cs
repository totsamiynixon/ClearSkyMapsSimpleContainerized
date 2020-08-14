using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.Notifications;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.PWA.Application.Readings.Notifications
{
    public class
        StaticSensorReadingCreatedNotificationHandler : INotificationHandler<StaticSensorReadingCreatedNotification>
    {
        private readonly IPWADispatchHelper _pwaDispatchHelper;
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IMapper _mapper;

        public StaticSensorReadingCreatedNotificationHandler(IPWADispatchHelper pwaDispatchHelper,
            ISensorCacheHelper sensorCacheHelper, IDataContextFactory<DataContext> dataContextFactory, IMapper mapper)
        {
            _pwaDispatchHelper = pwaDispatchHelper ?? throw new ArgumentNullException(nameof(pwaDispatchHelper));
            _sensorCacheHelper = sensorCacheHelper ?? throw new ArgumentNullException(nameof(sensorCacheHelper));
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Handle(StaticSensorReadingCreatedNotification notification,
            CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = await context.StaticSensors.AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == notification.SensorId, cancellationToken);
            if (sensor.IsAvailable())
            {
                var pollutionLevel = await _sensorCacheHelper.GetPollutionLevelAsync(sensor.Id);
                _pwaDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, pollutionLevel, notification.Reading);
            }
        }
    }
}