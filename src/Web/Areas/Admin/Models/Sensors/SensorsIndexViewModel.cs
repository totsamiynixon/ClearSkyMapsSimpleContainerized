using System.Collections.Generic;

namespace Web.Areas.Admin.Models.Sensors
{
    public class SensorsIndexViewModel
    {
        public List<SensorListItemViewModel> PortableSensors { get; set; }

        public List<StaticSensorListItemViewModel> StaticSensors { get; set; }
    }
}