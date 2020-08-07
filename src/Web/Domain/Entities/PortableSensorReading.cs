namespace Web.Domain.Entities
{
    public class PortableSensorReading : Reading
    {
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
    }
}
