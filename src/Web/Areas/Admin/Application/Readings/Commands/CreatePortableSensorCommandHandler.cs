using System;
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
    public class CreatePortableSensorCommandHandler : IRequestHandler<CreatePortableSensorCommand, PortableSensorDTO>
    {
        private readonly IMapper _mapper;
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        
        public CreatePortableSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory, IMapper mapper)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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