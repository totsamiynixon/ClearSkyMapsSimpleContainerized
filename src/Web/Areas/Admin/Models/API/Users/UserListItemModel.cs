namespace Web.Areas.Admin.Models.API.Users
{
    public class UserListItemModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}