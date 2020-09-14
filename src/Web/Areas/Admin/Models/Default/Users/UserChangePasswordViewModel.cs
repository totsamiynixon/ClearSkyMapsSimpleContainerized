namespace Web.Areas.Admin.Models.Default.Users
{
    public class UserChangePasswordViewModel
    {
        public UserChangePasswordViewModel(UserChangePasswordModel model, string email)
        {
            Model = model;
        }

        public UserChangePasswordModel Model { get; }

        public string Email { get; }
    }
}