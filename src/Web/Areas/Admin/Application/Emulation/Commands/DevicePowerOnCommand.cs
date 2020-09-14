using System;
using MediatR;

namespace Web.Areas.Admin.Application.Emulation.Commands
{
    public class DevicePowerOnCommand : IRequest<bool>
    {
        public Guid DeviceId { get; }

        public DevicePowerOnCommand(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentException(nameof(deviceId));
            }
            
            DeviceId = Guid.Parse(deviceId);
        }
    }
}