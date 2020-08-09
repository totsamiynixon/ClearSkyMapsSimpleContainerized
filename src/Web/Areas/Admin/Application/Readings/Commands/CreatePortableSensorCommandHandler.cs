using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreatePortableSensorCommandHandler : IRequestHandler<CreatePortableSensorCommand, bool>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public CreatePortableSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<bool> Handle(CreatePortableSensorCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = new PortableSensor
            {
                ApiKey = request.ApiKey
            };
            await context.PortableSensors.AddAsync(sensor, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
                
            return true;
        }
    }
}