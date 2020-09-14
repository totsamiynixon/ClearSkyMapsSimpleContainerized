namespace Web.Areas.Admin.Models.API.Sensors
{
    public class StaticSensorModel : SensorModel
    {
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public bool IsVisible { get; set; }
    }
}