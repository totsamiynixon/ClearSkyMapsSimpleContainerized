namespace Web.Application.Readings.DTO
{
    public class SensorReadingDTO
    {
        public float CO2 { get; set; }
        public float LPG { get; set; }
        public float CO { get; set; }
        public float CH4 { get; set; }
        public float Dust { get; set; }
        public float Temp { get; set; }
        public float Hum { get; set; }
        public float Preassure { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}