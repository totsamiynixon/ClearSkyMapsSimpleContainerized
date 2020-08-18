using System.Collections.Generic;

namespace Web.Areas.Admin.Models.API.Sensors
{
    public class SensorListModel
    {
        public List<StaticSensorListItemModel> StaticSensors { get; set; }
        
        public List<SensorListItemModel> PortableSensors { get; set; }
    }
}