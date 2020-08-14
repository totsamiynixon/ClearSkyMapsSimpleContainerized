namespace Web.Areas.Admin.Application.Readings.Queries.DTO
{
    public class SensorDTO
    {
        public int Id { get; set; }
        
        public string ApiKey { get; set; }
        
        public bool IsActive { get; set; }
    }
}