using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Data.Models
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
