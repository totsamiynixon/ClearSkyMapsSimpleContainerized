using System;

namespace Web.Areas.Admin.Application.Emulation.Queries.DTO
{
    public class EmulatorDeviceDTO
    {
        public string Type { get; set; }

        public bool IsPowerOn { get; set; }

        public string ApiKey { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
        
        public bool IsOn { get; set; }
        
        public Guid Guid { get; set; }
    }
}