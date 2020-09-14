using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Application.Emulation.Notifications;
using Web.Areas.Admin.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Commands
{
    public class StopEmulationCommandHandler : IRequestHandler<StopEmulationCommand, bool>
    {
        private readonly EmulationAppSettings _emulationAppSettings;
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        public StopEmulationCommandHandler(EmulationAppSettings emulationAppSettings, Emulator emulator, IMediator mediator)
        {
            _emulationAppSettings = emulationAppSettings ?? throw new ArgumentNullException(nameof(emulationAppSettings));
            _emulator = emulator ?? throw new ArgumentNullException(nameof(emulator));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(StopEmulationCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_emulationAppSettings.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }

            _emulator.StopEmulation();

            await _mediator.Publish(new EmulationStoppedNotification(_emulator), cancellationToken);

            return true;
        }
    }
}