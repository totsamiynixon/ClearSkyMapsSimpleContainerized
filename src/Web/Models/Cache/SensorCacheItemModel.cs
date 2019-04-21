using Web.Data.Models;
using Web.Enums;

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