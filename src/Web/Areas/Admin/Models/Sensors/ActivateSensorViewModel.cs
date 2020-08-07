using System.ComponentModel.DataAnnotations;

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