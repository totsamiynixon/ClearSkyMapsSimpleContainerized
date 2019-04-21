using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.Areas.Admin.Emulation
{
    public class SensorState
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsActive { get; set; }
    }
}