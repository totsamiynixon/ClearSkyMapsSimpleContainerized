using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ChangeVisibilityStaticSensorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Видимый")]
        public bool IsVisible { get; set; }

        public SensorDetailsViewModel Details { get; set; }
    }
}