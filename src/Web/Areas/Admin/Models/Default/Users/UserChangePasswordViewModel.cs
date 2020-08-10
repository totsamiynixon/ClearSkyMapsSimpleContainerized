namespace Web.Areas.Admin.Models.Default.Users
{
    public class UserChangePasswordViewModel
    {
        public UserChangePasswordViewModel(UserChangePasswordModel model)
        {
            Model = model;
        }
        public UserChangePasswordModel Model { get;  }
    }
}