using System;

namespace Web.Areas.Admin.Application.Emulation.Exceptions
{
    public class EmulationIsNotAvailableException : Exception
    {
        public EmulationIsNotAvailableException() : base("Emulation is not available")
        {
            
        }
    }
}