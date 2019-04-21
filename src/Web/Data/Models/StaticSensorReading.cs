namespace Web.Data.Models
{
    public class StaticSensorReading : Reading
    {
        public StaticSensor StaticSensor { get; set; }
        public int StaticSensorId { get; set; }
    }
}
