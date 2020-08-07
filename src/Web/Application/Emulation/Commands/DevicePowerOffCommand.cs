using System;
using MediatR;

namespace Web.Application.Emulation.Commands
{
    public class DevicePowerOffCommand : IRequest<bool>
    {
        public Guid DeviceId { get; }

        public DevicePowerOffCommand(string deviceId)
        {
            DeviceId = Guid.Parse(deviceId);
        }
    }
}