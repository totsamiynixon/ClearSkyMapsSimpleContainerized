namespace Web.Areas.Admin.Emulation
{
    public class SetSensorStateModel
    {
        public string TrackingKey { get; set; }

        public bool IsActive { get; set; }

        public string WebServerUrl { get; set; }
    }
}