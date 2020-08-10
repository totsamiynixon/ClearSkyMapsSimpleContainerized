using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class ChangeActivationSensorModel
    {
        [Required]
        public int? Id { get; set; }
        
        public bool IsActive { get; set; }
    }
}