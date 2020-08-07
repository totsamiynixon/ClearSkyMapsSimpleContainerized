using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Users
{
    public class DeleteUserViewModel
    {
        [Required]
        public string Id { get; set; }

        public string Email { get; set; }
    }
}