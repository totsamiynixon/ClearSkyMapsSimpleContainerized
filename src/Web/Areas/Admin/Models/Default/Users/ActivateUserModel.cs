using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Users
{
    public class ActivateUserModel
    {
        [Required]
        public string Id { get; set; }

        public bool IsActive { get; set; }
    }
}