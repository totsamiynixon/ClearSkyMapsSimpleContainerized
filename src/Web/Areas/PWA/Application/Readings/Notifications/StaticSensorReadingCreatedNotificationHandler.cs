using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Web.Application.Readings.DTO;
using Web.Application.Readings.Notifications;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data.Repository;

namespace Web.Areas.PWA.Application.Readings.Notifications
{
    public class
        StaticSensorReadingCreatedNotificationHandler : INotificationHandler<StaticSensorReadingCreatedNotification>
    {
        private readonly IPWADispatchHelper _pwaDispatchHelper;
        private readonly IRepository _repository;
        private readonly ISensorCacheHelper _sensorCacheHelper;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, StaticSensorReading>();
        }));

        public StaticSensorReadingCreatedNotificationHandler(IPWADispatchHelper pwaDispatchHelper,
            IRepository repository, ISensorCacheHelper sensorCacheHelper)
        {
            _pwaDispatchHelper = pwaDispatchHelper;
            _repository = repository;
            _sensorCacheHelper = sensorCacheHelper;
        }

        public async Task Handle(StaticSensorReadingCreatedNotification notification,
            CancellationToken cancellationToken)
        {
            var sensor = await _repository.GetSensorByIdAsync(notification.SensorId);
            if (sensor is StaticSensor staticSensor && staticSensor.IsAvailable())
            {
                var pollutionLevel = await _sensorCacheHelper.GetPollutionLevelAsync(sensor.Id);
                _pwaDispatchHelper.DispatchReadingsForStaticSensor(sensor.Id, pollutionLevel, notification.Reading);
            }
        }
    }
}