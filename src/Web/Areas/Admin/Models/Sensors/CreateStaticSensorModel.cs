namespace Web.Areas.Admin.Models.Sensors
{
    public class CreateStaticSensorModel
    {
        public string ApiKey { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}