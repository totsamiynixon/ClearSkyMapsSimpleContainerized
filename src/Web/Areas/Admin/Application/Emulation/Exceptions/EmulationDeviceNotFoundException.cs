using System;

namespace Web.Areas.Admin.Application.Emulation.Exceptions
{
    public class EmulationDeviceNotFoundException : Exception
    {
        public EmulationDeviceNotFoundException(Guid deviceId) : base($"Device with id: {deviceId} was not found")
        {
            
        }
    }
}