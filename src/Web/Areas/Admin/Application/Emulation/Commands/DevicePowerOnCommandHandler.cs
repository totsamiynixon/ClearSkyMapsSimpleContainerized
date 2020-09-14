using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Areas.Admin.Application.Emulation.Exceptions;
using Web.Areas.Admin.Emulation;

namespace Web.Areas.Admin.Application.Emulation.Commands
{
    public class DevicePowerOnCommandHandler : IRequestHandler<DevicePowerOnCommand, bool>
    {
        private readonly EmulationAppSettings _emulationAppSettings;
        private readonly Emulator _emulator;

        public DevicePowerOnCommandHandler(EmulationAppSettings emulationAppSettings, Emulator emulator)
        {
            _emulationAppSettings = emulationAppSettings ?? throw new ArgumentNullException(nameof(emulationAppSettings));
            _emulator = emulator ?? throw new ArgumentNullException(nameof(emulator));
        }

        public Task<bool> Handle(DevicePowerOnCommand request, CancellationToken cancellationToken)
        {
            //TODO: add validator for request

            if (!_emulationAppSettings.Enabled)
            {
                throw new EmulationIsNotAvailableException();
            }

            var device = _emulator.Devices.FirstOrDefault(f => f.GetGuid() == request.DeviceId.ToString());
            if (device == null)
            {
                throw new EmulationDeviceNotFoundException(request.DeviceId);
            }
            
            device.PowerOn();

            return Task.FromResult(true);
        }
    }
}