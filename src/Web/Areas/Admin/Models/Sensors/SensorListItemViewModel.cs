namespace Web.Areas.Admin.Models.Sensors
{
    public class SensorListItemViewModel
    {
        public int Id { get; set; }

        public string IPAddress { get; set; }

        public bool IsActive { get; set; }

        public bool IsConnected { get; set; }
    }
}