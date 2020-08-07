namespace Web.Areas.Admin.Models.Emulator
{
    public class SensorEmulatorListItemViewModel
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool IsOn { get; set; }

        public string ApiKey { get; set; }

        public string Guid { get; set; }

        public string Type { get; set; }
    }
}