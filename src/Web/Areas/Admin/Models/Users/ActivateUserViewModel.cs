namespace Web.Areas.Admin.Models.Users
{
    public class ActivateUserViewModel
    {
        public ActivateUserViewModel(ActivateUserModel model)
        {
            Model = model;
        }

        public ActivateUserModel Model { get; }
    }
}