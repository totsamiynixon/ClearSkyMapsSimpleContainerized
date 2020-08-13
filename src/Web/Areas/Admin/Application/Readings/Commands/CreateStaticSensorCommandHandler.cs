using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Web.Areas.Admin.Application.Readings.DTO;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreateStaticSensorCommandHandler : IRequestHandler<CreateStaticSensorCommand, StaticSensorDTO>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Sensor, SensorDTO>();
            x.CreateMap<StaticSensor, StaticSensorDTO>()
                .IncludeBase<Sensor, SensorDTO>();
        }));

        public CreateStaticSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
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