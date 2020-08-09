using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Domain.Entities;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class CreateStaticSensorCommandHandler : IRequestHandler<CreateStaticSensorCommand, bool>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;

        public CreateStaticSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task<bool> Handle(CreateStaticSensorCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = new StaticSensor
            {
                Readings = new List<StaticSensorReading>(),
                ApiKey = request.ApiKey,
            };
            sensor.Latitude = request.Latitude;
            sensor.Longitude = request.Longitude;
                
            await context.StaticSensors.AddAsync(sensor, cancellationToken);
                
            await context.SaveChangesAsync(cancellationToken);
                
            return true;
        }
    }
}