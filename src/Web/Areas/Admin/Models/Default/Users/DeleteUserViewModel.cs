namespace Web.Areas.Admin.Models.Default.Users
{
    public class DeleteUserViewModel
    {

        public DeleteUserViewModel(DeleteUserModel model)
        {
            Model = model;
        }
        public DeleteUserModel Model { get; }
    }
}