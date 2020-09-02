using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Sensors
{
    public class CreatePortableSensorModel
    {
        [Required]
        public string ApiKey { get; set; }
    }
}