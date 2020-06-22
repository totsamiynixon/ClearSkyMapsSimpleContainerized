using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Sensors
{
    public class DeleteSensorModel
    {
        [Required]
        public int? Id { get; set; }

        public bool IsCompletely { get; set; }
    }
}