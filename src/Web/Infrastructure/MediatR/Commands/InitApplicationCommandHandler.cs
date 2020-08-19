using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Infrastructure.Application.Notifications;
using Web.Infrastructure.Data.Initialize;

namespace Web.Infrastructure.MediatR.Commands
{
    public class InitApplicationCommandHandler : IRequestHandler<InitApplicationCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDatabaseInitializer _applicationDatabaseInitializer;

        public InitApplicationCommandHandler(IMediator mediator,
            IApplicationDatabaseInitializer applicationDatabaseInitializer)
        {
            _mediator = mediator;
            _applicationDatabaseInitializer = applicationDatabaseInitializer;
        }

        public async Task<bool> Handle(InitApplicationCommand request, CancellationToken cancellationToken)
        {
            await _applicationDatabaseInitializer.InitializeDbAsync();
            
            await _mediator.Publish(new ApplicationInitializedNotification(), cancellationToken);

            return true;
        }
    }
}