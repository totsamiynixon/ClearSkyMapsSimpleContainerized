using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Application.Emulation.Notifications;
using Web.Areas.Admin.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Commands
{
    public class BeginEmulationCommandHandler : IRequestHandler<BeginEmulationCommand, bool>
    {
        private readonly EmulationAppSettings _emulationAppSettings;
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        public BeginEmulationCommandHandler(Emulator emulator, EmulationAppSettings emulationAppSettings, IMediator mediator)
        {
            _emulator = emulator ?? throw new ArgumentNullException(nameof(emulator));
            _emulationAppSettings = emulationAppSettings ?? throw new ArgumentNullException(nameof(emulationAppSettings));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(BeginEmulationCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_emulationAppSettings.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }
            
            _emulator.RunEmulation();

            await _mediator.Publish(new EmulationStartedNotification(_emulator), cancellationToken);
            
            return true;
        }
    }
}