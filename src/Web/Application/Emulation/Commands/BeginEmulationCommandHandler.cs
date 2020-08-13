using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Application.Emulation.Exceptions;
using Web.Application.Emulation.Notifications;
using Web.Emulation;
using Web.Infrastructure;

namespace Web.Application.Emulation.Commands
{
    public class BeginEmulationCommandHandler : IRequestHandler<BeginEmulationCommand, bool>
    {
        private readonly AppSettings _appSettings;
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        public BeginEmulationCommandHandler(Emulator emulator, AppSettings appSettings, IMediator mediator)
        {
            _emulator = emulator ?? throw new ArgumentNullException(nameof(emulator));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(BeginEmulationCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_appSettings.Emulation.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }
            
            await _emulator.RunEmulationAsync();

            await _mediator.Publish(new EmulationStartedNotification(_emulator), cancellationToken);
            
            return true;
        }
    }
}