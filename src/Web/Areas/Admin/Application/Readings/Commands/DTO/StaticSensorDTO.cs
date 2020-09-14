namespace Web.Areas.Admin.Application.Readings.Commands.DTO
{
    public class StaticSensorDTO : SensorDTO
    {
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public bool IsVisible { get; set; }
    }
}