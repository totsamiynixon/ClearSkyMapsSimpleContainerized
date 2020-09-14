using System.Collections.Generic;

namespace Web.Domain.Entities
{
    public class StaticSensor : Sensor
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual List<StaticSensorReading> Readings { get; set; }

        public bool IsVisible { get; set; }

        public bool IsAvailable()
        {
            return IsActive && IsVisible && !IsDeleted;
        }
    }
}
