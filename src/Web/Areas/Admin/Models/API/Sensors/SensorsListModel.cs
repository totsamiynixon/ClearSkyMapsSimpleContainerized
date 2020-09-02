using System.Collections.Generic;

namespace Web.Areas.Admin.Models.API.Sensors
{
    public class SensorsListModel
    {
       public List<SensorListItemModel> PortableSensors { get; set; }
       
       public List<StaticSensorListItemModel> StaticSensors { get; set; }
    }
}