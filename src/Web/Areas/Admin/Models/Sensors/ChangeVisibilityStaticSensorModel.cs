using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ChangeVisibilityStaticSensorModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public bool? IsVisible { get; set; }
    }
}