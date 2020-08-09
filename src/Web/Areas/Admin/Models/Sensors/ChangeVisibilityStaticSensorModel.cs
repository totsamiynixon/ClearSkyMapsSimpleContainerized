using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Sensors
{
    public class ChangeVisibilityStaticSensorModel
    {
        [Required]
        public int? Id { get; set; }
        
        public bool IsVisible { get; set; }
    }
}