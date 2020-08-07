using System;
using MediatR;

namespace Web.Application.Emulation.Commands
{
    public class DevicePowerOnCommand : IRequest<bool>
    {
        public Guid DeviceId { get; }

        public DevicePowerOnCommand(string deviceId)
        {
            DeviceId = Guid.Parse(deviceId);
        }
    }
}