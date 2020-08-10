namespace Web.Areas.Admin.Models.API.Emulator
{
    public class SensorEmulatorListItemModel
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool IsOn { get; set; }

        public string ApiKey { get; set; }

        public string Guid { get; set; }

        public string Type { get; set; }
    }
}