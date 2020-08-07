using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.Models.Cache
{
    public class SensorCacheItemModel
    {

        public SensorCacheItemModel(StaticSensor sensor, PollutionLevel pollutionLevel)
        {
            Sensor = sensor;
            PollutionLevel = pollutionLevel;
        }

        public StaticSensor Sensor { get; set; }

        public PollutionLevel PollutionLevel { get; set; }
    }
}