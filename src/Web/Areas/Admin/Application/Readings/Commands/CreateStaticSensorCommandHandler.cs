using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Web.Areas.Admin.Application.Readings.Commands.DTO;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreateStaticSensorCommandHandler : IRequestHandler<CreateStaticSensorCommand, StaticSensorDTO>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IMapper _mapper;

        public CreateStaticSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory, IMapper mapper)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<StaticSensorDTO> Handle(CreateStaticSensorCommand request,
            CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = new StaticSensor
            {
                Readings = new List<StaticSensorReading>(),
                ApiKey = request.ApiKey,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
            };

            await context.StaticSensors.AddAsync(sensor, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<StaticSensor, StaticSensorDTO>(sensor);
        }
    }
}