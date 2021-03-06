﻿using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class ChangeVisibilityStaticSensorModel
    {
        [Required]
        public int? Id { get; set; }

        [Display(Name = "Видимый")]
        public bool IsVisible { get; set; }
    }
}