using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ActivateSensorModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public bool? IsActive { get; set; }
    }
}