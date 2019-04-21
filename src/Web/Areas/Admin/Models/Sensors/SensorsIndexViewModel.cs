using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Sensors
{
    public class SensorsIndexViewModel
    {
        public List<SensorListItemViewModel> PortableSensors { get; set; }

        public List<StaticSensorListItemViewModel> StaticSensors { get; set; }
    }
}