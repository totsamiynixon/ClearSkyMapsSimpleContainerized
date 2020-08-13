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
    public class CreatePortableSensorCommandHandler : IRequestHandler<CreatePortableSensorCommand, PortableSensorDTO>
    {
        private static IMapper _mapper = new Mapper(new MapperConfiguration(x =>
        {
            x.CreateMap<Sensor, SensorDTO>();
            x.CreateMap<PortableSensor, PortableSensorDTO>()
                .IncludeBase<Sensor, SensorDTO>();
        }));
        
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        
        public CreatePortableSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<PortableSensorDTO> Handle(CreatePortableSensorCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = new PortableSensor
            {
                ApiKey = request.ApiKey
            };
            await context.PortableSensors.AddAsync(sensor, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
                
            return _mapper.Map<PortableSensor, PortableSensorDTO>(sensor);
        }
    }
}