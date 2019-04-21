using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Enums;

namespace Web.Areas.Admin.Models.Emulator
{
    public class SensorEmulatorListItemViewModel
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public bool IsOn { get; set; }

        public string IPAddress { get; set; }

        public string Guid { get; set; }
    }
}