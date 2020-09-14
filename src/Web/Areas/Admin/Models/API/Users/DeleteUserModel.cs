using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.API.Users
{
    public class DeleteUserModel
    {
        [Required]
        public string Id { get; set; }
    }
}