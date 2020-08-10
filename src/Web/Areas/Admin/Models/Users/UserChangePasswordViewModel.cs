namespace Web.Areas.Admin.Models.Users
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