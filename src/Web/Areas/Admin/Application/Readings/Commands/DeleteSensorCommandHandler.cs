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
    public class DeleteSensorCommandHandler : IRequestHandler<DeleteSensorCommand, bool>
    {
        private readonly IDataContextFactory<DataContext> _dataContextFactory;
        private readonly IMediator _mediator;
        
        public DeleteSensorCommandHandler(IDataContextFactory<DataContext> dataContextFactory, IMediator mediator)
        {
            _dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        public async Task<bool> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
        {
            await using var context = _dataContextFactory.Create();
            var sensor = await context.Sensors.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);
            if (sensor == null)
            {
                throw new SensorNotFoundException("No Such Sensor");
            }
                
            if (!request.IsCompletely)
            {
                sensor.IsDeleted = true;
            }
            else
            { 
                context.Sensors.Remove(sensor);
            }
                
            await context.SaveChangesAsync(cancellationToken);

            await _mediator.Publish(new SensorDeletedNotification(request.Id), cancellationToken);

            return true;
        }
    }
}