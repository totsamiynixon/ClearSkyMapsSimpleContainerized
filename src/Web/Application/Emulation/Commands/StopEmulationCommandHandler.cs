using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Application.Emulation.Exceptions;
using Web.Emulation;
using Web.Infrastructure;

namespace Web.Application.Emulation.Commands
{
    public class StopEmulationCommandHandler : IRequestHandler<StopEmulationCommand, bool>
    {
        private readonly AppSettings _appSettings;
        private readonly Emulator _emulator;

        public StopEmulationCommandHandler(AppSettings appSettings, Emulator emulator)
        {
            _appSettings = appSettings;
            _emulator = emulator;
        }

        public Task<bool> Handle(StopEmulationCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_appSettings.Emulation.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }
            
            _emulator.StopEmulation();

            return Task.FromResult(true);
        }
    }
}