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
        private readonly ISensorCacheHelper _sensorCacheHelper;
        private readonly IMediator _mediator;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, StaticSensorReading>();
            x.CreateMap<SensorReadingDTO, PortableSensorReading>();
        }));

        public CreateReadingCommandHandler(IRepository repository, ISensorCacheHelper sensorCacheHelper, IMediator mediator)
        {
            _repository = repository;
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
                await _mediator.Publish(new PortableReadingCreatedNotification(sensor.Id, _mapper.Map<SensorReadingDTO, PortableSensorReading>(request.Reading)), cancellationToken);
            }
            else if (sensor is StaticSensor staticSensor)
            {
                var reading = _mapper.Map<SensorReadingDTO, StaticSensorReading>(request.Reading);
                reading.StaticSensorId = sensor.Id;
                
                await _repository.AddReadingAsync(reading);

                await _mediator.Publish(new StaticSensorReadingCreatedNotification(sensor.Id, reading), cancellationToken);

            }

            return true;
        }
    }
}