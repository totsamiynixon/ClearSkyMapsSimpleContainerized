using System.Collections.Generic;

namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class SensorsIndexViewModel
    {
        public List<SensorListItemViewModel> PortableSensors { get; set; }

        public List<StaticSensorListItemViewModel> StaticSensors { get; set; }
    }
}