using System;

namespace Web.Application.Emulation.Exceptions
{
    public class EmulationIsNotAvailableException : Exception
    {
        public EmulationIsNotAvailableException() : base("Emulation is not available")
        {
            
        }
    }
}