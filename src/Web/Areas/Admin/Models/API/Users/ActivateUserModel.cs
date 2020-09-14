using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.API.Users
{
    public class ActivateUserModel
    {
        [Required]
        public string Id { get; set; }

        public bool IsActive { get; set; }
    }
}