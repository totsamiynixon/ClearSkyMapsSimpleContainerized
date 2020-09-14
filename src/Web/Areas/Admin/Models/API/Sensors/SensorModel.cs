namespace Web.Areas.Admin.Models.API.Sensors
{
    public class SensorModel
    {
        public int Id { get; set; }
        
        public string ApiKey { get; set; }
        
        public bool IsActive { get; set; }
    }
}