using Web.Domain.Enums;

namespace Web.Areas.Admin.Models.API.Sensors
{
    public class StaticSensorListItemModel : SensorListItemModel
    {
        public bool IsVisible { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public PollutionLevel PollutionLevel { get; set; }
    }
}