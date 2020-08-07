using Web.Domain.Enums;

namespace Web.Areas.Admin.Models.Sensors
{
    public class StaticSensorListItemViewModel : SensorListItemViewModel
    {
        public bool IsVisible { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public PollutionLevel PollutionLevel { get; set; }
    }
}