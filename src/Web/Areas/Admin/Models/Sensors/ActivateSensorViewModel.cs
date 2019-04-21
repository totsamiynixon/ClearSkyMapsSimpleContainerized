using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ActivateSensorViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }

        public SensorDetailsViewModel Details { get; set; }
    }
}