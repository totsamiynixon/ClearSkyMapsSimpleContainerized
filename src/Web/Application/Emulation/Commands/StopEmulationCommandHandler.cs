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
    public class StopEmulationCommandHandler : IRequestHandler<StopEmulationCommand, bool>
    {
        private readonly AppSettings _appSettings;
        private readonly Emulator _emulator;
        private readonly IMediator _mediator;

        public StopEmulationCommandHandler(AppSettings appSettings, Emulator emulator, IMediator mediator)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _emulator = emulator ?? throw new ArgumentNullException(nameof(emulator));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(StopEmulationCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_appSettings.Emulation.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }

            _emulator.StopEmulation();

            await _mediator.Publish(new EmulationStoppedNotification(_emulator), cancellationToken);

            return true;
        }
    }
}