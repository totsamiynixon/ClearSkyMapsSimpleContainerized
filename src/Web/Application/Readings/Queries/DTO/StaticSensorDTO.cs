using System.Collections.Generic;
using Web.Domain.Enums;

namespace Web.Application.Readings.Queries.DTO
{
    public class StaticSensorDTO
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public PollutionLevel PollutionLevel { get; set; }

        public List<StaticSensorReadingDTO> Readings { get; set; }
    }
}