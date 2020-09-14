using System;
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
    public class ChangeSensorActivationStateCommandHandler : IRequestHandler<ChangeSensorActivationStateCommand, bool>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IMediator _mediator;

        public ChangeSensorActivationStateCommandHandler(IDataContextFactory<DataContext> dataContextFactory,
            IMediator mediator)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory)); 
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(ChangeSensorActivationStateCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor =
                await context.Sensors.FirstOrDefaultAsync(f => f.Id == request.SensorId, cancellationToken);
               
            if (sensor == null)
            {
                throw new SensorNotFoundException(request.SensorId);
            }
                
            sensor.IsActive = request.IsActive;

            await context.SaveChangesAsync(cancellationToken);
                
            await _mediator.Publish(new SensorActivationStateChangedNotification(sensor.Id), cancellationToken);

            return true;
        }
    }
}