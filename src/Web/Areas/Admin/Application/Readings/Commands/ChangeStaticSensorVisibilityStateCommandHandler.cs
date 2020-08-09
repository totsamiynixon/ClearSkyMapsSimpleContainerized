using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Application.Readings.Exceptions;
using Web.Areas.Admin.Application.Readings.Notifications;
using Web.Infrastructure.Data;
using Web.Infrastructure.Data.Factory;

namespace Web.Areas.Admin.Application.Readings.Commands
{
    public class ChangeStaticSensorVisibilityStateCommandHandler : IRequestHandler<ChangeStaticSensorVisibilityStateCommand, bool>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IMediator _mediator;

        public ChangeStaticSensorVisibilityStateCommandHandler(IDataContextFactory<DataContext> dataContextFactory, IMediator mediator)
        {
            _dataContextFactory = dataContextFactory;
            _mediator = mediator;
        }

        public async Task<bool> Handle(ChangeStaticSensorVisibilityStateCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var staticSensor = await context.StaticSensors.FirstOrDefaultAsync(f => f.Id == request.SensorId, cancellationToken);
                
            if (staticSensor == null)
            {
                throw new SensorNotFoundException("No Such Sensor");
            }
                
            staticSensor.IsVisible = request.IsVisible;
                
            await context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new StaticSensorVisibilityStateChangedNotification(staticSensor.Id),
                cancellationToken);
                
            return true;
        }
    }
}