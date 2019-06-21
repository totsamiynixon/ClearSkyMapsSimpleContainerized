using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Enums;

namespace Web.Areas.PWA.Models.Sensors
{
    public class StaticSensorModel
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public PollutionLevel PollutionLevel { get; set; }

        public List<StaticSensorReadingModel> Readings { get; set; }
    }
}
