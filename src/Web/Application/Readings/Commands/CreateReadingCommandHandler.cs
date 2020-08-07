using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Web.Application.Readings.DTO;
using Web.Application.Readings.Exceptions;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;
using Web.Areas.PWA.Helpers.Interfaces;
using Web.Domain.Entities;
using Web.Helpers.Interfaces;
using Web.Infrastructure.Data.Repository;

namespace Web.Application.Readings.Commands
{
    public class CreateReadingCommandHandler : IRequestHandler<CreateReadingCommand, bool>
    {
        private readonly IRepository _repository;
        private readonly IAdminDispatchHelper _adminDispatchHelper;
        private readonly IPWADispatchHelper _pwaDispatchHelper;
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IMediator _mediator;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, StaticSensorReading>();
        }));

        public CreateReadingCommandHandler(IRepository repository, IAdminDispatchHelper adminDispatchHelper,
            IPWADispatchHelper pwaDispatchHelper, ISensorCacheHelper sensorCacheHelper, IMediator mediator)
        {
            _repository = repository;
            _adminDispatchHelper = adminDispatchHelper;
            _pwaDispatchHelper = pwaDispatchHelper;
            _sensorCacheHelper = sensorCacheHelper;
            _mediator = mediator;
        }
        public async Task<bool> Handle(CreateReadingCommand request, CancellationToken cancellationToken)
        {
            var sensor = await _repository.GetSensorByApiKeyAsync(request.ApiKey);
            if (sensor == null)
            {
                throw new SensorNotFoundException("No Such Sensor");
            }

            if (sensor is PortableSensor)
            {
                await _mediator.Publish(new PortableReadingCreatedNotification(sensor.Id, request.Reading), cancellationToken);
            }
            else if (sensor is StaticSensor staticSensor)
            {
                var reading = _mapper.Map<SensorReadingDTO, StaticSensorReading>(request.Reading);
                reading.StaticSensorId = sensor.Id;
                
                await _repository.AddReadingAsync(reading);
                
                if (staticSensor.IsAvailable())
                    await _sensorCacheHelper.UpdateSensorCacheWithReadingAsync(reading);
                
                await _mediator.Publish(new StaticSensorReadingCreatedNotification(sensor.Id, request.Reading), cancellationToken);

            }

            return true;
        }
    }
}