namespace Web.Areas.Admin.Models.Default.Users
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