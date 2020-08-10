using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.Default.Users
{
    public class DeleteUserModel
    {
        [Required]
        public string Id { get; set; }
    }
}