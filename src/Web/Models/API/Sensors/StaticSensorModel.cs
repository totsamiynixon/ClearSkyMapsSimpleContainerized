using System.Collections.Generic;
using Web.Domain.Enums;

namespace Web.Models.API.Sensors
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