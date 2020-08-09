using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.DTO;
using Web.Application.Readings.Exceptions;
using Web.Application.Readings.Notifications;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Application.Readings.Commands
{
    public class CreateReadingCommandHandler : IRequestHandler<CreateReadingCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        private static readonly IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<SensorReadingDTO, StaticSensorReading>();
            x.CreateMap<SensorReadingDTO, PortableSensorReading>();
        }));

        public CreateReadingCommandHandler(IMediator mediator, IDataContextFactory<DataContext> dataContextFactory)
        {
            _mediator = mediator;
            _dataContextFactory = dataContextFactory;
        }

        public async Task<bool> Handle(CreateReadingCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = await context.Sensors
                .FirstOrDefaultAsync(x => x.ApiKey == request.ApiKey, cancellationToken);
            switch (sensor)
            {
                case null:
                    throw new SensorNotFoundException("No Such Sensor");
                case PortableSensor _:
                    await _mediator.Publish(
                        new PortableReadingCreatedNotification(sensor.Id,
                            _mapper.Map<SensorReadingDTO, PortableSensorReading>(request.Reading)), cancellationToken);
                    break;
                case StaticSensor staticSensor:
                {
                    var reading = _mapper.Map<SensorReadingDTO, StaticSensorReading>(request.Reading);
                    reading.StaticSensorId = sensor.Id;

                    await context.Set<Reading>().AddAsync(reading, cancellationToken);

                    await _mediator.Publish(new StaticSensorReadingCreatedNotification(sensor.Id, reading),
                        cancellationToken);
                    break;
                }
            }

            return true;
        }
    }
}