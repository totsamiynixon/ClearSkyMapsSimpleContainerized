using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Infrastructure.Data.Initialize;
using Web.Infrastructure.MediatR.Notifications;

namespace Web.Infrastructure.MediatR.Commands
{
    public class InitApplicationCommandHandler : IRequestHandler<InitApplicationCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IEnumerable<IApplicationDatabaseInitializer> _applicationDatabaseInitializers;

        public InitApplicationCommandHandler(IMediator mediator,
            IEnumerable<IApplicationDatabaseInitializer> applicationDatabaseInitializers)
        {
            _mediator = mediator;
            _applicationDatabaseInitializers = applicationDatabaseInitializers;
        }

        public async Task<bool> Handle(InitApplicationCommand request, CancellationToken cancellationToken)
        {
            foreach (var applicationDatabaseInitializer in _applicationDatabaseInitializers)
            {
                await applicationDatabaseInitializer.InitializeDbAsync();
            }

            await _mediator.Publish(new ApplicationInitializedNotification(), cancellationToken);

            return true;
        }
    }
}