using System.ComponentModel.DataAnnotations;

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