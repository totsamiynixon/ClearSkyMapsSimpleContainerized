using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

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