namespace Web.Areas.Admin.Models.Users
{
    public class ActivateUserViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}