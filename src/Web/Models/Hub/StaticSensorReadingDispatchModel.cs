using System;
using Web.Domain.Enums;

namespace Web.Models.Hub
{
    public class StaticSensorReadingDispatchModel
    {
        public int SensorId { get; set; }

        public ReadingDispatchModel Reading { get; set; }

        public PollutionLevel PollutionLevel { get; set; }
    }

    public class ReadingDispatchModel
    {
        public int Id { get; set; }
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