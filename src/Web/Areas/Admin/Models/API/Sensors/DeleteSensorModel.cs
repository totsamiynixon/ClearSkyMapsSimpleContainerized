using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.API.Sensors
{
    public class DeleteSensorModel
    {
        [Required]
        public int? Id { get; set; }

        public bool IsCompletely { get; set; }
    }
}