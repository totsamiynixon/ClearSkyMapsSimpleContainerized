using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.Areas.Admin.Models.Sensors
{
    public class SensorDetailsViewModel
    {
        public string IPAddress { get; set; }

        public bool IsActive { get; set; }
    }
}