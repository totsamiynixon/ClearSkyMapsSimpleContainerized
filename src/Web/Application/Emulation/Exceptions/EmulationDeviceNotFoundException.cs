using System;

namespace Web.Application.Emulation.Exceptions
{
    public class EmulationDeviceNotFoundException : Exception
    {
        public EmulationDeviceNotFoundException(Guid deviceId) : base($"Device with id: {deviceId} was not found")
        {
            
        }
    }
}