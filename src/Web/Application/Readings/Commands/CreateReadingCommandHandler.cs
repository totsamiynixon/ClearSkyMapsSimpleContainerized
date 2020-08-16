using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.Commands.DTO;
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

        private readonly IMapper _mapper;

        public CreateReadingCommandHandler(IMediator mediator, IDataContextFactory<DataContext> dataContextFactory, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));;
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

                    await context.StaticSensorReadings.AddAsync(reading, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);

                    await _mediator.Publish(new StaticSensorReadingCreatedNotification(sensor.Id, reading),
                        cancellationToken);
                    break;
                }
            }

            return true;
        }
    }
}