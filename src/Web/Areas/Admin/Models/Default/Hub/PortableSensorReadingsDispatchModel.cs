using System;

namespace Web.Areas.Admin.Models.Default.Hub
{
    public class PortableSensorReadingsDispatchModel
    {
        public float CO2 { get; set; }
        public float LPG { get; set; }
        public float CO { get; set; }
        public float CH4 { get; set; }
        public float Dust { get; set; }
        public float Temp { get; set; }
        public float Hum { get; set; }
        public float Preassure { get; set; }
        public DateTime Created { get; set; }
    }
}